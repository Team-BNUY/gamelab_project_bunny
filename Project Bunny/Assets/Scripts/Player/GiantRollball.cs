using UnityEngine;

namespace Player
{
    public class GiantRollball : MonoBehaviour
    {
        [SerializeField] private Rigidbody _snowballRigidbody;
        [SerializeField] private Transform _snowballTransform;
        [SerializeField] private ParticleSystem _snowballBurst;
    
        // ReSharper disable once NotAccessedField.Local
        // TODO: Implement damage system once game loop is complete
        [SerializeField] private float _damage;

        private float _throwForce;
        private float _mass;
        
        // ReSharper disable once NotAccessedField.Local
        private StudentController _studentThrower;
        
        private void Awake()
        {
            if (_snowballRigidbody == null)
            {
                _snowballRigidbody = gameObject.GetComponent<Rigidbody>();
            }

            _mass = _snowballRigidbody.mass;
        }
    }
}
