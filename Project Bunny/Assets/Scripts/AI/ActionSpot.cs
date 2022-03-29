using System.Collections.Generic;
using AI.Agents;
using UnityEngine;

namespace AI
{
    public class ActionSpot : MonoBehaviour
    {
        [SerializeField] private ActionSpotType _type;
        [SerializeField] private Transform _interactiveObject;

        private readonly List<Student> _occupyingStudents = new List<Student>();
        private bool _occupied;

        private void OnTriggerEnter(Collider other)
        {
            var student = other.GetComponent<Student>();
            if (_occupyingStudents.Contains(student)) return;
            
            _occupyingStudents.Add(student);
        }
        
        private void OnTriggerExit(Collider other)
        {
            var student = other.GetComponent<Student>();
            if (!_occupyingStudents.Contains(student)) return;
            
            _occupyingStudents.Remove(student);
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
            get => _occupied || _occupyingStudents.Count != 0;
        }
    }
    
    public enum ActionSpotType { Cry, Pole, Sit }
}
