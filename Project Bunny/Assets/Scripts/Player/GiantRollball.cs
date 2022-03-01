using System;
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

        // ReSharper disable once NotAccessedField.Local
        // TODO: Implement damage system once game loop is complete
        [SerializeField] private float _damage;
        [SerializeField] private float _growthFactor;

        private void Awake()
        {
            if (_snowballRigidbody == null)
            {
                _snowballRigidbody = gameObject.GetComponent<Rigidbody>();
            }
        }

        private void FixedUpdate()
        {
            SelfPush();
            GrowSize();
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
    }
}
