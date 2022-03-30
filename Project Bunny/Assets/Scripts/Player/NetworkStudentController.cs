using Cinemachine;
using Interfaces;
using Networking;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Image = UnityEngine.UI.Image;

namespace Player
{
    [SelectionBase]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class NetworkStudentController : MonoBehaviourPunCallbacks, IPunObservable
    {
        private const float DEATH_TIME_DELAY = 3f;

        [Header("Components")]
        [SerializeField] private Transform _studentTransform;
        [SerializeField] private Transform _playerModel;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Animator _animator;
        [SerializeField] private SkinnedMeshRenderer _jersey;
        private Camera _playerCamera;
        private CinemachineVirtualCamera _playerVCam;
        private CharacterController _characterController;
        private CinemachineFramingTransposer _playerVCamFramingTransposer;

        [Header("Movement")]
        [SerializeField] [Min(0)] private float _movementSpeed;
        [SerializeField] [Range(0f, 1f)] private float _movementFriction;
        private Vector3 _playerCurrentVelocity;
        private Vector3 _playerPosition;
        private Quaternion _playerRotation;
        private Vector2 _inputMovement;
        private float _mousePosAngle;

        [Header("Player Properties")] 
        [SerializeField] private Canvas _worldUI;
        [SerializeField] private RectTransform _canvasTransform;
        [SerializeField] private Image[] _hearts;
        [SerializeField] private int _maxHealth;
        private int _currentHealth;
        private GameObject _currentObjectInHand;
        private INetworkInteractable _currentInteractable;
        private INetworkTriggerable _currentTriggerable;
        private bool _isSliding;

        [Header("Snowball")]
        [SerializeField] private Transform _playerHand;
        [SerializeField] [Min(0)] private float _digSnowballMaxTime;
        [SerializeField] private float _minForce;
        [SerializeField] private float _maxForce;
        [SerializeField] [Range(0f, 2.0f)] private float _forceIncreaseTimeRate;
        private NetworkSnowball _playerSnowball;
        private int _currentStandingGround;
        private float _throwForce;
        private float _digSnowballTimer;
        private bool _isAiming;

        [Header("Booleans")]
        private bool _isWalking;
        private bool _isDigging;
        private bool _hasSnowball;
        private bool _isDead;
        // List of readonly files. No need for them to have a _ prefix
        private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
        private static readonly int IsDiggingHash = Animator.StringToHash("isDigging");
        private static readonly int HasSnowballHash = Animator.StringToHash("hasSnowball");
        private static readonly int ThrowSnowball = Animator.StringToHash("ThrowSnowball");

        // PROPERTIES (REPLACE PUBLIC GETTERS)
        public CharacterController CharacterControllerComponent => _characterController;
        public Quaternion PlayerRotation => _playerRotation;
        public Transform PlayerHand => _playerHand;
        public bool HasSnowball => _hasSnowball;
        public bool IsDigging => _isDigging;
        public bool IsDead => _isDead;
        public CinemachineFramingTransposer PlayerVCamFramingTransposer => _playerVCamFramingTransposer;

        [Header("Network")]
        [SerializeField] private TMPro.TMP_Text _nickNameText;
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string PlayerID { get; set; }
        public byte TeamID { get; set; }
        private bool _isJerseyNull;


        #region Callbacks

        private void Awake()
        {
            _studentTransform ??= transform;
            _characterController ??= GetComponent<CharacterController>();
            _animator ??= gameObject.GetComponent<Animator>();
            _playerInput ??= GetComponent<PlayerInput>();

            _playerInput.actionEvents[0].AddListener(OnMove);
            _playerInput.actionEvents[1].AddListener(OnLook);
        }

        private void Start()
        {
            _isJerseyNull = _jersey == null;
            _throwForce = _minForce;
            _currentHealth = _maxHealth;
            SetNameText();
            UpdateTeamColorVisuals();
            PhotonTeamsManager.PlayerJoinedTeam += OnPlayerJoinedTeam;
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(SyncPlayerInfo), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer.GetPhotonTeam()?.Code);
            }
        }

        private void Update()
        {
            if (!photonView.IsMine || _isDead) return;

            SetStandingGround();

            if (!_isDigging)
            {
                MoveStudent();
            }

            if (_isAiming && _hasSnowball)
            {
                if (_throwForce <= _maxForce)
                {
                    IncreaseThrowForce();
                }
                _playerSnowball.DrawTrajectory();
            }

            DigSnowball();
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.TryGetComponent<NetworkGiantRollball>(out var giantRollball))
            {
                if (!_hasSnowball && !giantRollball.CanDamage)
                {
                    giantRollball.PushGiantRollball(transform);
                }
            }
        }

        private void OnPlayerJoinedTeam(Photon.Realtime.Player player, PhotonTeam team)
        {
            photonView.RPC("UpdateTeamColorVisuals", RpcTarget.AllBuffered);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out INetworkTriggerable triggerable))
            {
                if (_currentTriggerable == null)
                {
                    _currentTriggerable = triggerable;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out INetworkTriggerable triggerable))
            {
                if (_currentTriggerable == triggerable)
                {
                    _currentTriggerable = null;
                }
            }
        }
        #endregion

        #region Actions


        /// <summary>
        /// Change player's position and orientation in global axes using Character Controller
        /// </summary>
        private void MoveStudent()
        {
            // If the player's character controller is disabled, then don't move them. Otherwise, move them. 
            if (_characterController.enabled)
            {
                if (_isSliding)
                {
                    var desiredVelocity = _playerPosition * _movementSpeed;
                    _playerCurrentVelocity = Vector3.Lerp(_playerCurrentVelocity, desiredVelocity, _movementFriction * Time.deltaTime * 0.5f);
                    _characterController.Move(_playerCurrentVelocity * Time.deltaTime);
                }
                else
                {
                    _characterController.Move(_playerPosition * (_movementSpeed * Time.deltaTime));
                }
            }
            
            _playerModel.rotation = _playerRotation;

            var signedAngle = Vector2.SignedAngle(new Vector2(0, 1), _inputMovement);
            Debug.Log(signedAngle);
        }

        /// <summary>
        /// Activates Digging Stopwatch and creates snowball on hand once digging complete
        /// </summary>
        private void DigSnowball()
        {
            if (!photonView.IsMine) return;

            if (_isDigging && !_hasSnowball)
            {
                _digSnowballTimer += Time.deltaTime;
            }

            if (_digSnowballTimer < _digSnowballMaxTime) return;

            var prefabToSpawn = ArenaManager.Instance.SnowballPrefab.name;
            if (_currentStandingGround == LayerMask.NameToLayer("Ice"))
            {
                prefabToSpawn = ArenaManager.Instance.IceballPrefab.name;
            }
            _currentObjectInHand = PhotonNetwork.Instantiate(prefabToSpawn, _playerHand.position, _playerHand.rotation * Quaternion.Euler(0f, -90f, 0f));
            //_currentObjectInHand.transform.parent = _playerHand;
            // TODO: Object pooling to avoid using GetComponent at Instantiation
            _playerSnowball = _currentObjectInHand.GetComponent<NetworkSnowball>();
            _playerSnowball.SetSnowballThrower(this);
            _playerSnowball.SetHoldingPlace(transform);
            _hasSnowball = true;
            _isDigging = false;
            _digSnowballTimer = 0.0f;
            _animator.SetBool(IsDiggingHash, false);
            _animator.SetBool(HasSnowballHash, true);
        }

        /// <summary>
        /// Increase throw force every frame when aiming snowball
        /// </summary>
        private void IncreaseThrowForce()
        {
            _throwForce += Time.deltaTime * _forceIncreaseTimeRate;
            _playerSnowball.SetSnowballForce(_throwForce);
        }

        /// <summary>
        /// Throws snowball once activated
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void ThrowStudentSnowball()
        {
            if (!photonView.IsMine || _playerSnowball == null) return;

            _playerSnowball.DisableLineRenderer();
            _isAiming = false;
            _throwForce = _minForce;
            _playerSnowball.ThrowSnowball();
            _hasSnowball = false;
            _currentObjectInHand = null;
            _playerSnowball = null;
            _animator.SetBool(HasSnowballHash, false);
        }

        public void GetDamaged(int damage) 
        {
            photonView.RPC(nameof(GetDamagedRPC), RpcTarget.AllBuffered, damage);
        }


        /// <summary>
        /// Student gets damaged when snowball or combat item is thrown at them successfully
        /// </summary>
        /// <param name="damage"></param>
        [PunRPC]
        // ReSharper disable once UnusedMember.Global
        public void GetDamagedRPC(int damage)
        {
            if (damage >= _currentHealth)
            {
                _currentHealth = 0;
            }
            else
            {
                _currentHealth -= damage;
            }
            SetHeartsVisibility();

            if (_currentHealth <= 0)
            {
                _isDead = true;
                _characterController.enabled = false;
                _worldUI.gameObject.SetActive(false);
                Instantiate(ArenaManager.Instance.SnowmanPrefab, _studentTransform.position, _studentTransform.rotation);

                if (photonView.IsMine)
                {
                    ScoreManager.Instance.IncrementTeamDeaths(TeamID);
                    Invoke(nameof(Respawn), DEATH_TIME_DELAY);
                }
            }
        }

        private void Respawn()
        {
            _worldUI.gameObject.SetActive(true);
            _currentHealth = _maxHealth;
            _studentTransform.position = ArenaManager.Instance.GetPlayerSpawnPoint(this);
            _characterController.enabled = true;
            _isDead = false;
            photonView.RPC(nameof(SetHeartsVisibilityRPC), RpcTarget.All);
        }
        
        private void SetHeartsVisibility()
        {
            for (var i = 1; i <= _maxHealth; i++)
            {
                _hearts[i-1].color = i <= _currentHealth ? Color.white : new Color(1f, 1f, 1f, 0.5f);
            }
        }
        
        [PunRPC]
        private void SetHeartsVisibilityRPC()
        {
            for (var i = 1; i <= _maxHealth; i++)
            {
                _hearts[i-1].color = i <= _currentHealth ? Color.white : new Color(1f, 1f, 1f, 0.5f);
            }
        }

        #endregion

        #region InputSystem

        /// <summary>
        /// DO NOT CHANGE NAME: Translates 2D Vector input action into position coordinates in world space
        /// </summary>
        /// <param name="context"></param>
        // ReSharper disable once UnusedMember.Global
        public void OnMove(InputAction.CallbackContext context)
        {
            _inputMovement = context.ReadValue<Vector2>();
            var gravity = Physics.gravity.y * Time.deltaTime * 100f;
            _playerPosition = new Vector3(_inputMovement.x, 0f, _inputMovement.y);
            _playerPosition.y += gravity;
            _isWalking = _animator.GetBool(IsWalkingHash);

            //_animator.SetBool(_hasSnowballHash, _hasSnowball);
            if (photonView.IsMine && _isWalking && _inputMovement.magnitude == 0.0f)
            {
                _animator.SetBool(IsWalkingHash, false);
            }
            else if (photonView.IsMine && !_isWalking && _inputMovement.magnitude > 0.0f)
            {
                _animator.SetBool(IsWalkingHash, true);
            }
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Translate mouse 2D Vector input action into angular rotation
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void OnLook(InputAction.CallbackContext context)
        {
            if (_playerCamera == null) return;

            _mousePosAngle = Utilities.MousePosToRotationInput(_studentTransform, _playerCamera);
            _playerRotation = Quaternion.Euler(0f, _mousePosAngle, 0f);
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Activates the Right Mouse Button
        /// </summary>
        /// <param name="context"></param>
        // ReSharper disable once UnusedMember.Global
        public void OnDig(InputAction.CallbackContext context)
        {
            //If the player is currently interacting with an interactable, don't dig
            if (_currentInteractable != null) return;

            // If player has snowball on hand or character is in the air, don't dig
            if (_hasSnowball || !_characterController.isGrounded) return;

            // Don't allow player to dig while on ice and sliding fast
            if (_currentStandingGround == LayerMask.NameToLayer("Ice") &&
                _characterController.velocity.magnitude >= 2.0f) return;

            //  Can't dig at surface with no ice or snow
            if (_currentStandingGround != LayerMask.NameToLayer("Ground") &&
                _currentStandingGround != LayerMask.NameToLayer("Ice")) return;

            // If action is being performed, start digging
            if (context.performed)
            {
                _isDigging = true;
                if (photonView.IsMine)
                {
                    _animator.SetBool(IsWalkingHash, false);
                    _animator.SetBool(IsDiggingHash, true);
                }
            }
            // If digging is abruptly cancelled, cancel action and reset timer
            if (context.canceled)
            {
                _isDigging = false;
                _digSnowballTimer = 0.0f;
                if (photonView.IsMine)
                {
                    _animator.SetBool(IsDiggingHash, false);
                }
            }

            _playerCurrentVelocity = Vector3.zero;
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Activates the Left Mouse Button
        /// </summary>
        /// <param name="context"></param>
        // ReSharper disable once UnusedMember.Global
        public void OnThrow(InputAction.CallbackContext context)
        {
            // If the action is performed, do something
            if (context.performed)
            {
                //If player has a snowball, then start aiming it. Otherwise call the Interactable's Click method.
                if (_hasSnowball)
                {
                    _isAiming = true;
                }
                else
                {
                    _currentInteractable?.Click();
                }
            }

            // If the action is cancelled, do something
            if (context.canceled)
            {
                //If player has a snowball, then throw it. Otherwise call the Interactable's Release method.
                if (photonView.IsMine && _hasSnowball)
                {
                    PlaySnowballThrowAnimation();
                }
                else
                {
                    _currentInteractable?.Release();
                }
            }
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Activates the Interaction key, currently E
        /// </summary>
        /// <param name="context"></param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!photonView.IsMine) return;


                _currentTriggerable?.Trigger(this);


                if (_hasSnowball) return; //Don't interact with interactables if the player already has a snowball. 

                // TODO: Interact with other items here
                // When you press 'E', it checks to see if there is an
                // interactable nearby and if there is, assume control of it. 

                if (_currentInteractable == null)
                {
                    _currentInteractable = ReturnNearestInteractable();
                    _currentInteractable?.Enter(this);
                }
                else
                {
                    _currentInteractable?.Exit();
                    _currentInteractable = null;
                }
            }
        }

        #endregion

        #region RPC

        [PunRPC]
        private void SyncPlayerInfo(string playerID, byte TeamID)
        {
            this.PlayerID = playerID;
            this.TeamID = TeamID;
        }

        //[PunRPC]
        // ReSharper disable once UnusedMember.Local
        private void PlaySnowballThrowAnimation()
        {
            _animator.SetTrigger(ThrowSnowball);
        }

        /// <summary>
        /// Temporary functionality for updating visuals like mesh object and name text colors
        /// Functionality will still be kept for later, but more refined
        /// </summary>
        [PunRPC]
        public void UpdateTeamColorVisuals()
        {
            if (_isJerseyNull)
            {
                Debug.LogError("Missing reference to player jersey");
                return;
            }
            object teamId;
            PhotonTeam team;
            if (photonView.Owner.CustomProperties.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamId) &&
                PhotonTeamsManager.Instance.TryGetTeamByCode((byte)teamId, out team))
            {
                switch (team.Code)
                {
                    case 1:
                        _jersey.material.color = Color.blue;
                        _nickNameText.color = Color.blue;
                        break;
                    case 2:
                        _jersey.material.color = Color.red;
                        _nickNameText.color = Color.red;
                        break;
                }
            }
        }


        /// <summary>
        /// Tmeporary function for restoring a player's colors to all white to show they are teamless
        /// </summary>
        [PunRPC]
        public void RestoreTeamlessColors()
        {
            _nickNameText.color = Color.white;
        }

        public void RestoreTeamlessColors_RPC()
        {
            photonView.RPC("RestoreTeamlessColors", RpcTarget.AllBuffered);
        }

        public void SetNameText()
        {
            _nickNameText.text = photonView.Owner.NickName;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Track the layer of the object the player is standing on
        /// </summary>
        private void SetStandingGround()
        {
            if (Physics.Raycast(_studentTransform.position, _studentTransform.TransformDirection(Vector3.down), out var hit, 1.0f))
            {
                Debug.DrawRay(_studentTransform.position, _studentTransform.TransformDirection(Vector3.down) * hit.distance, Color.red);
                _currentStandingGround = hit.collider.gameObject.layer;
                if (!_isSliding)
                {
                    _playerCurrentVelocity = _characterController.velocity;
                }
                _isSliding = _currentStandingGround == LayerMask.NameToLayer("Ice");
            }
        }

        /// <summary>
        /// Attach unique instantiated camera with player
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        // ReSharper disable once UnusedMember.Global
        public void SetCamera(GameObject cam, float angle, float distance)
        {
            _playerVCam = cam.GetComponent<CinemachineVirtualCamera>();
            _playerCamera = cam.GetComponentInChildren<Camera>();
            _playerVCam.Follow = _studentTransform;
            _playerVCamFramingTransposer = _playerVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            SetFrameTransposerProperties(angle, distance);
        }

        /// <summary>
        /// Set VCam's FrameTransposer properties
        /// Put it in separate function to modify at any time
        /// If you want to tweak more properties, add more arguments to this function
        /// and the SetCamera(...) above
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        private void SetFrameTransposerProperties(float angle, float distance)
        {
            _playerVCamFramingTransposer.m_CameraDistance = distance;
            _playerVCam.transform.rotation = Quaternion.Euler(angle, 0f, 0f);
            _canvasTransform.rotation = Quaternion.Euler(angle, 0f, 0f);
        }

        /// <summary>
        /// Utility function that returns the nearest interactable to the player
        /// </summary>
        /// <returns></returns>
        private INetworkInteractable ReturnNearestInteractable()
        {
            INetworkInteractable interactable = null;
            var maxColliders = 3; //maximum number of objects near to the player that can be looped through
            var hitColliders = new Collider[maxColliders];
            var numColliders = Physics.OverlapSphereNonAlloc(_studentTransform.position, 1f, hitColliders);

            if (numColliders < 1) return null;

            //Loop through 3 nearest objects and check if any of them are interactables that implement IInteractable
            for (var i = 0; i < numColliders; i++)
            {
                if (hitColliders[i].gameObject.TryGetComponent<INetworkInteractable>(out var interactableObject))
                {
                    interactable = interactableObject;
                }
            }

            return interactable;
        }

        /// <summary>
        /// Accessed through animation event. Disables Walking animation when necessary
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void SetWalkingAnimator()
        {
            if (photonView.IsMine)
            {
                _animator.SetBool(IsWalkingHash, _isWalking);
            }
        }

        /// <summary>
        /// This is a very important method, it basically is entirely responsible for syncing the object on the network.
        /// If (stream.IsWriting) == true, it means that we own the player, so transmit data to everyone else on the network (hence, write)
        /// Else, we read information based on the current position and rotation.
        /// Note, that position and rotation are already transmitted because of the network transform.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_hasSnowball);
                stream.SendNext(_isDigging);
                stream.SendNext(_isWalking);
                stream.SendNext(_isAiming);
                stream.SendNext(_currentHealth);
                stream.SendNext(_isDead);
                stream.SendNext(_worldUI.gameObject.activeSelf);
                stream.SendNext(_canvasTransform.rotation);
            }
            else
            {
                _hasSnowball = (bool) stream.ReceiveNext();
                _isDigging = (bool) stream.ReceiveNext();
                _isWalking = (bool) stream.ReceiveNext();
                _isAiming = (bool) stream.ReceiveNext();
                _currentHealth = (int) stream.ReceiveNext();
                _isDead = (bool) stream.ReceiveNext();
                _worldUI.gameObject.SetActive((bool) stream.ReceiveNext());
                _canvasTransform.rotation = (Quaternion) stream.ReceiveNext();
            }
        }

        #endregion
    }
}