using System;
using UnityEngine;

namespace AI
{
    [Serializable]
    public class State
    {
        [SerializeField] private string _key;
        [SerializeField] private int _value;
        
        /// <summary>
        /// Constructor that initializes the State's key and value to <paramref name="key"/> and <paramref name="value"/> respectively
        /// </summary>
        /// <param name="key">The key of the State</param>
        /// <param name="value">The value of the State</param>
        public State(string key, int value)
        {
            _key = key;
            _value = value;
        }
        
        /// <summary>
        /// Copy constructor that initializes the state's key and value to the <paramref name="state"/>'s key and value respectively
        /// </summary>
        /// <param name="state">The State to copy the key and value</param>
        public State(State state)
        {
            _key = state._key;
            _value = state._value;
        }
        
        /// <summary>
        /// Adds an <paramref name="addedValue"/> to the State's value
        /// </summary>
        /// <param name="addedValue">The value to add to the State's value</param>
        public void AddValue(int addedValue)
        {
            _value += addedValue;
        }
        
        /// <summary>
        /// Sets a <paramref name="newValue"/> to the State's value
        /// </summary>
        /// <param name="newValue">The new value for the State</param>
        public void SetValue(int newValue)
        {
            _value = newValue;
        }
        
        // Properties
        
        public string Key => _key;
        public int Value => _value;
    }
}
