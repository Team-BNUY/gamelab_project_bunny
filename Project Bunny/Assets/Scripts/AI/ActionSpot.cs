using System.Collections.Generic;
using System.Linq;
using AI.Agents;
using UnityEngine;

namespace AI
{
    public class ActionSpot : MonoBehaviour
    {
        [SerializeField] private ActionSpotType _type;
        [SerializeField] private Transform _interactiveObject;
        
        private readonly List<GameObject> _occupyingObjects = new List<GameObject>();
        private bool _occupied;
        
        private void OnTriggerEnter(Collider other)
        {
            var o = other.gameObject;
            if (_occupyingObjects.Contains(o)) return;
            
            _occupyingObjects.Add(o);
        }
        
        private void OnTriggerExit(Collider other)
        {
            var o = other.gameObject;
            if (!_occupyingObjects.Contains(o)) return;
            
            _occupyingObjects.Remove(o);
        }

        public void Use()
        {
            _occupied = true;
        }

        public void Free()
        {
            _occupied = false;
        }

        // Properties

        public ActionSpotType Type
        {
            get => _type;
        }

        public Transform InteractiveObject
        {
            get => _interactiveObject;
        }
        
        public bool Occupied
        {
            get => _occupied || _occupyingObjects.Any(o => o.GetComponent<Student>());
        }
    }
    
    public enum ActionSpotType { Cry, Pole }
}
