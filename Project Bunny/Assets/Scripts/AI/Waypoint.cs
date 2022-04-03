using System.Collections.Generic;
using AI.Agents;
using UnityEngine;

namespace AI
{
    public class Waypoint : MonoBehaviour
    {
        private readonly List<Student> _occupyingStudents = new List<Student>();

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

        // Properties
        
        public bool Occupied
        {
            get => _occupyingStudents.Count != 0;
        }
    }
}
