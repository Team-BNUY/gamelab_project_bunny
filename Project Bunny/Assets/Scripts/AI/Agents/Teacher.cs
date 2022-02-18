using System;
using System.Collections.Generic;
using System.Linq;
using Input;
using UnityEngine;

namespace AI.Agents
{
    public class Teacher : Agent
    {
        public delegate void SeeBadStudent(StudentController student);
        public static event SeeBadStudent OnSeenBadStudent;
        
        [SerializeField] [Range(0f, 180f)] private float _fieldOfView;
        [SerializeField] [Min(0f)] private float _viewDistance;

        private StudentController[] _allStudents;
        private StudentController _targetStudent;
        private Dictionary<StudentController, Vector3> _badStudents = new Dictionary<StudentController, Vector3>();

        protected override void Start()
        {
            // Fetching all students
            _allStudents = FindObjectsOfType<StudentController>();
            
            // Goals
            var sentToTimeout = new State("caughtStudent", 1);
            var states = new StateSet(sentToTimeout);
            var goal = new Goal(states, false);
            goals.Add(goal, 1);
            
            // Creating actions
            base.Start();
        }

        private void Update()
        {
            if (WitnessedBadAction(out var badStudent))
            {
                _targetStudent = badStudent;
            }
        }

        private bool WitnessedBadAction(out StudentController badStudent)
        {
            var badStudentFound = false;
            badStudent = null;

            foreach (var student in _allStudents.OrderBy(s => Vector3.Distance(transform.position, s.transform.position)))
            {
                if(!student.hasSomethingEquipped) continue;
                
                var myTransform = transform;
                var myPosition = myTransform.position;
                var studentPosition = student.transform.position;
                
                if(Vector3.Distance(myPosition, studentPosition) > _viewDistance) continue;
                
                var angle = Vector3.Angle(myTransform.forward, studentPosition - myPosition);

                if (2f * angle > _fieldOfView) continue;
                
                if (_badStudents.ContainsKey(student))
                {
                    _badStudents[student] = studentPosition;
                }
                else
                {
                    _badStudents.Add(student, studentPosition);
                    OnSeenBadStudent?.Invoke(student);
                }
                
                if (!badStudent)
                {
                    badStudent = student;
                }
                
                badStudentFound = true;
            }

            return badStudentFound;
        }

        // Properties

        public StudentController TargetStudent
        {
            get => _targetStudent;
        }
    }
}
