namespace AI.Core
{
    public class Goal
    {
        private StateSet _stateSet;
        private bool _temporary;
        
        /// <summary>
        /// Constructor that sets the Goal's <paramref name="stateSet"/> and whether or not it is temporary (removed after completion)
        /// </summary>
        /// <param name="stateSet">The Goal's set of states</param>
        /// <param name="temporary">Whether or not the goals is temporary (removed after completion)</param>
        public Goal(StateSet stateSet, bool temporary)
        {
            _stateSet = stateSet;
            _temporary = temporary;
        }
        
        // Properties
        
        public StateSet StateSet => _stateSet;
        public bool Temporary => _temporary;
    }
}
