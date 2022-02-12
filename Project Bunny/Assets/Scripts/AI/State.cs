using System;
using UnityEngine;

namespace AI
{
    [Serializable]
    public class State
    {
        [SerializeField] 
        private string key;
        [SerializeField]
        private int value;
        
        /// <summary>
        /// Constructor that initializes the State's key and value to <paramref name="key"/> and <paramref name="value"/> respectively
        /// </summary>
        /// <param name="key">The key of the State</param>
        /// <param name="value">The value of the State</param>
        public State(string key, int value)
        {
            this.key = key;
            this.value = value;
        }
        
        /// <summary>
        /// Copy constructor that initializes the state's key and value to the <paramref name="state"/>'s key and value respectively
        /// </summary>
        /// <param name="state">The State to copy the key and value</param>
        public State(State state)
        {
            key = state.key;
            value = state.value;
        }
        
        /// <summary>
        /// Adds an <paramref name="addedValue"/> to the State's value
        /// </summary>
        /// <param name="addedValue">The value to add to the State's value</param>
        public void AddValue(int addedValue)
        {
            value += addedValue;
        }
        
        /// <summary>
        /// Sets a <paramref name="newValue"/> to the State's value
        /// </summary>
        /// <param name="newValue">The new value for the State</param>
        public void SetValue(int newValue)
        {
            value = newValue;
        }
        
        // Properties
        public string Key => key;
        public int Value => value;
    }
}
