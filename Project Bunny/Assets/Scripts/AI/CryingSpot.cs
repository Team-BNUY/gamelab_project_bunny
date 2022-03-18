using AI.Agents;
using UnityEngine;

namespace AI
{
    public class CryingSpot : MonoBehaviour
    {
        private bool _occupied;

        private void OnTriggerEnter(Collider other)
        {
            var student = other.GetComponent<Student>();
            if (student)
                _occupied = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            var student = other.GetComponent<Student>();
            if (student)
                _occupied = false;
        }

        // Properties

        public bool Occupied
        {
            get => _occupied;
        }
    }
}
