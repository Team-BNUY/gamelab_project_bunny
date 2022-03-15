using UnityEngine;

namespace AI
{
    public class CryingSpot : MonoBehaviour
    {
        private bool _occupied;
        
        // Properties

        public bool Occupied
        {
            get => _occupied;
            set => _occupied = value;
        }
    }
}
