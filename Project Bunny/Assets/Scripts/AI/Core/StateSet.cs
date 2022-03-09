using System.Collections.Generic;
using System.Linq;

namespace AI.Core
{
    public class StateSet
    {
        private List<State> _states;
        
        /// <summary>
        /// Default constructor initializing an empty list of State
        /// </summary>
        public StateSet()
        {
            _states = new List<State>();
        }

        /// <summary>
        /// Constructor initializing a new list of State with <paramref name="state"/>
        /// </summary>
        /// <param name="state">The state to add to the states</param>
        public StateSet(State state)
        {
            _states = new List<State> { new State(state) };
        }
    
        /// <summary>
        /// Constructor initializing a new list of State out of existing <paramref name="states"/>
        /// </summary>
        /// <param name="states">An enumerable of states to add to the states</param>
        public StateSet(IEnumerable<State> states)
        {
            this._states = new List<State>();
            foreach(var state in states)
            {
                var stateCopy = new State(state);
                this._states.Add(stateCopy);
            }
        }
        
        /// <summary>
        /// Copy constructor initializing a new list of State out of an existing <paramref name="stateSet"/>
        /// </summary>
        /// <param name="stateSet"></param>
        public StateSet(StateSet stateSet)
        {
            _states = new List<State>();
            foreach (var stateCopy in stateSet._states.Select(s => new State(s)))
            {
                _states.Add(stateCopy);
            }
        }
        
        /// <summary>
        /// Checks whether or not a State with <paramref name="key"/> already exists in the states
        /// </summary>
        /// <param name="key">The key of the State to search for</param>
        /// <returns>True if the states contain a State with <paramref name="key"/></returns>
        public bool HasState(string key)
        {
            return _states.Any(s => s.Key == key);
        }
        
        /// <summary>
        /// Adds a new State to the states
        /// </summary>
        /// <param name="state">The State to add to the states</param>
        /// <param name="keepLowestValue">Whether or not the lowest value should be kept if <paramref name="state"/> already exists in the states</param>
        public void AddState(State state, bool keepLowestValue = false)
        {
            if (HasState(state.Key))
            {
                var stateInSet = GetState(state.Key);
                if (keepLowestValue)
                {
                    if (state.Value < stateInSet.Value)
                    {
                        stateInSet.SetValue(state.Value);
                    }
                }
                else if (state.Value > stateInSet.Value)
                {
                    stateInSet.SetValue(state.Value);
                }

                return;
            }

            _states.Add(new State(state));
        }
        
        /// <summary>
        /// Adds a new State to the states
        /// </summary>
        /// <param name="key">The key of the State to add to the states</param>
        /// <param name="value">The value of the State to add to the states</param>
        /// <param name="keepLowestValue">Whether or not the lowest value should be kept if a State with <paramref name="key"/> already exists in the states</param>
        public void AddState(string key, int value, bool keepLowestValue = false)
        {
            if (HasState(key))
            {
                var stateInSet = GetState(key);
                if (keepLowestValue)
                {
                    if (value < stateInSet.Value)
                    {
                        stateInSet.SetValue(value);
                    }
                }
                else if (value > stateInSet.Value)
                {
                    stateInSet.SetValue(value);
                }

                return;
            }

            _states.Add(new State(key, value));
        }

        /// <summary>
        /// Adds a StateSet to the states
        /// </summary>
        /// <param name="stateSet">The StateSet to add to the states</param>
        /// <param name="keepLowestValues">Whether or not the lowest value should be kept if a State within <paramref name="stateSet"/> already exists in the states</param>
        public void AddStates(StateSet stateSet, bool keepLowestValues = false)
        {
            foreach (var state in stateSet._states)
            {
                AddState(state, keepLowestValues);
            }
        }
        
        /// <summary>
        /// Adds an enumerable of states to the states
        /// </summary>
        /// <param name="stateSet">The enumerable of states to add to the states</param>
        /// <param name="keepLowestValues">Whether or not the lowest value should be kept if a State within <paramref name="stateSet"/> already exists in the states</param>
        public void AddStates(IEnumerable<State> stateSet, bool keepLowestValues = false)
        {
            foreach (var state in stateSet)
            {
                AddState(state);
            }
        }
        
        /// <summary>
        /// Modifies a State's value by <paramref name="value"/> if it already exists in the states or adds it otherwise
        /// </summary>
        /// <param name="key">The key of the State to modify</param>
        /// <param name="value">The value modifier of the State</param>
        public void ModifyState(string key, int value)
        {
            if(HasState(key))
            {
                var stateInSet = GetState(key);
                stateInSet.AddValue(value);
                
                if (stateInSet.Value <= 0)
                {
                    RemoveState(stateInSet.Key);
                }
            }
            else if (value > 0)
            {
                _states.Add(new State(key, value));
            }
        }
        
        /// <summary>
        /// Removes a State with <paramref name="key"/> from the states
        /// </summary>
        /// <param name="key">The key of the State to remove</param>
        public void RemoveState(string key)
        {
            if(!HasState(key)) return;
            
            _states.Remove(GetState(key));
        }
    
        /// <summary>
        /// Sets the value of a State with the same key as <paramref name="state"/> to the value of <paramref name="state"/>
        /// </summary>
        /// <param name="state">The State containing the new value</param>
        public void SetState(State state)
        {
            if (HasState(state.Key))
            {
                GetState(state.Key).SetValue(state.Value);
            }
            else
            {
                _states.Add(new State(state));
            }
        }
        
        /// <summary>
        /// Sets the value of a State with <paramref name="key"/> to <paramref name="value"/>
        /// </summary>
        /// <param name="key">The key of the State to set a new value</param>
        /// <param name="value">The new value to set to the State</param>
        public void SetState(string key, int value)
        {
            if (HasState(key))
            {
                GetState(key).SetValue(value);
            }
            else
            {
                _states.Add(new State(key, value));
            }
        }
        
        /// <summary>
        /// Fetches the State with <paramref name="key"/> in the states
        /// </summary>
        /// <param name="key">The key of the State to fetch</param>
        /// <returns>The State with <paramref name="key"/> if found in the states, null otherwise</returns>
        public State GetState(string key)
        {
            return _states.Find(s => s.Key == key);
        }
        
        /// <summary>
        /// Computes the union of this StateSet and <paramref name="stateSet"/>
        /// </summary>
        /// <param name="stateSet">The StateSet to combine with this StateSet</param>
        /// <returns>The union of this StateSet and <paramref name="stateSet"/></returns>
        public StateSet Union(StateSet stateSet)
        {
            var union = new StateSet(this);
            foreach (var state in stateSet._states)
            {
                union.AddState(state);
            }

            return union;
        }
        
        // Properties
        
        public IEnumerable<State> States => _states;
    }
}
