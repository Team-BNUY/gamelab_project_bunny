using System.Linq;
using AI.Agents;
using AI.Core;
using Player;
using UnityEngine;

namespace AI.Actions.TeacherActions
{
    public class Investigate : Action
    {
        private Teacher _teacher;
        
        private StudentController _targetStudent;
        private float _investigationTime;
        private float _walkingFieldOfView;
        private float _investigationFieldOfView;

        private float _timer;
        
        public Investigate(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Teacher agent, bool hasTarget, float investigationTime, float walkingFieldOfView, float investigationFieldOfView)
             : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _teacher = agent;
            _investigationTime = investigationTime;
            _walkingFieldOfView = walkingFieldOfView;
            _investigationFieldOfView = investigationFieldOfView;
        }
        
        /// <summary>
        /// Determines if the Teacher can investigate a remembered position of a bad student
        /// </summary>
        /// <returns>True if there is at least one bad student in the bad students list</returns>
        public override bool IsAchievable()
        {
            return _teacher.BadStudents.Count != 0;
        }
        
        /// <summary>
        /// Selects the nearest bad student remembered to investigate the position
        /// </summary>
        /// <returns>Always true, no possible fail for this pre-processing</returns>
        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;
            _timer = _investigationTime;
            
            // View parameters
            _teacher.FieldOfView = _walkingFieldOfView;
            _teacher.LookForward();
            
            // To make sure the Teacher goes first to the last remembered position of the student they were chasing
            if (_teacher.LastTargetStudent)
            {
                _targetStudent = _teacher.LastTargetStudent;
                target = _teacher.BadStudents[_targetStudent];
                _teacher.LastTargetStudent = null;

                return true;
            }
            
            // Determines the nearest remembered bad student and sets them as the target of the investigation
            var orderedBadStudents = _teacher.BadStudents.OrderBy(x => Vector3.Distance(x.Value, _teacher.transform.position)).First();
            _targetStudent = orderedBadStudents.Key;
            target = orderedBadStudents.Value;

            return true;
        }
        
        /// <summary>
        /// Sets the Teacher's increased field of view and view distance and performs the action for the investigation time
        /// </summary>
        public override void Perform()
        {
            // View parameters
            _teacher.FieldOfView = _investigationFieldOfView;
            
            //_teacher.SetAnimatorParameter("Investigate", true);
            _timer -= Time.deltaTime;
            
            if (_timer > 0f) return;
            
            if (invoked) return;
            invoked = true;
            
            agent.CompleteAction();
        }
        
        /// <summary>
        /// Removes the target student from the Teacher's list of bad students and decreases the "remembersBadStudent" state by one from the Teacher
        /// </summary>
        /// <returns>Always true, no fail possible fail for this post-processing</returns>
        public override bool PostPerform()
        {
            _teacher.BadStudents.Remove(_targetStudent);
            _teacher.BeliefStates.ModifyState("remembersBadStudent", -1);
            
            return true;
        }

        public override void OnInterrupt()
        {
            return;
        }
    }
}
