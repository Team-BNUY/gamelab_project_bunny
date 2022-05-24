using System.Linq;
using Arena;
using UnityEngine;
using Cinemachine;
using Interfaces;
using Photon.Pun;
using Random = UnityEngine.Random;

namespace Player
{
    public class NetworkRollBallThrow : MonoBehaviourPunCallbacks, INetworkInteractable, IPunObservable, INetworkTriggerable
    {
        [Header("Components")]
        [SerializeField] private Animator _hoverButton;
        [SerializeField] private Transform _playerSeat;
        [SerializeField] private Transform _rollBallSeat;
        [SerializeField] private GameObject _aimArrow;

        [Header("Properties")] [SerializeField] private int _id;
        [SerializeField] private float _newCameraDistance;
        [SerializeField] [Min(0)] private float _rotationSpeed;
        private GameObject _player;
        private NetworkStudentController _currentStudentController;
        private CinemachineFramingTransposer _playerVCamSettings;
        private CharacterController _playerCharController;
        private Vector3 _previousMouseToWorldDirection;

        [Header("Snowball")]
        [SerializeField] [Min(0)] private float _coolDownTime;
        private float _cooldownTimer;
        private bool _ready;

        public bool IsActive { get; private set; }

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

            if (!IsActive || !_currentStudentController || !_currentStudentController.photonView.IsMine) return;

            RotateRollBallAiming();
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(IsActive);
                stream.SendNext(_ready);
            }
            else
            {
                IsActive = (bool) stream.ReceiveNext();
                _ready = (bool) stream.ReceiveNext();
            }
        }

        #endregion

        #region InterfaceMethods

        /// <summary>
        /// Slingshot's implementation of the Enter() method. Initializes key variables.
        /// </summary>
        public void Enter(NetworkStudentController currentStudentController)
        {
            if (!_ready)
            {
                currentStudentController.CurrentInteractable = null;
                return;
            }
            
            if (IsActive || currentStudentController.HasSnowball) return;
            
            _hoverButton.enabled = false;
            _hoverButton.gameObject.SetActive(false);
            
            //Initialize key variables
            photonView.RPC(nameof(SetActive), RpcTarget.All, true, currentStudentController.PlayerID);
            photonView.TransferOwnership(_currentStudentController.photonView.Owner);
            _currentStudentController.IsUsingCannon = true;
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
            _currentStudentController.Animator.applyRootMotion = false;
            _currentStudentController.Animator.transform.localPosition = Vector3.zero;
            _currentStudentController.SetAnimatorParameter("InteractIdle", false);
            _currentStudentController.SetAnimatorParameter("UsingInteractable", false);

            // If already aiming while exiting, then just throw the current snowball and restore everything
            _cooldownTimer = 0.0f;

            //Restore key variables to null/default value
            _player = null;
            _currentStudentController.IsUsingCannon = false;
            photonView.RPC(nameof(SetActive), RpcTarget.All, false, default(string));

            //Restoring the original camera distance of the player's camera when quitting control of Slingshot.
            if (_playerVCamSettings)
            {
                _playerVCamSettings.m_CameraDistance = 25;
                _playerVCamSettings = null;
            }

            if (_playerCharController)
            {
                _playerCharController.enabled = true;
                _playerCharController = null;
            }
        }

        /// <summary>
        /// Slingshot's implementation of the Click() method
        /// </summary>
        public void Click()
        {
            if (!IsActive || !_currentStudentController || !_ready) return;
            
            if (_currentStudentController.photonView.IsMine)
            {
                _aimArrow.SetActive(false);
            }
            
            _hoverButton.enabled = false;
            _hoverButton.gameObject.SetActive(false);
            
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
        
        public void TriggerableTrigger(NetworkStudentController currentStudentController)
        {
            
        }

        public void TriggerableEnter()
        {
            if (IsActive || !_ready) return;
            
            _hoverButton.enabled = true;
            _hoverButton.gameObject.SetActive(true);
            _hoverButton.Play("EInteract");
        }

        public void TriggerableExit()
        {
            _hoverButton.enabled = false;
            _hoverButton.gameObject.SetActive(false);
        }

        #endregion

        #region CannonBallLogic

        public void PushRollBall()
        {
            ThrowSnowball();
            _player = null;
            _currentStudentController.CurrentInteractable = null;
            _currentStudentController.IsUsingCannon = false;
            _currentStudentController.Animator.applyRootMotion = false;
            _currentStudentController.Animator.transform.localPosition = Vector3.zero;
            photonView.RPC(nameof(SetActive), RpcTarget.All, false, default(string));
            
            _cooldownTimer = 0f;

            //Restoring the original camera distance of the player's camera when quitting control of Slingshot.
            if (_playerVCamSettings)
            {
                _playerVCamSettings.m_CameraDistance = 25;
                _playerVCamSettings = null;
            }

            //Restore key variables to null/default value
            if (_playerCharController)
            {
                _playerCharController.enabled = true;
                _playerCharController = null;
            }
        }
        
        /// <summary>
        /// Start the snowball timer and spawn a new snowball when the timer is up
        /// </summary>
        private void CannonRollBallUpdate()
        {
            if (_ready || IsActive || !PhotonNetwork.IsMasterClient) return;

            //If there is no cannonball currently loaded, then increase the timer. 
            _cooldownTimer += Time.deltaTime;

            //If the cooldown timer is up, then spawn a new cannonball. If not, return. 
            if (_cooldownTimer < _coolDownTime) return;
            
            SpawnRollBall();
        }

        /// <summary>
        /// Rotate the Slingshot with variable speed towards the mouse cursor.
        /// </summary>
        private void RotateRollBallAiming()
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
            var currentRollBall = PhotonNetwork.Instantiate(ArenaManager.Instance.GiantRollballPrefab.name, _rollBallSeat.position, _rollBallSeat.rotation);
            currentRollBall.GetComponent<NetworkGiantRollball>().ID = _id;
            photonView.RPC(nameof(ReadyUp), RpcTarget.All, true);
            _cooldownTimer = 0f;
        }

        /// <summary>
        /// Throw the snowball and restore related variables to default values.
        /// </summary>
        private void ThrowSnowball()
        {
            if (_ready)
            {
                var playerRollBall = FindObjectsOfType<NetworkGiantRollball>().FirstOrDefault(b => b.ID == _id);
                if (playerRollBall)
                {
                    _cooldownTimer = 0f;
                    playerRollBall.photonView.TransferOwnership(_currentStudentController.photonView.Owner);
                    playerRollBall.PushGiantRollball(_currentStudentController);
                }
            }
            
            photonView.RPC(nameof(ReadyUp), RpcTarget.All, false);
        }

        #endregion

        #region RPCs

        [PunRPC]
        public void SetActive(bool value, string userId)
        {
            IsActive = value;
            _currentStudentController = ArenaManager.Instance.AllPlayers.FirstOrDefault(x => x.PlayerID == userId);
        }
        
        [PunRPC]
        public void ReadyUp(bool value)
        {
            _ready = value;
        }

        #endregion
    }
}