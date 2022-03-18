using System.Collections.Generic;
using System.Linq;
using AI.Agents;
using UnityEngine;

namespace AI
{
    public class CryingSpot : MonoBehaviour
    {
        private List<GameObject> occupyingObjects = new List<GameObject>();

        private void OnTriggerEnter(Collider other)
        {
            var o = other.gameObject;
            if (occupyingObjects.Contains(o)) return;
            
            occupyingObjects.Add(o);
        }
        
        private void OnTriggerExit(Collider other)
        {
            var o = other.gameObject;
            if (!occupyingObjects.Contains(o)) return;
            
            occupyingObjects.Remove(o);
        }

        // Properties

        public bool Occupied
        {
            get => occupyingObjects.Any(o => o.GetComponent<Student>());
        }
    }
}
