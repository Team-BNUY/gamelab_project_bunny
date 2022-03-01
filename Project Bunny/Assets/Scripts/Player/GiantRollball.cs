using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class GiantRollball : MonoBehaviour
    {
        [SerializeField] private Rigidbody _snowballRigidbody;
        [SerializeField] private Transform _snowballTransform;
        // ReSharper disable once NotAccessedField.Local
        [SerializeField] private ParticleSystem _snowballBurst;
        [SerializeField] private LayerMask _hitLayers;
        [SerializeField, Min(0.0f)] private float _destroySpeedThreshold;
        [SerializeField, Min(0.0f)] private float _damageThreshold;

        // ReSharper disable once NotAccessedField.Local
        // TODO: Implement damage system once game loop is complete
        [SerializeField] private float _damage;
        [SerializeField] private float _growthFactor;

        private bool _isDestroyable;
        private bool _canDamage;

        private void Awake()
        {
            if (_snowballRigidbody == null)
            {
                _snowballRigidbody = gameObject.GetComponent<Rigidbody>();
            }
        }

        private void Update()
        {
            TrackGiantRollballStates();
        }

        private void FixedUpdate()
        {
            SelfPush();
            GrowSize();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (IsInLayerMask(other.gameObject)  && _isDestroyable)
            {
                // Damage student 
                if (other.gameObject.TryGetComponent<StudentController>(out var otherStudent) && _canDamage)
                {
                    otherStudent.GetDamaged(_damage);
                }
                var go = Instantiate(_snowballBurst.gameObject, transform.position, Quaternion.identity);
                go.GetComponent<ParticleSystem>().Play();
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Let student push the giant snowball
        /// </summary>
        /// <param name="student"></param>
        public void PushGiantRollball(Transform student)
        {
            var distance = _snowballTransform.position - student.position;
            distance = distance.normalized;
            _snowballRigidbody.AddForce(distance * 20f);
        }

        /// <summary>
        /// Additional force to the giant snowball for better physics
        /// </summary>
        private void SelfPush()
        {
            _snowballRigidbody.AddForce(0f, -500f, 0f);
        }

        /// <summary>
        /// Grow giant snowball while it rolls
        /// </summary>
        private void GrowSize()
        {
            _snowballTransform.localScale += Vector3.one * (_growthFactor * _snowballRigidbody.velocity.magnitude * Time.fixedDeltaTime);
        }

        private void TrackGiantRollballStates()
        {
            var currentSpeed= _snowballRigidbody.velocity.magnitude;
            _isDestroyable = currentSpeed >= _destroySpeedThreshold;
            _canDamage = currentSpeed >= _damageThreshold;
        }

        /// <summary>
        /// Check if collider object is part of the breakable layers
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool IsInLayerMask(GameObject obj)
        {
            return (_hitLayers.value & (1 << obj.layer)) > 0;
        }
    }
}
