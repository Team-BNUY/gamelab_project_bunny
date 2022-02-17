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

        public override bool PrePerform()
        {
            _target = FindClosest(_teacher.BadStudents, navMeshAgent)?.transform;

            if (!_target) return false;
            
            agent.AnimationState = AnimationState.Locomotion;

            return true;
        }
        
        public override void Perform()
        {
            navMeshAgent.speed = _speed;
            navMeshAgent.SetDestination(_target.position);
        }

        public override bool PostPerform()
        {
            invoked = false;

            return success;
        }
    }
}
