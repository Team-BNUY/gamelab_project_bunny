using System.Collections.Generic;
using System.Linq;
using AI.Core;
using Networking;
using Photon.Pun;
using Player;
using UnityEngine;

namespace AI.Agents
{
    public class Teacher : Agent, IPunObservable
    {
        // Events
        public delegate void PlayerInteraction(NetworkStudentController player);
        public static event PlayerInteraction OnSeeNewBadStudent;
        public static event PlayerInteraction OnFoundBadStudent;
        public static event PlayerInteraction OnLostBadStudent;
        
        // References
        [Header("References")]
        [SerializeField] private Transform _exclamationMarkPosition;
        
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
        private Vector3 _hitDirection;
        private bool _stunned;
        
        // Audio
        [Header("Audio")]
        [SerializeField] private AudioClip _hitBySnowballSound;
        [SerializeField] private AudioClip _hitByIceballSound;
        [SerializeField] private AudioClip _hitByRollballSound;
        [SerializeField] private AudioClip _stunSound;
        private AudioSource _audioSource;

        // Students references
        private NetworkStudentController _targetStudent;
        private NetworkStudentController _lastTargetStudent;
        private Dictionary<NetworkStudentController, Vector3> _badStudents = new Dictionary<NetworkStudentController, Vector3>();
        
        private static readonly int StunnedAnim = Animator.StringToHash("Stunned");
        private static readonly int LookingAround = Animator.StringToHash("LookingAround");

        /// <summary>
        /// References all the students, add the goals and create the actions
        /// </summary>
        protected override void Start()
        {
            _audioSource ??= GetComponent<AudioSource>();
            
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
            
            // Investigate animation for when the teacher is first spawned
            _stunned = true;
            animator.SetBool(LookingAround, true);
            Invoke(nameof(StartSurveilling), ArenaManager.Instance.TeacherPreparationTime);
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

                if (!_lastTargetStudent || _lastTargetStudent != _targetStudent)
                {
                    if (_lastTargetStudent && currentAction is {Name: "Chase Student"})
                    {
                        if (_lastTargetStudent.IsDead)
                        {
                            _badStudents.Remove(_lastTargetStudent);
                        }
                        
                        InterruptGoal();
                    }
                    
                    _lastTargetStudent = _targetStudent;
                }
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

        private void OnCollisionEnter(Collision collision)
        {
            var projectile = collision.gameObject.CompareTag("Projectile");
            if (!projectile) return;
            
            if (collision.gameObject.TryGetComponent<NetworkGiantRollball>(out var giantRollball) && !giantRollball.CanDamage) return;

            if (!_stunned)
            {
                photonView.RPC(nameof(Stun), RpcTarget.All);
                
                if (collision.gameObject.TryGetComponent<NetworkSnowball>(out var ball))
                {
                    if (ball._studentThrower.photonView.IsMine)
                    {
                        ScoreManager.Instance.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, ScoreManager.REBEL_KEY);
                    }
                }
            }

            if (giantRollball)
            {
                PlayHitAudio(2);
                return;
            }
            
            var snowball = collision.gameObject.GetComponent<NetworkSnowball>();
            if (!snowball) return;
            
            // Hit by a snowball
            var throwerPosition = snowball._studentThrower.transform.position;
            var adjustedThrowerPosition = new Vector3(throwerPosition.x, transform.position.y, throwerPosition.z);
            var hitDirection = adjustedThrowerPosition - transform.position;
            var angle = Vector3.SignedAngle(transform.forward, _hitDirection, Vector3.up);

            if (!_stunned)
            {
                photonView.RPC(nameof(GetHitByProjectile), RpcTarget.All);
            }
            
            photonView.RPC(nameof(SyncHitDirection), RpcTarget.All, hitDirection);
            
            SetAnimatorParameter("Hit", true, true);
            if (angle < 0 && angle >= -45f || angle >= 0 && angle < 45f)
            {
                SetAnimatorParameter("HitFront", true, true);
            }
            else if (angle < -45f && angle >= -135f)
            {
                SetAnimatorParameter("HitLeft", true, true);
            }
            else if (angle >= 45f && angle < 135f)
            {
                SetAnimatorParameter("HitRight", true, true);
            }
            else
            {
                SetAnimatorParameter("HitBack", true, true);
            }
            
            PlayHitAudio(snowball.IsIceBall ? 1 : 0);
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
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_stunned);
                stream.SendNext(_hitDirection);
            }
            else
            {
                _stunned = (bool) stream.ReceiveNext();
                _hitDirection = (Vector3) stream.ReceiveNext();
            }
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
        
