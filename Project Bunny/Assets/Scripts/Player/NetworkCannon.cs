using System.Collections.Generic;
using System.Linq;
using Arena;
using UnityEngine;
using Cinemachine;
using Interfaces;
using Networking;
using Photon.Pun;

namespace Player
{
    public class NetworkCannon : MonoBehaviourPunCallbacks, INetworkInteractable, IPunObservable, INetworkTriggerable
    {
        [Header("Components")]
        [SerializeField] private Animator _hoverButton;
        [SerializeField] private Transform _playerSeat;
        [SerializeField] private Transform _cannonBallSeat;
        [SerializeField] private GameObject _aimArrow;
        [SerializeField] private Transform _bone;

        [Header("Properties")]
        [SerializeField] private float _newCameraDistance;
        [SerializeField] [Min(0)] private float _rotationSpeed;
        [SerializeField] [Range(0f, 2.0f)] private float _pullIncreaseTimeRate;
        [SerializeField] [Min(0)] private float _pullingFactor = 0.75f;
        private GameObject _player;
        private CinemachineFramingTransposer _playerVCamSettings;
        private CharacterController _playerCharController;
        private Vector3 _initialBonePosition;
        private Vector3 _initialSnowballSeatPosition;
        private Vector3 _previousMouseToWorldDirection;

        [Header("Snowball")]
        [SerializeField] [Min(1)] private int _numberOfSnowballs;
        [SerializeField] [Min(0)] private float _cooldownTime;
        [SerializeField] private float _minForce, _maxForce;
        [SerializeField] [Range(0f, 2.0f)] private float _forceIncreaseTimeRate;
        [SerializeField] private float _minAngle, _maxAngle;
        private readonly List<NetworkSnowball> _cannonballCollection = new List<NetworkSnowball>();
        private float _throwForce;
        private bool _hasSnowball, _isAiming;
        private float _coolDownTimer;

        [Header("Audio")]
        [SerializeField] private AudioClip[] _shootSounds;
        [SerializeField] private AudioClip _reloadSound;
        private AudioSource _audioSource;
        
        public bool IsActive { get; private set; }
        public NetworkStudentController CurrentStudentController { get; private set; }
        public Transform CannonBallSeat => _cannonBallSeat;
        
        #region Callbacks

        private void Awake()
        {
            photonView.OwnershipTransfer = OwnershipOption.Takeover;
            _audioSource ??= GetComponent<AudioSource>();
        }

        private void Start()
        {
            _throwForce = _minForce;
            _initialBonePosition = _bone.transform.localPosition;
            _initialSnowballSeatPosition = _cannonBallSeat.transform.localPosition;
        }

        private void Update()
        {
            if (!IsActive || !CurrentStudentController || !CurrentStudentController.photonView.IsMine) return;

            RotateSlingShot();
            CannonBallUpdate();

            if (!_hasSnowball || !_isAiming) return;

            if (_throwForce < _maxForce)
            {
                IncreaseThrowForce();
            }
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(IsActive);
            }
            else
            {
                IsActive = (bool) stream.ReceiveNext();
            }
        }

        #endregion

        #region InterfaceMethods

        /// <summary>
        /// Slingshot's implementation of the Enter() method. Initializes key variables.
        /// </summary>
        public void Enter(NetworkStudentController currentStudentController)
        {
            if (IsActive)
            {
                currentStudentController.CurrentInteractable = null;
                return;
            }
            
            _hoverButton.enabled = false;
            _hoverButton.gameObject.SetActive(false);
            
            //Initialize key variables
            photonView.RPC(nameof(SetActive), RpcTarget.All, true, currentStudentController.PlayerID);
            photonView.TransferOwnership(CurrentStudentController.photonView.Owner);
            CurrentStudentController.IsUsingCannon = true;
            _player = CurrentStudentController.transform.gameObject;
            
            // Idle animation
            CurrentStudentController.SetAnimatorParameter("InteractIdle", true);
            CurrentStudentController.SetAnimatorParameter("UsingInteractable", true);

            //Setting the new distance of the player camera when assuming control of the Slingshot
            _playerVCamSettings = CurrentStudentController.PlayerVCamFramingTransposer;
            _playerVCamSettings.m_CameraDistance = _newCameraDistance;

            //Disable player controller in order to set the player's position manually
            _playerCharController = CurrentStudentController.CharacterControllerComponent;
            _playerCharController.enabled = false;
            
            if (CurrentStudentController.photonView.IsMine)
            {
                _aimArrow.SetActive(true);
            }

            //If there is no cannonball already on the slingshot, then spawn one. 
            if (_hasSnowball && _coolDownTimer >= _cooldownTime)
            {
                SpawnCannonBall();
            }
        }

