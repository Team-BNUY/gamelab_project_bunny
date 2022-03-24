using System.Collections.Generic;
using System.Linq;
using AI.Core;
using Photon.Pun;
using Player;
using UnityEngine;

namespace AI.Agents
{
    public class Teacher : Agent
    {
        // Events
        public delegate void PlayerInteraction(NetworkStudentController player);
        public static event PlayerInteraction OnSeeNewBadStudent;
        public static event PlayerInteraction OnFoundBadStudent;
        public static event PlayerInteraction OnLostBadStudent;
        
        // View parameters
        [Header("View parameters")]
        [SerializeField] private Transform _head; 
        [SerializeField] [Range(0f, 180f)] private float _fieldOfView;
        [SerializeField] [Min(0f)] private float _viewDistance;
        [SerializeField] [Min(0f)] private float _headRotationSpeed = 10f;
        [SerializeField] private LayerMask _viewBlockingLayer;
        private Vector3 _viewDirection;
        private bool _lookingForward;
        
        // Stun parameters
        [SerializeField] private float _stunDuration = 2f;
        private bool _stunned;
        
        [Header("Waypoints")] 
        [SerializeField] private Transform[] _waypoints;
                
        // Students references
        private NetworkStudentController _targetStudent;
        private NetworkStudentController _lastTargetStudent;
        private Dictionary<NetworkStudentController, Vector3> _badStudents = new Dictionary<NetworkStudentController, Vector3>();
        private static readonly int StunnedAnim = Animator.StringToHash("Stunned");

        /// <summary>
        /// References all the students, add the goals and create the actions
        /// </summary>
        protected override void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;
        
            // Goals
            var state = new State("searchedForBadStudent", 1);
            var states = new StateSet(state);
            var goal = new Goal(states, false);
            goals.Add(goal, 1);
            
            state = new State("caughtBadStudent", 1);
            states = new StateSet(state);
            goal = new Goal(states, false);
            goals.Add(goal, 2);

            // Creating actions
            base.Start();
        }
        
