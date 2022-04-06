using System.Linq;
using UnityEngine;
using Cinemachine;
using Interfaces;
using Photon.Pun;
using Random = UnityEngine.Random;

namespace Player
{
    public class NetworkRollballThrow : MonoBehaviourPunCallbacks, INetworkInteractable, IPunObservable
    {
        [Header("Components")]
        [SerializeField] private Transform _playerSeat;
        [SerializeField] private Transform _rollballSeat;
        [SerializeField] private GameObject _aimArrow;

        [Header("Properties")] [SerializeField] private int _id;
        [SerializeField] private float _newCameraDistance;
        [SerializeField] [Min(0)] private float _rotationSpeed;
        private bool _isActive;
        private GameObject _player;
        private NetworkStudentController _currentStudentController;
        private CinemachineFramingTransposer _playerVCamSettings;
        private CharacterController _playerCharController;
        private Vector3 _previousMouseToWorldDirection;

        [Header("Snowball")]
        [SerializeField] [Min(0)] private float _coolDownTime;
        private float _cooldownTimer;
        private bool _ready;

        public bool IsActive => _isActive;

        #region Callbacks

        private void Awake()
        {
            photonView.OwnershipTransfer = OwnershipOption.Takeover;
        }

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            SpawnRollBall();
        }
        
        private void Update()
        {
            CannonRollBallUpdate();

            if (!_isActive || !_currentStudentController || !_currentStudentController.photonView.IsMine) return;

            RotateRollballAiming();
        }

        #endregion

        #region InterfaceMethods

        /// <summary>
        /// Slingshot's implementation of the Enter() method. Initializes key variables.
        /// </summary>
        public void Enter(NetworkStudentController currentStudentController)
        {
            if (!_ready || currentStudentController.HasSnowball) return;
            
            //Initialize key variables
            photonView.RPC(nameof(SetActive), RpcTarget.All, true, currentStudentController.PlayerID);
            photonView.TransferOwnership(_currentStudentController.photonView.Owner);
            _currentStudentController.UsingCannon = true;
            _player = _currentStudentController.transform.gameObject;

            // Idle animation
            _currentStudentController.SetAnimatorParameter("InteractIdle", true);
            _currentStudentController.SetAnimatorParameter("UsingInteractable", true);
            
            //Setting the new distance of the player camera when assuming control of the Slingshot
            _playerVCamSettings = _currentStudentController.PlayerVCamFramingTransposer;
            _playerVCamSettings.m_CameraDistance = _newCameraDistance;

            //Disable player controller in order to set the player's position manually
            _playerCharController = _currentStudentController.CharacterControllerComponent;
            _playerCharController.enabled = false;
            
            if (_currentStudentController.photonView.IsMine)
            {
                _aimArrow.SetActive(true);
            }
        }

        /// <summary>
        /// Slingshot's implementation of the Exit() method. Restores variables to default/null.
        /// </summary>
        public void Exit()
        {
            if (!_currentStudentController) return;
            
            if (_currentStudentController.photonView.IsMine)
            {
                _aimArrow.SetActive(false);
            }
            
            // Idle animation
            _currentStudentController.SetAnimatorParameter("InteractIdle", false);
            _currentStudentController.SetAnimatorParameter("UsingInteractable", false);

            // If already aiming while exiting, then just throw the current snowball and restore everything
            _cooldownTimer = 0.0f;

            //Restore key variables to null/default value
            _player = null;
            _currentStudentController.UsingCannon = false;
            photonView.RPC(nameof(SetActive), RpcTarget.All, false, default(string));


            //Restoring the original camera distance of the player's camera when quitting control of Slingshot.
            _playerVCamSettings.m_CameraDistance = 25;

            //Restore key variables to null/default value
            _playerVCamSettings = null;
            _playerCharController.enabled = true;
            _playerCharController = null;
        }

        /// <summary>
        /// Slingshot's implementation of the Click() method
        /// </summary>
        public void Click()
        {
            if (!_isActive || !_currentStudentController || !_ready) return;
            
            if (_currentStudentController.photonView.IsMine)
            {
                _aimArrow.SetActive(false);
            }

            _currentStudentController.IsKicking = true;
                
            // Idle animation
            _currentStudentController.SetAnimatorParameter("InteractIdle", false);
            _currentStudentController.SetAnimatorParameter("UsingInteractable", false);

            var random = Random.Range(0, 3);
            _currentStudentController.SetAnimatorParameter("Random", random);
            _currentStudentController.SetAnimatorParameter("Kick");
        }

