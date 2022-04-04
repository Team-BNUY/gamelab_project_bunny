using System;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkGiantRollball : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] private Rigidbody _snowballRigidbody;
        [SerializeField] private Transform _snowballTransform;
        [SerializeField] private LayerMask _hitLayers;
        [SerializeField, Min(0.0f)] private float _destroySpeedThreshold;
        [SerializeField, Min(0.0f)] private float _damageThreshold;
        [SerializeField, Min(0.0f)] private float _sizeThreshold;
        [SerializeField, Min(0.0f)] private float _pushForce;

        // ReSharper disable once NotAccessedField.Local
        // TODO: Implement damage system once game loop is complete
        [SerializeField] private int _damage;
        [SerializeField] private float _growthFactor;

        private int _spawnIndex;
        private bool _isGrowing;
        private bool _isDestroyable;
        private bool _canDamage;
        private bool _hasCollided;
        public bool CanDamage => _canDamage;
        //public int Damage => _damage;

        private void Awake()
        {
            _snowballRigidbody ??= gameObject.GetComponent<Rigidbody>();
            _snowballTransform ??= transform;
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

        private void OnCollisionStay(Collision other)
        {
            _isGrowing = other.gameObject.layer == LayerMask.NameToLayer("Ground");

            if (other.gameObject.TryGetComponent<NetworkStudentController>(out var student) && _canDamage)
            {
                if (_hasCollided || !PhotonNetwork.IsMasterClient) return;
                _hasCollided = true;


                if (student.photonView.IsMine)
                {
                    ScoreManager.Instance.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, ScoreManager.AVALANCHE_KEY);
                }

                student.photonView.RPC("GetDamagedRPC", RpcTarget.All, _damage);
                BreakRollball();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || IsInLayerMask(other.gameObject) && _isDestroyable)
            {
                BreakRollball();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_canDamage)
            {
                PushGiantRollball(other.transform);
            }
        }

        public void InitializeGiantRollball(int index)
        {
            _spawnIndex = index;
        }

        /// <summary>
        /// Let student push the giant snowball
        /// </summary>
        /// <param name="pusherTransform"></param>
        private void PushGiantRollball(Transform pusherTransform)
        {
            var distance = _snowballTransform.position - pusherTransform.position;
            distance = distance.normalized;
            _snowballRigidbody.AddForce(distance * _pushForce);
        }

        /// <summary>
        /// Additional force to the giant snowball for better physics
        /// </summary>
        private void SelfPush()
        {
            _snowballRigidbody.AddForce(0f, -100f, 0f);
        }

        /// <summary>
        /// Grow giant snowball while it rolls
        /// </summary>
        private void GrowSize()
        {
            if (_snowballTransform.localScale.x >= _sizeThreshold)
            {
                BreakRollball();
            }
            
            if (_isGrowing)
            {
                _snowballTransform.localScale += Vector3.one * (_growthFactor * _snowballRigidbody.velocity.magnitude * Time.fixedDeltaTime);
            }
        }
        
        /// <summary>
        /// Track the Giant Snowball's properties
        /// </summary>
        private void TrackGiantRollballStates()
        {
            var velocity = _snowballRigidbody.velocity;
            if (!_isDestroyable && velocity.magnitude >= _destroySpeedThreshold)
            {
                _isDestroyable = true;
            }
            _canDamage = velocity.magnitude >= _damageThreshold;
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
        
        private void BreakRollball()
        {
            var go = Instantiate(ArenaManager.Instance.GiantRollballBurst, transform.position, Quaternion.identity);
            go.GetComponent<ParticleSystem>().Play();
            if (PhotonNetwork.IsMasterClient)
            {
                ArenaManager.Instance.RemoveGiantRollball(_spawnIndex);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_spawnIndex);
                stream.SendNext(_canDamage);
                stream.SendNext(_isDestroyable);
                stream.SendNext(_isGrowing);
                stream.SendNext(_hasCollided);
            }
            else
            {
                _spawnIndex = (int) stream.ReceiveNext();
                _canDamage = (bool) stream.ReceiveNext();
                _isDestroyable = (bool) stream.ReceiveNext();
                _isGrowing = (bool) stream.ReceiveNext();
                _hasCollided = (bool) stream.ReceiveNext();
            }
        }
    }
}
