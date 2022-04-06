using System.Linq;
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

        // ReSharper disable once NotAccessedField.Local
        // TODO: Implement damage system once game loop is complete
        [SerializeField] private int _damage;
        [SerializeField] private float _growthFactor;

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
        }

        private void FixedUpdate()
        {
            SelfPush();
            GrowSize();
        }

        private void OnCollisionStay(Collision other)
        {
            _isGrowing = other.gameObject.layer == LayerMask.NameToLayer("Ground");

            if (_canDamage) return;
            
            if (other.gameObject.TryGetComponent<NetworkStudentController>(out var player))
            {
                if (_hasCollided || _pusher && player == _pusher && _pusher.IsKicking) return;
                _hasCollided = true;

                if (player.photonView.IsMine)
                {
                    ScoreManager.Instance.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, ScoreManager.AVALANCHE_KEY);
                }

                player.photonView.RPC("GetDamagedRPC", RpcTarget.All, _damage);
                BreakRollball();
            }
            else if (IsInLayerMask(other.gameObject))
            {
                BreakRollball();
            }
        }

        /// <summary>
        /// Let student push the giant snowball
        /// </summary>
        /// <param name="pusherTransform"></param>
        public void PushGiantRollball(NetworkStudentController pusher)
        {
            photonView.RPC(nameof(SetPusher), RpcTarget.All, pusher.PlayerID);
            Invoke(nameof(StopKicking), 0.5f);
            
            photonView.RPC(nameof(DisableIsKinematic), RpcTarget.All);
            _canDamage = true;

            var distance = _snowballTransform.position - pusher.transform.position;
            distance = distance.normalized;
            _snowballRigidbody.AddForce(distance * _pushForce, ForceMode.Impulse);
        }

        [PunRPC]
        public void SetPusher(string playerID)
        {
            _pusher = ArenaManager.Instance.AllPlayers.FirstOrDefault(x => x.PlayerID == playerID);
        }

        [PunRPC]
        public void DisableIsKinematic()
        {
            _snowballRigidbody.isKinematic = false;
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
            
            if (_isGrowing)
            {
                _snowballTransform.localScale += Vector3.one * (_growthFactor * _snowballRigidbody.velocity.magnitude * Time.fixedDeltaTime);
            }
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