        /// <summary>
        /// Slingshot's implementation of the Release() method
        /// </summary>
        public void Release()
        {

        }

        #endregion

        #region CannonBallLogic

        public void PushRollball()
        {
            ThrowSnowball();
            _player = null;
            _currentStudentController.CurrentInteractable = null;
            _currentStudentController.UsingCannon = false;
            photonView.RPC(nameof(SetActive), RpcTarget.All, false, default(string));
                
            //Restoring the original camera distance of the player's camera when quitting control of Slingshot.
            _playerVCamSettings.m_CameraDistance = 25;

            //Restore key variables to null/default value
            _playerVCamSettings = null;
            _playerCharController.enabled = true;
            _playerCharController = null;
            _cooldownTimer = 0f;
        }
        
        /// <summary>
        /// Start the snowball timer and spawn a new snowball when the timer is up
        /// </summary>
        private void CannonRollBallUpdate()
        {
            if (_ready || !PhotonNetwork.IsMasterClient) return;
            
            //If there is no cannonball currently loaded, then increase the timer. 
            _cooldownTimer += Time.deltaTime;

            //If the cooldown timer is up, then spawn a new cannonball. If not, return. 
            if (_cooldownTimer < _coolDownTime) return;
            
            SpawnRollBall();
        }

        /// <summary>
        /// Rotate the Slingshot with variable speed towards the mouse cursor.
        /// </summary>
        private void RotateRollballAiming()
        {
            var finalRotation = _currentStudentController.PlayerRotation * Quaternion.Euler(0f, 90f, 0f);
            
            var mousePosition = _currentStudentController.MousePosition;
            var playerCamera = _currentStudentController.PlayerCamera;
            var cameraTransform = playerCamera.transform;
            var cameraPosition = cameraTransform.position;
            Physics.Raycast(cameraPosition, cameraTransform.forward, out var hitInfo, float.PositiveInfinity, LayerMask.GetMask("Ground"));
            var distanceToGround = Vector3.Distance(hitInfo.point, cameraPosition);
            var mouseToWorldPosition = playerCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distanceToGround));
            var playerModel = _currentStudentController.PlayerModel;
            var playerPosition = playerModel.position;
            var targetDirection = mouseToWorldPosition - playerPosition;

            if (_previousMouseToWorldDirection == Vector3.zero)
            {
                _previousMouseToWorldDirection = targetDirection;
            }
            
            var angle = Vector3.SignedAngle(_previousMouseToWorldDirection, targetDirection, Vector3.up);

            _previousMouseToWorldDirection = Vector3.Slerp(_previousMouseToWorldDirection, targetDirection, Time.deltaTime * 8f);
            
            _currentStudentController.SetAnimatorParameter("RotationDirection", angle);
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, Time.deltaTime * _rotationSpeed);
            _player.transform.position = _playerSeat.position;
            _currentStudentController.SetPlayerRotation(_playerSeat.rotation * Quaternion.Euler(0f, -90f, 0f));
        }

        /// <summary>
        /// Spawn a new cannonball and set values for relevant variables
        /// </summary>
        private void SpawnRollBall()
        {
            var currentRollball = PhotonNetwork.Instantiate(ArenaManager.Instance.GiantRollballPrefab.name, _rollballSeat.position, _rollballSeat.rotation);
            currentRollball.GetComponent<NetworkGiantRollball>().ID = _id;
            _ready = true;
        }

        /// <summary>
        /// Throw the snowball and restore related variables to default values.
        /// </summary>
        private void ThrowSnowball()
        {
            if (_ready)
            {
                var playerRollball = FindObjectsOfType<NetworkGiantRollball>().FirstOrDefault(b => b.ID == _id);
                if (playerRollball)
                {
                    playerRollball.photonView.TransferOwnership(_currentStudentController.photonView.Owner);
                    playerRollball.PushGiantRollball(_currentStudentController);
                }
            }
            
            _ready = false;
        }

        #endregion
        
        [PunRPC]
        public void SetActive(bool value, string userId)
        {
            _isActive = value;
            _currentStudentController = ArenaManager.Instance.AllPlayers.FirstOrDefault(x => x.PlayerID == userId);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_isActive);
                stream.SendNext(_ready);
            }
            else
            {
                _isActive = (bool) stream.ReceiveNext();
                _ready = (bool) stream.ReceiveNext();
            }
        }
    }
}