        /// <summary>
        /// Targets a bad student if witnessed performing a bad action
        /// </summary>
        private void Update()
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient || _stunned) return;

            if (WitnessedBadAction(out var badStudent))
            {
                _targetStudent = badStudent;
            }
            else if(_targetStudent)
            {
                if (_targetStudent.IsDead)
                {
                    _badStudents.Remove(_targetStudent);
                    _targetStudent = null;
                }
                
                _lastTargetStudent = _targetStudent;
                OnLostBadStudent?.Invoke(_targetStudent);
                _targetStudent = null;
            }

            LookAtViewDirection();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_stunned) return;
            
            var projectile = other.CompareTag("Projectile");
            if (!projectile) return;

            _stunned = true;
            InterruptGoal();
            animationState = AnimationState.Idle;
            SetAnimatorParameters();
            animator.SetBool(StunnedAnim, true);
            Invoke(nameof(WakeUp), _stunDuration);
        }

        /// <summary>
        /// Subscribes to events
        /// </summary>
        protected override void OnEnable()
        {
            OnSeeNewBadStudent += SeeNewStudent;
            OnFoundBadStudent += FindStudent;
            OnLostBadStudent += LoseStudent;
        }
        
        /// <summary>
        /// Unsubscribes to events
        /// </summary>
        protected override void OnDisable()
        {
            OnSeeNewBadStudent -= SeeNewStudent;
            OnFoundBadStudent -= FindStudent;
            OnLostBadStudent -= LoseStudent;
        }

        /// <summary>
        /// Forces the Teacher to look forward
        /// </summary>
        public void LookForward()
        {
            _lookingForward = true;
        }
    
        /// <summary>
        /// Forces the Teacher to look at a <paramref name="direction"/>
        /// </summary>
        /// <param name="direction">Direction to look at</param>
        public void LookAtDirection(Vector3 direction)
        {
            _lookingForward = false;
            _viewDirection = direction;
        }

        public static void LoseDeadPlayer(NetworkStudentController player)
        {
            OnLostBadStudent?.Invoke(player);
        }
        
        /// <summary>
        /// Remembers every student seen performing a bad action, updates their last seen position and determines which bad student is the closest
        /// </summary>
        /// <param name="badStudent">The closest bad student seen</param>
        /// <returns>True if at least one bad student has been seen during the frame</returns>
        private bool WitnessedBadAction(out NetworkStudentController badStudent)
        {
            var badStudentFound = false;
            badStudent = null;
            
            // Ordering the students by distance and iterating through all of them
            foreach (var student in ArenaManager.Instance.AllPlayers.OrderBy(s => Vector3.Distance(transform.position, s.transform.position)))
            {   
                if(!student.HasSnowball && !_badStudents.ContainsKey(student)) continue;
                
                // If the student holds a tool or if it is already in the list of bad students
                var myPosition = transform.position;
                var studentPosition = student.transform.position;
                if(Vector3.Distance(myPosition, studentPosition) > _viewDistance) continue;
                
                // If the student is within view distance
                var angle = Vector3.Angle(_viewDirection, studentPosition - myPosition);
                if (2f * angle > _fieldOfView) continue;

                var direction = student.transform.position - _head.transform.position + Vector3.up * 2.5f; // TODO Take student height somewhere else
                var distance = Vector3.Distance(studentPosition, transform.position);
                if (Physics.Raycast(_head.transform.position, direction, distance, _viewBlockingLayer)) continue;
                    
                // If the student is within field of view
                // Updates the bad student's last seen position if already in the list of bad students
                if (_badStudents.ContainsKey(student))
                {
                    _badStudents[student] = studentPosition;
                    OnFoundBadStudent?.Invoke(student);
                }
                // Adds the new bad student to the list of bad student and invokes the OnSeenBadStudent event
                else
                {
                    _badStudents.Add(student, studentPosition);
                    OnSeeNewBadStudent?.Invoke(student);
                }
                
                // Remembers the closest bad student seen 
                if (!badStudent)
                {
                    badStudent = student;
                }
                
                badStudentFound = true;
            }

            return badStudentFound;
        }
        
        /// <summary>
        /// Rotates the Teacher's head to face the view direction
        /// </summary>
        private void LookAtViewDirection()
        {
            if (_lookingForward)
            {
                _viewDirection = transform.forward;
            }
            
            var lookRotation = Quaternion.LookRotation(_viewDirection, Vector3.up);
            var lerpRotation = Quaternion.Lerp(_head.rotation, lookRotation, _headRotationSpeed * Time.deltaTime);
            
            _head.rotation = lerpRotation;
        }
        
        /// <summary>
        /// Adds the "seesBadStudent" state to the Teacher's beliefs states if not already present
        /// </summary>
        /// <param name="student">The student found</param>
        private void FindStudent(NetworkStudentController student)
        {
            beliefStates.AddState("seesBadStudent", 1);
            
            if (currentAction is {Name: "Investigate"})
            {
                InterruptGoal();
            }
        }

        /// <summary>
        /// Adds the "seesBadStudent" state to the Teacher's beliefs states if not already present and remembers the student's position
        /// </summary>
        /// <param name="student">The new bad student that has been seen</param>
        private void SeeNewStudent(NetworkStudentController student)
        {
            beliefStates.AddState("seesBadStudent", 1);
            RememberStudent(student);
            
            if (currentAction is {Name: "Investigate"} || currentAction is {Name: "Surveil"})
            {
                InterruptGoal();
            }
        }
        
        /// <summary>
        /// Adds the "remembersStudent" state to the Teacher's beliefs states or increases this state by one if already present
        /// </summary>
        /// <param name="student">The bad student to remember</param>
        private void RememberStudent(NetworkStudentController student)
        {
            beliefStates.ModifyState("remembersBadStudent", 1);
        }
        
        /// <summary>
        /// Removes the "seesBadStudent" state from the Teacher's beliefs states
        /// </summary>
        /// <param name="student">The student that has just been lost</param>
        private void LoseStudent(NetworkStudentController student)
        {
            beliefStates.RemoveState("seesBadStudent");

            if (currentAction is {Name: "Chase Student"})
            {
                InterruptGoal();
            }
        }

        private void WakeUp()
        {
            _stunned = false;
            animator.SetBool(StunnedAnim, false);
        }

        // Properties

        public float FieldOfView
        {
            set => _fieldOfView = value;
        }

        public NetworkStudentController TargetStudent
        {
            get => _targetStudent;
        }

        public NetworkStudentController LastTargetStudent
        {
            get => _lastTargetStudent;
            set => _lastTargetStudent = value;
        }
        
        public Dictionary<NetworkStudentController, Vector3> BadStudents
        {
            get => _badStudents;
        }

        public Transform[] Waypoints
        {
            get => _waypoints;
        }

        public bool Stunned
        {
            get => _stunned;
        }
    }
}
