using UnityEngine;

namespace AI.Actions
{
    public class Chase : Action
    {
        private Transform _kid;
        
        public Chase(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Agent agent, bool hasTarget, string targetTag)
             : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget, targetTag)
        {
            
        }

        public override void Perform()
        {
            navMeshAgent.SetDestination(_kid.position);
        }
    
        public override bool PrePerform()
        {
            var kids = Object.FindObjectsOfType<SimpleCharacterController>();
            _kid = FindClosest(kids, navMeshAgent).transform;

            if (!_kid) return false;
            
            agent.AnimationState = AnimationState.Locomotion;

            return true;
        }

        public override bool PostPerform()
        {
            invoked = false;

            return success;
        }
    }
}