        public void Unhit()
        {
            SetAnimatorParameter("Hit", false);
        }
        
        public void UnhitSides()
        {
            SetAnimatorParameter("HitFront", false);
            SetAnimatorParameter("HitBack", false);
            SetAnimatorParameter("HitLeft", false);
            SetAnimatorParameter("HitRight", false);
        }

        private void StartSurveilling()
        {
            _stunned = false;
            animator.SetBool(LookingAround, false);
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
                if(!student.HasSnowball && !student.UsingCannon && !_badStudents.ContainsKey(student)) continue;
                
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
                badStudent ??= student;
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
            if (student != _targetStudent)
            {
                photonView.RPC(nameof(SetExclamationMark), student.photonView.Owner, true);
            }
            
            beliefStates.AddState("seesBadStudent", 1);
            
            if (currentAction is {Name: "Investigate"} || currentAction is {Name: "SurveilAfterHit"})
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
            photonView.RPC(nameof(SetExclamationMark), student.photonView.Owner, true);
            
            beliefStates.AddState("seesBadStudent", 1);
            RememberStudent(student);
            
            if (currentAction is {Name: "Investigate"} || currentAction is {Name: "Surveil"} || currentAction is {Name: "SurveilAfterHit"})
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
            if (student)
            {
                photonView.RPC(nameof(SetExclamationMark), student.photonView.Owner, false);
            }

            beliefStates.RemoveState("seesBadStudent");

            if (currentAction is {Name: "Chase Student"})
            {
                InterruptGoal();
            }
        }
        
        private void EnableExclamationMark()
        {
            ArenaManager.Instance.ExclamationMark.gameObject.SetActive(true);
            Invoke(nameof(DisableExclamationMark), 2f);
        }
        
        private void DisableExclamationMark()
        {
            ArenaManager.Instance.ExclamationMark.gameObject.SetActive(false);
        }
        
        private void PlayHitAudio(int ballType)
        {
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(PlayHitAudioRpc), RpcTarget.All, ballType);
            }
        }

        [PunRPC]
        private void PlayHitAudioRpc(int ballType)
        {
            switch (ballType)
            {
                case 0:
                    _audioSource.PlayOneShot(_hitBySnowballSound, 2f * AudioManager.Instance.Volume);
                    break;
                case 1:
                    _audioSource.PlayOneShot(_hitByIceballSound, 2f * AudioManager.Instance.Volume);
                    break;
                default:
                    _audioSource.PlayOneShot(_hitByRollballSound, 2f * AudioManager.Instance.Volume);
                    break;
            }
        }
        
        [PunRPC]
        private void SetExclamationMark(bool active)
        {
            DisableExclamationMark();

            if (active)
            {
                Invoke(nameof(EnableExclamationMark), 0.1f);
            }
        }

        [PunRPC]
        private void Stun()
        {
            _stunned = true;
            InterruptGoal();
            animationState = AnimationState.Idle;
            SetAnimatorParameters();
            animator.SetBool(StunnedAnim, true);
            Invoke(nameof(WakeUp), _stunDuration);
        }
        
        [PunRPC]
        private void SyncHitDirection(Vector3 hitDirection)
        {
            _hitDirection = hitDirection;
        }
        
        [PunRPC]
        private void GetHitByProjectile()
        {
            beliefStates.AddState("hitByProjectile", 1);
        }
        
        [PunRPC]
        private void SetBoolRPC(string boolean, bool value)
        {
            animator.SetBool(boolean, value);
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

        public bool Stunned
        {
            get => _stunned;
        }

        public Vector3 HitDirection
        {
            get => _hitDirection;
        }

        public Transform ExclamationMarkPosition
        {
            get => _exclamationMarkPosition;
        }

        public AudioClip StunAudio
        {
            get => _stunSound;
        }
    }
}