        /// <summary>
        /// Slingshot's implementation of the Exit() method. Restores variables to default/null.
        /// </summary>
        public void Exit()
        {
            if (CurrentStudentController.photonView.IsMine)
            {
                _aimArrow.SetActive(false);
            }
            
            _hoverButton.enabled = true;
            _hoverButton.gameObject.SetActive(true);
            _hoverButton.Play("EInteract");

            // Idle animation
            CurrentStudentController.SetAnimatorParameter("InteractIdle", false);
            CurrentStudentController.SetAnimatorParameter("UsingInteractable", false);

            // If already aiming while exiting, then just throw the current snowball and restore everything
            if (_isAiming)
            {
                ThrowSnowball();
            }
            else
            {
                _cannonballCollection.ForEach(b => b.DestroySnowball());
                _cannonballCollection.Clear();
                _bone.transform.localPosition = _initialBonePosition;
                _cannonBallSeat.transform.localPosition = _initialSnowballSeatPosition;
            }
            
            _coolDownTimer = 0.0f;

            //Restore key variables to null/default value
            _player = null;
            CurrentStudentController.IsUsingCannon = false;
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
            if (!_hasSnowball) return;
            
            PlaySound(_reloadSound);
            CurrentStudentController.SetAnimatorParameter("Pulling", true);
            _isAiming = true;
        }

        /// <summary>
        /// Slingshot's implementation of the Release() method
        /// </summary>
        public void Release()
        {
            if (!_hasSnowball || !_isAiming) return;
            
            CurrentStudentController.SetAnimatorParameter("Pulling", false);
            ThrowSnowball();
        }

        #endregion

        #region CannonBallLogic

        /// <summary>
        /// Start the snowball timer and spawn a new snowball when the timer is up
        /// </summary>
        private void CannonBallUpdate()
        {
            //If there is no cannonball currently loaded, then increase the timer. 
            if (!_hasSnowball)
            {
                _coolDownTimer += Time.deltaTime;
            }

            //If the cooldown timer is up, then spawn a new cannonball. If not, return. 
            if (_coolDownTimer < _cooldownTime) return;

            SpawnCannonBall();
        }

