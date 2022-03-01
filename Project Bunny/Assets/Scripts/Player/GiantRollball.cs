using System;
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
        [SerializeField] private float _growthFactor;

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

        private void FixedUpdate()
        {
            SelfPush();
            GrowSize();
        }

        public void PushGiantRollball(Transform student)
        {
            var distance = transform.position - student.position;
            distance = distance.normalized;
            _snowballRigidbody.AddForce(distance * 20f);
        }

        private void SelfPush()
        {
            _snowballRigidbody.AddForce(0f, -500f, 0f);
        }

        public void GrowSize()
        {
            transform.localScale += Vector3.one * (_growthFactor * _snowballRigidbody.velocity.magnitude * Time.fixedDeltaTime);
        }
    }
}
