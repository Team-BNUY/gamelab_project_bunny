using System.Linq;
using AI.Agents;
using Player;
using UnityEngine;

namespace AI.Actions.TeacherActions
{
    public class Investigate : Action
    {
        private Teacher _teacher;
        private StudentController _targetStudent;
        private float _investigationTime;
        private float _newFieldOfView;
        private float _originalFieldOfView;
        
        public Investigate(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Teacher agent, bool hasTarget, float investigationTime, float newFieldOfView)
             : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _teacher = agent;
            _investigationTime = investigationTime;
            _newFieldOfView = newFieldOfView;
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
        /// Sets the Teacher's increased field of view and selects the nearest bad student remembered to investigate the position
        /// </summary>
        /// <returns>Always true, no possible fail for this pre-processing</returns>
        public override bool PrePerform()
        {
            // Sets the new field of view
            _originalFieldOfView = _teacher.FieldOfView;
            _teacher.FieldOfView = _newFieldOfView;
            
            // To make sure the Teacher goes first to the last remembered position of the student they were chasing
            if (_teacher.TargetStudent)
            {
                _targetStudent = _teacher.TargetStudent;
                _teacher.TargetStudent = null;
                
                return true;
            }
            
            // Determines the nearest remembered bad student and sets them as the target of the investigation
            var orderedBadStudents = _teacher.BadStudents.OrderBy(x => Vector3.Distance(x.Value, _teacher.transform.position)).First();
            _targetStudent = orderedBadStudents.Key;
            target = orderedBadStudents.Value;

            return true;
        }
        
        /// <summary>
        /// Performs the action for the investigation time
        /// </summary>
        public override void Perform()
        {
            // Interrupts the action if the Teacher found a bad student during the investigation
            if (_teacher.TargetStudent)
            {
                PostPerform();
                _teacher.InterruptGoal();
            }
            
            //_teacher.SetAnimatorParameter("Investigate", true);
            _investigationTime -= Time.deltaTime;
            
            if (_investigationTime > 0f) return;
            
            if (invoked) return;
            invoked = true;
            
            agent.CompleteAction();
        }
        
        /// <summary>
        /// Resets the field of view to the original one, removes the target student from the Teacher's list of bad students and decreases the "remembersBadStudent" state by one from the Teacher
        /// </summary>
        /// <returns>Always true, no fail possible fail for this post-processing</returns>
        public override bool PostPerform()
        {
            _teacher.FieldOfView = _originalFieldOfView;
            _teacher.BadStudents.Remove(_targetStudent);
            _teacher.BeliefStates.ModifyState("remembersBadStudent", -1);

            invoked = false;
            
            return true;
        }
    }
}
