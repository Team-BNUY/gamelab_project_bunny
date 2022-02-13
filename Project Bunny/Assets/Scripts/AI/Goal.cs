using UnityEngine;

namespace AI
{
    public class Goal
    {
        private StateSet stateSet;
        private bool temporary;
        
        /// <summary>
        /// Constructor that sets the Goal's <paramref name="stateSet"/> and whether or not it is temporary (removed after completion)
        /// </summary>
        /// <param name="stateSet">The Goal's set of states</param>
        /// <param name="temporary">Whether or not the goals is temporary (removed after completion)</param>
        public Goal(StateSet stateSet, bool temporary)
        {
            this.stateSet = stateSet;
            this.temporary = temporary;
        }
        
        // Properties
        
        public StateSet StateSet => stateSet;
        public bool Temporary => temporary;
    }
}
