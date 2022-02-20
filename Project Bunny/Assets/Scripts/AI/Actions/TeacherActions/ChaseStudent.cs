using AI.Agents;
using UnityEngine;

namespace AI.Actions.TeacherActions
{
    public class ChaseStudent : Action
    {
        private Teacher _teacher;
        private float _speed;
        
        private Transform _target;
        
        public ChaseStudent(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Teacher teacher, bool hasTarget, float speed)
             : base(name, cost, preconditionStates, afterEffectStates, teacher, hasTarget)
        {
            _teacher = teacher;
            _speed = speed;
        }
        
        /// <summary>
        /// Sets Action's target as the Teacher's current student target
        /// </summary>
        /// <returns>True if the Teacher has a student target</returns>
        public override bool PrePerform()
        {
            if (_teacher.TargetStudent)
            {
                _target = _teacher.TargetStudent.transform;
            }

            if (!_target) return false;
            
            agent.AnimationState = AnimationState.Locomotion;

            return true;
        }
        
        /// <summary>
        /// Chases the Action's target
        /// </summary>
        public override void Perform()
        {
            navMeshAgent.speed = _speed;
            navMeshAgent.SetDestination(_target.position);
        }
        
        /// <summary>
        /// Resets the Action's target
        /// </summary>
        /// <returns>Always true, no possible fail for this post-processing</returns>
        public override bool PostPerform()
        {
            _target = null;
            
            invoked = false;

            return success;
        }
    }
}