        /// <summary>
        /// Rotate the Slingshot with variable speed towards the mouse cursor.
        /// </summary>
        private void RotateSlingShot()
        {
            var finalRotation = CurrentStudentController.PlayerRotation * Quaternion.Euler(0f, 90f, 0f);

            var mousePosition = CurrentStudentController.MousePosition;
            var playerCamera = CurrentStudentController.PlayerCamera;
            var cameraTransform = playerCamera.transform;
            var cameraPosition = cameraTransform.position;
            Physics.Raycast(cameraPosition, cameraTransform.forward, out var hitInfo, float.PositiveInfinity, LayerMask.GetMask("Ground"));
            var distanceToGround = Vector3.Distance(hitInfo.point, cameraPosition);
            var mouseToWorldPosition = playerCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distanceToGround));
            var playerModel = CurrentStudentController.PlayerModel;
            var playerPosition = playerModel.position;
            var targetDirection = mouseToWorldPosition - playerPosition;

            if (_previousMouseToWorldDirection == Vector3.zero)
            {
                _previousMouseToWorldDirection = targetDirection;
            }
            
            var angle = Vector3.SignedAngle(_previousMouseToWorldDirection, targetDirection, Vector3.up);

            _previousMouseToWorldDirection = Vector3.Slerp(_previousMouseToWorldDirection, targetDirection, Time.deltaTime * 8f);
            
            CurrentStudentController.SetAnimatorParameter("RotationDirection", angle);
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, Time.deltaTime * _rotationSpeed);
            _player.transform.position = _playerSeat.position;
            CurrentStudentController.SetPlayerRotation(_playerSeat.rotation * Quaternion.Euler(0f, -90f, 0f));
        }

        /// <summary>
        /// Spawn a new cannonball and set values for relevant variables
        /// </summary>
        private void SpawnCannonBall()
        {
            for (var i = 0; i < _numberOfSnowballs; i++)
            {
                var cannonBall = PhotonNetwork.Instantiate(ArenaManager.Instance.CannonBall.name, _cannonBallSeat.position, _cannonBallSeat.rotation);
                var snowball = cannonBall.GetComponent<NetworkSnowball>();
                snowball.photonView.RPC("SetParent", RpcTarget.All, true);
                snowball.SetRandomCannonPlacement();
                _cannonballCollection.Add(snowball);
            }
            
            _cannonballCollection.ForEach(c => c.SetSnowballThrower(CurrentStudentController));
            _hasSnowball = true;
            _coolDownTimer = 0f;
        }

        /// <summary>
        /// When aiming, increase the throw force of the cannonball
        /// </summary>
        private void IncreaseThrowForce()
        {
            var boneTransform = _bone.transform;
            var position = boneTransform.localPosition;
            var nextOnAxis = position.y - Time.deltaTime * _pullIncreaseTimeRate;
            var newPosition = new Vector3(position.x, nextOnAxis, position.z);
            boneTransform.localPosition = newPosition;

            var seatTransform = _cannonBallSeat.transform;
            position = seatTransform.localPosition;
            nextOnAxis = position.x - Time.deltaTime * _pullIncreaseTimeRate * _pullingFactor;
            newPosition = new Vector3(nextOnAxis, position.y, position.z);
            seatTransform.localPosition = newPosition;
            
            _throwForce += Time.deltaTime * _forceIncreaseTimeRate;
            _aimArrow.transform.localScale += new Vector3(0f, 0f, Time.deltaTime * _forceIncreaseTimeRate);
            _cannonballCollection.ForEach(c => c.SetSnowballForce(_throwForce));
        }

        /// <summary>
        /// Throw the snowball and restore related variables to default values.
        /// </summary>
        private void ThrowSnowball()
        {
            _isAiming = false;
            _throwForce = _minForce;
            _hasSnowball = false;

            if (_cannonballCollection.Count != 0)
            {
                foreach (var cannonBall in _cannonballCollection)
                {
                    cannonBall.SetHoldingPlace(_cannonBallSeat);
                    cannonBall.photonView.RPC("SetParent", RpcTarget.All, false);
                    cannonBall.ThrowBurstSnowballs(_minForce, _maxForce, _minAngle, _maxAngle);
                    
                    _audioSource.Stop();
                    var random = Random.Range(0, _shootSounds.Length);
                    PlaySoundOneShot(_shootSounds[random]);
                }

                _bone.transform.localPosition = _initialBonePosition;
                _cannonBallSeat.transform.localPosition = _initialSnowballSeatPosition;
            }

            _aimArrow.transform.localScale = Vector3.one;
            _cannonballCollection.Clear();
        }

        #endregion

        #region Triggerable

        public void TriggerableTrigger(NetworkStudentController currentStudentController)
        {
            
        }

        public void TriggerableEnter()
        {
            if (IsActive) return;
            
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
        
        #region Utilities

        private void PlaySoundOneShot(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip, AudioManager.Instance.Volume * 2f);
        }

        private void PlaySound(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.volume = AudioManager.Instance.Volume * 2f;
            _audioSource.Play();
        }

        #endregion

        #region RPCs

        [PunRPC]
        private void SetActive(bool value, string userId)
        {
            IsActive = value;
            CurrentStudentController = ArenaManager.Instance.AllPlayers.FirstOrDefault(x => x.PlayerID == userId);
        }

        #endregion
    }
}