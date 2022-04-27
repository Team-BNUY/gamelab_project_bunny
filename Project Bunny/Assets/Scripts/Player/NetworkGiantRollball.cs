using System.Linq;
using Arena;
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
        [SerializeField, Min(0.0f)] private float _sizeThreshold;
        [SerializeField, Min(0.0f)] private float _pushForce;

        [SerializeField] private int _damage;
        [SerializeField] private float _growthFactor;
        [SerializeField] private AudioClip _rollSound;
        [SerializeField] private AudioClip _destroySound;
        [SerializeField] private float _volumeIncreaseTimeRate = 0.1f;
        private AudioSource _audioSource;
        
        private NetworkStudentController _pusher;
        private bool _isGrowing;
        private bool _canDamage;
        private bool _hasCollided;
        public int _id;
        
        public bool CanDamage => _canDamage;

        public int ID
        {
            get => _id;
            set => _id = value;
        }
        
        private void Awake()
        {
            photonView.OwnershipTransfer = OwnershipOption.Takeover;
            
            _snowballRigidbody ??= gameObject.GetComponent<Rigidbody>();
            _snowballTransform ??= transform;
            _audioSource ??= GetComponent<AudioSource>();
        }

        private void FixedUpdate()
        {
            SelfPush();
            GrowSize();
        }

        private void OnCollisionStay(Collision other)
        {
            _isGrowing = other.gameObject.layer == LayerMask.NameToLayer("Ground");

            if (!_canDamage) return;
            
            if (other.gameObject.TryGetComponent<NetworkStudentController>(out var player))
            {
                if (_hasCollided || _pusher && player == _pusher && _pusher.IsKicking) return;
                _hasCollided = true;

                if (player.photonView.IsMine)
                {
                    ScoreManager.Instance.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, ScoreManager.AVALANCHE_KEY);
                }

                player.GetDamaged(_damage);
                BreakRollball();
            }
            else if (IsInLayerMask(other.gameObject))
            {
                BreakRollball();
                
                // Play destroy sound
                AudioManager.Instance.PlayClipAt(_destroySound, transform.position);
            }
        }

        /// <summary>
        /// Let student push the giant snowball
        /// </summary>
        /// <param name="pusher">The player pushing the roll ball</param>
        public void PushGiantRollball(NetworkStudentController pusher)
        {
            // Play the kick sound
            _audioSource.clip = _rollSound;
            _audioSource.volume = 0.0f;
            _audioSource.Play();
            pusher.PlayKickAudio();
            
            // Allow to damage and prevent self damage on initial kick
            photonView.RPC(nameof(SetPusher), RpcTarget.All, pusher.PlayerID);
            Invoke(nameof(StopKicking), 0.5f);

            // Add force to the roll ball
            var distance = _snowballTransform.position - pusher.transform.position;
            distance = distance.normalized;
            _snowballRigidbody.AddForce(distance * _pushForce, ForceMode.Impulse);
        }

        [PunRPC]
        public void SetPusher(string playerID)
        {
            _pusher = ArenaManager.Instance.AllPlayers.FirstOrDefault(x => x.PlayerID == playerID);
            _snowballRigidbody.isKinematic = false;
            _canDamage = true;
        }

        private void StopKicking()
        {
            _pusher.IsKicking = false;
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

            if (!_isGrowing) return;
            
            _audioSource.volume += Time.deltaTime * _volumeIncreaseTimeRate;
            _snowballTransform.localScale += Vector3.one * (_growthFactor * _snowballRigidbody.velocity.magnitude * Time.fixedDeltaTime);
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
        
        /// <summary>
        /// Destroy the roll ball
        /// </summary>
        private void BreakRollball()
        {
            // Play the particle effect
            var go = Instantiate(ArenaManager.Instance.GiantRollballBurst, transform.position, Quaternion.identity);
            go.GetComponent<ParticleSystem>().Play();

            // Destroy the roll ball
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_canDamage);
                stream.SendNext(_isGrowing);
                stream.SendNext(_hasCollided);
                stream.SendNext(_id);
            }
            else
            {
                _canDamage = (bool) stream.ReceiveNext();
                _isGrowing = (bool) stream.ReceiveNext();
                _hasCollided = (bool) stream.ReceiveNext();
                _id = (int) stream.ReceiveNext();
            }
        }
    }
}
