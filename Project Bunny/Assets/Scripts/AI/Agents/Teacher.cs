using System.Collections.Generic;
using System.Linq;
using Input;
using UnityEngine;

namespace AI.Agents
{
    public class Teacher : Agent
    {
        // Events
        public delegate void SeeBadStudent(StudentController student);
        public static event SeeBadStudent OnSeenBadStudent;
        
        // View parameters
        [SerializeField] [Range(0f, 180f)] private float _fieldOfView;
        [SerializeField] [Min(0f)] private float _viewDistance;
        
        // Students references
        private StudentController[] _allStudents;
        private StudentController _targetStudent;
        private Dictionary<StudentController, Vector3> _badStudents = new Dictionary<StudentController, Vector3>();
        
        /// <summary>
        /// References all the students, add the goals and create the actions
        /// </summary>
        protected override void Start()
        {
            // Fetching all students
            _allStudents = FindObjectsOfType<StudentController>(); // TODO Take from an eventual future GameManager
            
            // Goals
            var sentToTimeout = new State("caughtStudent", 1);
            var states = new StateSet(sentToTimeout);
            var goal = new Goal(states, false);
            goals.Add(goal, 1);
            
            // Creating actions
            base.Start();
        }
        
        /// <summary>
        /// Targets a bad student if witnessed performing a bad action
        /// </summary>
        private void Update()
        {
            if (WitnessedBadAction(out var badStudent))
            {
                _targetStudent = badStudent;
            }
        }
        
        /// <summary>
        /// Subscribes to the OnSeenBadStudent(Student student) event
        /// </summary>
        private void OnEnable()
        {
            OnSeenBadStudent += RememberStudent;
        }
        
        /// <summary>
        /// Unsubscribes to the OnSeenBadStudent(Student student) event
        /// </summary>
        private void OnDisable()
        {
            OnSeenBadStudent -= RememberStudent;
        }
        
        /// <summary>
        /// Remembers every student seen performing a bad action, updates their last seen position and determines which bad student is the closest
        /// </summary>
        /// <param name="badStudent">The closest bad student seen</param>
        /// <returns>True if at least one bad student has been seen during the frame</returns>
        private bool WitnessedBadAction(out StudentController badStudent)
        {
            var badStudentFound = false;
            badStudent = null;
            
            // Ordering the students by distance and iterating through all of them
            foreach (var student in _allStudents.OrderBy(s => Vector3.Distance(transform.position, s.transform.position)))
            {   
                //if(!student.hasSomethingEquipped && !_badStudents.ContainsKey(student)) continue;
                
                // If the student holds a tool or if it is already in the list of bad students
                var myTransform = transform;
                var myPosition = myTransform.position;
                var studentPosition = student.transform.position;
                
                if(Vector3.Distance(myPosition, studentPosition) > _viewDistance) continue;
                
                // If the student is within view distance
                var angle = Vector3.Angle(myTransform.forward, studentPosition - myPosition);

                if (2f * angle > _fieldOfView) continue;
                
                // If the student is within field of view
                // Updates the bad student's last seen position if already in the list of bad students
                if (_badStudents.ContainsKey(student))
                {
                    _badStudents[student] = studentPosition;
                }
                // Adds the new bad student to the list of bad student and invokes the OnSeenBadStudent event
                else
                {
                    _badStudents.Add(student, studentPosition);
                    OnSeenBadStudent?.Invoke(student);
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
        /// Adds the "seenBadStudent" state to the Teacher's beliefs states or increases this state by one if already present
        /// </summary>
        /// <param name="student">The new bad student that has been seen</param>
        private void RememberStudent(StudentController student)
        {
            beliefStates.ModifyState("seenBadStudent", 1);
        }

        // Properties

        public StudentController TargetStudent
        {
            get => _targetStudent;
        }
    }
}
