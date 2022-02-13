using UnityEngine;

namespace AI
{
    public abstract class ActionData : ScriptableObject
    {
        [SerializeField]
        protected new string name = "Action";
        [SerializeField]
        protected int cost;
        [SerializeField]
        protected bool hasTarget;
        [SerializeField]
        protected string targetTag;
        [SerializeField]
        protected State[] preconditionStates;
        [SerializeField]
        protected State[] afterEffectStates;
        
        /// <summary>
        /// Creates an Action out of this ActionData and sends it to the <paramref name="agent"/>'s possible actions
        /// </summary>
        /// <param name="agent">The agent to send the action</param>
        public abstract void Create(Agent agent);
        
        // Properties
        
        public bool HasTarget => hasTarget;
    }
}
