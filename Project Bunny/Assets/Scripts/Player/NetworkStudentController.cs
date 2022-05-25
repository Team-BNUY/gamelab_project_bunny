using System.Collections;
using Arena;
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
        // General components
        [Header("Components")]
        [SerializeField] private Transform _playerModel;
        [SerializeField] private Collider _playerCollider;
        [SerializeField] private Transform _playerHand;
        private Transform _playerTransform;
        private PlayerInput _playerInput;

        // Camera
        private CinemachineVirtualCamera _playerVCam;

        // Gameplay parameters
        [Header("Gameplay Parameters")]
        [SerializeField] private float _deathTimeDelay = 3f;

        private bool _isSliding;

        // Movement parameters
        [Header("Movement Parameters")]
        [SerializeField] [Min(0)] private float _movementSpeed;
        [SerializeField] [Range(0f, 1f)] private float _movementFriction;
        private Vector3 _playerCurrentVelocity;
        private Vector3 _playerPosition;
        private Vector2 _inputMovement;
        private Vector2 _deltaVector;
        private Vector3 _target = Vector3.zero;
        private bool _startGame;
        private bool _isWalking;
        
        // Snowball throw parameters
        [Header("Throw Parameters")]
        [SerializeField] [Min(0)] private float _digSnowballMaxTime;
        [SerializeField] private float _minForce;
        [SerializeField] private float _maxForce;
        [SerializeField] [Range(0f, 2.0f)] private float _forceIncreaseTimeRate;
        [SerializeField] private float _minAngle;
        [SerializeField] private float _maxAngle;
        [SerializeField] [Range(0f, 8.0f)] private float _angleIncreaseTimeRate;
        private NetworkSnowball _playerSnowball;
        private int _currentStandingGround;
        private float _throwForce;
        private float _throwAngle;
        private float _digSnowballTimer;
        private bool _isAiming;
        private bool _threwSnowball;
        private bool _isDigging;

        // UI
        [Header("UI")]
        [SerializeField] private Canvas _worldUI;
        [SerializeField] private RectTransform _canvasTransform;
        [SerializeField] private RectTransform _healthTransform;
        [SerializeField] private RectTransform _nicknameTransform;
        [SerializeField] private GameObject _hostCrown;
        [SerializeField] public GameObject _isReadyBubble;
        [SerializeField] private Image[] _hearts;
        [SerializeField] private SpriteRenderer _digSnowballRadialBar;
        [SerializeField] private int _maxHealth;
        private int _currentHealth;
        private GameObject _currentObjectInHand;
        private INetworkTriggerable _currentTriggerable;
        private float _diggingBarDecrement;
        
        // Audio
        [Header("Audio")] 
        [SerializeField] private AudioClip _snowballThrowSound;
        [SerializeField] private AudioClip _hitBySnowballSound;
        [SerializeField] private AudioClip _hitByIceballSound;
        [SerializeField] private AudioClip _hitByRollballSound;
        [SerializeField] private AudioClip _kickSound;
        [SerializeField] private AudioClip _deathSound;
        private AudioSource _audioSource;

        // Readonly animator parameter hashes
        private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
        private static readonly int IsDiggingHash = Animator.StringToHash("isDigging");
        private static readonly int HasSnowballHash = Animator.StringToHash("hasSnowball");
        private static readonly int ThrowSnowball = Animator.StringToHash("ThrowSnowball");
        private static readonly int DeltaX = Animator.StringToHash("deltaX");
        private static readonly int DeltaY = Animator.StringToHash("deltaY");
        private static readonly int PrepareThrow = Animator.StringToHash("PrepareThrow");
        private static readonly int CancelPrepare = Animator.StringToHash("CancelPrepare");
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int HitFront = Animator.StringToHash("HitFront");
        private static readonly int HitBack = Animator.StringToHash("HitBack");
        private static readonly int HitLeft = Animator.StringToHash("HitLeft");
        private static readonly int HitRight = Animator.StringToHash("HitRight");
        private static readonly int Arc2 = Shader.PropertyToID("_Arc2");
        private static readonly int Property = Shader.PropertyToID("Arc Point 2");

        #region Properties

        public CharacterController CharacterControllerComponent { get; private set; }
        public Quaternion PlayerRotation { get; private set; }
        public Transform PlayerHand => _playerHand;
        public bool HasSnowball { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsInFollowZone { get; private set; }
        public bool IsBeingControlled { get; private set; }
        public float CameraLockX { get; private set; }
        public bool IsInClass { get; private set; }
        public CinemachineFramingTransposer PlayerVCamFramingTransposer { get; private set; }
        public Collider PlayerCollider => _playerCollider;
        public Camera PlayerCamera { get; private set; }
        public Transform PlayerModel => _playerModel;
        public PlayerCustomization PlayerCustomization { get; private set; }
        public Vector2 MousePosition { get; private set; }
        public Animator Animator { get; private set; }
        public string PlayerID { get; set; }
        public byte TeamID { get; set; }
        public INetworkInteractable CurrentInteractable { get; set; }
        public bool IsKicking { get; set; }
        public bool IsUsingCannon { get; set; }
        public bool IsFrozen { get; set; }

        #endregion

        #region Callbacks

        private void Awake()
        {
            _playerTransform = transform;
            CharacterControllerComponent = GetComponent<CharacterController>();
            Animator = _playerModel.GetComponentInChildren<Animator>();
            _playerInput = GetComponent<PlayerInput>();
            PlayerCustomization = GetComponent<PlayerCustomization>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            // UI
            _isReadyBubble.SetActive(false);

            // Parameters
            _throwForce = _minForce;
            _throwAngle = _minAngle;
            _currentHealth = _maxHealth;
            IsBeingControlled = false;
            _target = Vector3.zero;
            _startGame = false;
            _diggingBarDecrement = 360f / (_digSnowballMaxTime * 60f);
            
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(SyncPlayerInfo), RpcTarget.AllBuffered, PlayerID, TeamID);
            }
        }

        private void Update()
        {
            if (!photonView.IsMine || IsDead) return;

            SetStandingGround();

            if (!_isDigging && !IsFrozen)
            {
                if (!IsBeingControlled)
                {
                    MoveStudent();
                }
                else if (_target != Vector3.zero)
                {
                    MoveToPoint();
                }
            }

            if (_isAiming && HasSnowball && !IsFrozen)
            {
                if (!_threwSnowball)
                {
                    if (_throwForce <= _maxForce)
                    {
                        IncreaseThrowForce();
                    }

                    if (_throwAngle <= _maxAngle)
                    {
                        IncreaseThrowAngle();
                    }
                }

                _playerSnowball.DrawTrajectory();
            }

            DigSnowball();
        }
        
        protected override void OnEnable()
        {
            PhotonTeamsManager.PlayerJoinedTeam += OnPlayerJoinedTeam;
            _playerInput.actionEvents[0].AddListener(OnMove);
            _playerInput.actionEvents[1].AddListener(OnLook);
            
            base.OnEnable();
        }
        
        protected override void OnDisable()
        {
            PhotonTeamsManager.PlayerJoinedTeam -= OnPlayerJoinedTeam;
            _playerInput.actionEvents[0].RemoveListener(OnMove);
            _playerInput.actionEvents[1].RemoveListener(OnLook);
            
            base.OnDisable();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var rollBall = collision.gameObject.GetComponent<NetworkGiantRollball>();
            if (rollBall && rollBall.CanDamage)
            {
                PlayHitAudio(2);
            }
            
            var snowball = collision.gameObject.GetComponent<NetworkSnowball>();
            if (!snowball) return;

            // Hit by a snowball
            var thrower = snowball.StudentThrower;
            var throwDirection = thrower.transform.position - _playerModel.position;
            var angle = Vector3.SignedAngle(_playerModel.forward, throwDirection, Vector3.up);

            if (angle < 0 && angle >= -45f || angle >= 0 && angle < 45f)
            {
                photonView.RPC(nameof(SetBoolRPC), RpcTarget.All, "HitFront", true);
            }
            else if (angle < -45f && angle >= -135f)
            {
                photonView.RPC(nameof(SetBoolRPC), RpcTarget.All, "HitLeft", true);
            }
            else if (angle >= 45f && angle < 135f)
            {
                photonView.RPC(nameof(SetBoolRPC), RpcTarget.All, "HitRight", true);
            }
            else
            {
                photonView.RPC(nameof(SetBoolRPC), RpcTarget.All, "HitBack", true);
            }

            photonView.RPC(nameof(SetBoolRPC), RpcTarget.All, "Hit", true);
            PlayHitAudio(snowball.IsIceBall ? 1 : 0);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("CameraDeadZoneX") && _playerVCam)
            {
                IsInFollowZone = true;
            }

            if (!other.TryGetComponent(out INetworkTriggerable triggerable)) return;
            
            _currentTriggerable ??= triggerable;
            if (photonView.IsMine)
            {
                _currentTriggerable?.TriggerableEnter();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.tag.Equals("CameraDeadZoneX") || !IsInFollowZone || !_playerVCam) return;
            
            CameraLockX = _playerVCam.State.RawPosition.x;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals("CameraDeadZoneX") && _playerVCam)
            {
                CameraLockX = _playerVCam.State.RawPosition.x;
                IsInFollowZone = false;
            }

            if (!other.TryGetComponent(out INetworkTriggerable triggerable) || _currentTriggerable != triggerable) return;
            
            if (photonView.IsMine)
            {
                _currentTriggerable?.TriggerableExit();
            }

            _currentTriggerable = null;
        }

        private void OnPlayerJoinedTeam(Photon.Realtime.Player player, PhotonTeam team)
        {
            if (this == null || !photonView.IsMine) return;
            
            PlayerCustomization.UpdateTeamColorVisuals();
        }

        private void OnPlayerLeftTeam(Photon.Realtime.Player player, PhotonTeam team)
        {
            if (this == null || !photonView.IsMine) return;
            
            PlayerCustomization.RestoreTeamlessColors();
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
                stream.SendNext(HasSnowball);
                stream.SendNext(_isDigging);
                stream.SendNext(_isWalking);
                stream.SendNext(_isAiming);
                stream.SendNext(_currentHealth);
                stream.SendNext(IsDead);
                stream.SendNext(IsFrozen);
                stream.SendNext(_worldUI.gameObject.activeSelf);
                stream.SendNext(_playerModel.gameObject.activeSelf);
                stream.SendNext(_canvasTransform.rotation);
                stream.SendNext(_canvasTransform.localPosition);
                stream.SendNext(_healthTransform.gameObject.activeSelf);
                stream.SendNext(_nicknameTransform.offsetMax);
                stream.SendNext(_nicknameTransform.offsetMin);
                stream.SendNext(_hostCrown.gameObject.activeSelf);
                stream.SendNext(IsBeingControlled);
                stream.SendNext(_target);
                stream.SendNext(IsKicking);
                stream.SendNext(IsUsingCannon);
                stream.SendNext(IsInClass);
            }
            else
            {
                HasSnowball = (bool)stream.ReceiveNext();
                _isDigging = (bool)stream.ReceiveNext();
                _isWalking = (bool)stream.ReceiveNext();
                _isAiming = (bool)stream.ReceiveNext();
                _currentHealth = (int)stream.ReceiveNext();
                IsDead = (bool)stream.ReceiveNext();
                IsFrozen = (bool)stream.ReceiveNext();
                _worldUI.gameObject.SetActive((bool)stream.ReceiveNext());
                _playerModel.gameObject.SetActive((bool)stream.ReceiveNext());
                _canvasTransform.rotation = (Quaternion)stream.ReceiveNext();
                _canvasTransform.localPosition = (Vector3)stream.ReceiveNext();
                _healthTransform.gameObject.SetActive((bool)stream.ReceiveNext());
                _nicknameTransform.offsetMax = (Vector2)stream.ReceiveNext();
                _nicknameTransform.offsetMin = (Vector2)stream.ReceiveNext();
                _hostCrown.gameObject.SetActive((bool)stream.ReceiveNext());
                IsBeingControlled = (bool)stream.ReceiveNext();
                _target = (Vector3)stream.ReceiveNext();
                IsKicking = (bool)stream.ReceiveNext();
                IsUsingCannon = (bool)stream.ReceiveNext();
                IsInClass = (bool) stream.ReceiveNext();
            }
        }

        #endregion

        #region Actions
        
        /// <summary>
        /// Change player's position and orientation in global axes using Character Controller
        /// </summary>
        private void MoveStudent()
        {
            if (IsBeingControlled) return;

            // If the player's character controller is disabled, then don't move them. Otherwise, move them. 
            if (CharacterControllerComponent.enabled)
            {
                if (_isSliding)
                {
                    var desiredVelocity = _playerPosition * _movementSpeed;
                    _playerCurrentVelocity = Vector3.Lerp(_playerCurrentVelocity, desiredVelocity, _movementFriction * Time.deltaTime * 0.5f);
                    CharacterControllerComponent.Move(_playerCurrentVelocity * Time.deltaTime);
                }
                else
                {
                    CharacterControllerComponent.Move(_playerPosition * (_movementSpeed * Time.deltaTime));
                }
            }

            if (CurrentInteractable == null)
            {
                SetPlayerRotation(PlayerRotation);
            }

            ComputeWalkAnimation();
        }

        /// <summary>
        /// Change player's position and orientation in global axes using Character Controller
        /// </summary>
        private void MoveToPoint()
        {
            if (!IsBeingControlled) return;

            var gravity = Physics.gravity.y * Time.deltaTime * 100f;
            _playerPosition.y += gravity;

            var offset = _target - transform.position;
            offset.y = 0;

            if (offset.magnitude > .1f)
            {
                _isWalking = true;
                photonView.RPC(nameof(SetWalkHashBool_RPC), RpcTarget.All, true);
                CharacterControllerComponent.Move(offset.normalized * (_movementSpeed * Time.deltaTime));
            }
            else
            {
                StopControlledMovement();
                _isWalking = false;
                photonView.RPC(nameof(SetWalkHashBool_RPC), RpcTarget.All, false);
            }

            PlayerRotation = Quaternion.LookRotation(offset.normalized);
            _playerModel.rotation = PlayerRotation;
            
            Animator.SetFloat(DeltaY, 1f);
            Animator.SetFloat(DeltaX, 0f);
        }

        /// <summary>
        /// Activates Digging Stopwatch and creates snowball on hand once digging complete
        /// </summary>
        private void DigSnowball()
        {
            if (!photonView.IsMine || IsFrozen) return;

            if (_isDigging && !HasSnowball)
            {
                _digSnowballTimer += Time.deltaTime;
                var arc2Float = _digSnowballRadialBar.material.GetFloat("_Arc2");
                _digSnowballRadialBar.material.SetFloat("_Arc2", arc2Float - _diggingBarDecrement);
            }

            if (_digSnowballTimer < _digSnowballMaxTime) return;
            
            var prefabToSpawn = IsInClass ? RoomManager.Instance.SnowballPrefab.name : ArenaManager.Instance.SnowballPrefab.name;
            if (_currentStandingGround == LayerMask.NameToLayer("Ice"))
            {
                prefabToSpawn = ArenaManager.Instance.IceballPrefab.name;
            }
            _currentObjectInHand = PhotonNetwork.Instantiate(prefabToSpawn, _playerHand.position, _playerHand.rotation * Quaternion.Euler(0f, -90f, 0f));
            // TODO: Object pooling to avoid using GetComponent at Instantiation
            _playerSnowball = _currentObjectInHand.GetComponent<NetworkSnowball>();
            _playerSnowball.SetSnowballThrower(this);
            _playerSnowball.SetHoldingPlace(transform);
            HasSnowball = true;
            _isDigging = false;
            _digSnowballTimer = 0.0f;
            _digSnowballRadialBar.material.SetFloat("_Arc2", 360f);
            photonView.RPC(nameof(SetDigHashBool_RPC), RpcTarget.All, false);
            photonView.RPC(nameof(SetHasSnowballHashBool_RPC), RpcTarget.All, true);

            if (photonView.IsMine && !IsInClass)
            {
                ScoreManager.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, ScoreManager.ShovelerKey);
            }
        }

        /// <summary>
        /// Throws snowball once activated
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void ThrowStudentSnowball()
        {
            if (!photonView.IsMine || _playerSnowball == null || IsFrozen) return;

            _playerSnowball.DisableLineRenderer();
            _isAiming = false;
            _throwForce = _minForce;
            _throwAngle = _minAngle;
            _playerSnowball.ThrowSnowball();
            HasSnowball = false;
            _currentObjectInHand = null;
            _playerSnowball = null;
            photonView.RPC(nameof(SetHasSnowballHashBool_RPC), RpcTarget.All, false);

            if (photonView.IsMine)
            {
                ScoreManager.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, ScoreManager.TeachersPetKey);
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
            _isWalking = _inputMovement.magnitude > 0.0f;

            if (!photonView.IsMine) return;

            switch (_isWalking)
            {
                case false:
                    photonView.RPC("SetWalkHashBool_RPC", RpcTarget.All, false);
                    break;
                case true when IsFrozen:
                    photonView.RPC("SetWalkHashBool_RPC", RpcTarget.All, false);
                    break;
                case true when !IsFrozen:
                    photonView.RPC("SetWalkHashBool_RPC", RpcTarget.All, true);
                    break;
            }
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Translate mouse 2D Vector input action into angular rotation
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void OnLook(InputAction.CallbackContext context)
        {
            if (PlayerCamera == null || IsFrozen) return;

            var mousePosAngle = Utilities.MousePosToRotationInput(_playerTransform, PlayerCamera);
            PlayerRotation = Quaternion.Euler(0f, mousePosAngle, 0f);
        }

        // ReSharper disable once UnusedMember.Global
        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (PlayerCamera == null || IsFrozen) return;

            MousePosition = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Activates the Right Mouse Button
        /// </summary>
        /// <param name="context"></param>
        // ReSharper disable once UnusedMember.Global
        public void OnDig(InputAction.CallbackContext context)
        {
            //If the player is currently interacting with an interactable, don't dig
            if (CurrentInteractable != null || IsFrozen) return;

            // If player has snowball on hand or character is in the air, don't dig
            if (HasSnowball || !CharacterControllerComponent.isGrounded) return;

            // Don't allow player to dig while on ice and sliding fast
            if (_currentStandingGround == LayerMask.NameToLayer("Ice") &&
                CharacterControllerComponent.velocity.magnitude >= 2.0f) return;

            //  Can't dig at surface with no ice or snow
            if (_currentStandingGround != LayerMask.NameToLayer("Ground") &&
                _currentStandingGround != LayerMask.NameToLayer("Ice")) return;

            // If action is being performed, start digging
            if (context.performed)
            {
                _isDigging = true;
                
                if (photonView.IsMine)
                {
                    photonView.RPC("SetDigHashBool_RPC", RpcTarget.All, true);
                }
            }
            // If digging is abruptly cancelled, cancel action and reset timer
            if (context.canceled)
            {
                _isDigging = false;
                _digSnowballTimer = 0.0f;
                _digSnowballRadialBar.material.SetFloat("_Arc2", 360f);
                if (photonView.IsMine)
                {
                    photonView.RPC("SetDigHashBool_RPC", RpcTarget.All, false);
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
            if (IsFrozen) return;

            // If the action is performed, do something
            if (context.performed)
            {
                //If player has a snowball, then start aiming it. Otherwise call the Interactable's Click method.
                if (HasSnowball && !_isDigging)
                {
                    _isAiming = true;

                    if (photonView.IsMine)
                    {
                        Animator.SetBool(PrepareThrow, true);
                    }
                }
                else
                {
                    CurrentInteractable?.Click();
                }
            }

            // If the action is cancelled, do something
            if (context.canceled)
            {
                //If player has a snowball, then throw it. Otherwise call the Interactable's Release method.
                if (photonView.IsMine && HasSnowball)
                {
                    AudioManager.Instance.PlayOneShot(_snowballThrowSound, IsInClass ? 0.5f : 2f);
                    photonView.RPC(nameof(PlaySnowballThrowAnimation), RpcTarget.All);
                    _threwSnowball = true;
                    Animator.SetBool(PrepareThrow, false);
                }
                else
                {
                    CurrentInteractable?.Release();
                }
            }
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Activates the Interaction key, currently E
        /// </summary>
        /// <param name="context"></param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!photonView.IsMine || !context.performed || IsFrozen) return;

            _currentTriggerable?.TriggerableTrigger(this);

            if (HasSnowball || IsDead) return; //Don't interact with interactables if the player already has a snowball. 

            // TODO: Interact with other items here
            // When you press 'E', it checks to see if there is an
            // interactable nearby and if there is, assume control of it. 

            if (CurrentInteractable == null)
            {
                CurrentInteractable = ReturnNearestInteractable();
                CurrentInteractable?.Enter(this);
            }
            else
            {
                CurrentInteractable?.Exit();
                CurrentInteractable = null;
            }
        }

        #endregion

        #region PublicCalls
        
        /// <summary>
        /// Sets the isFrozen boolean
        /// Can also be used to add additional things to be disabled/enabled
        /// </summary>
        /// <param name="isFrozen"></param>
        public void SetStudentFreezeState(bool isFrozen)
        {
            IsFrozen = isFrozen;
            _isAiming = false;
            _throwForce = _minForce;
            _throwAngle = _minAngle;
        }

        public void Unhit()
        {
            Animator.SetBool(Hit, false);
        }

        public void UnhitSides()
        {
            Animator.SetBool(HitFront, false);
            Animator.SetBool(HitBack, false);
            Animator.SetBool(HitLeft, false);
            Animator.SetBool(HitRight, false);
        }

        public void GetDamaged(int damage)
        {
            photonView.RPC(nameof(GetDamaged_RPC), RpcTarget.AllBuffered, damage);
        }
        
        public void LookAtTeacher(bool isTeacher)
        {
            _playerVCam.gameObject.SetActive(!isTeacher);
            ArenaManager.Instance.TeacherVirtualCamera.gameObject.SetActive(isTeacher);
            _isAiming = !isTeacher;

            if (Animator.GetCurrentAnimatorStateInfo(1).IsName("Prepare Snowball"))
            {
                Animator.SetTrigger(CancelPrepare);
            }

            if (HasSnowball && _playerSnowball)
            {
                _playerSnowball.DisableLineRenderer();
            }
        }
        
        /// <summary>
        /// Attach unique instantiated camera with player
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        /// <param name="isInYard"></param>
        /// <param name="nameTextHeight"></param>
        /// <param name="canvasHeight"></param>
        public void SetCamera(GameObject cam, float angle, float distance, bool isInYard, float nameTextHeight, float canvasHeight)
        {
            IsInClass = !isInYard;
            _playerVCam = cam.GetComponentInChildren<CinemachineVirtualCamera>();
            PlayerCamera = cam.GetComponentInChildren<Camera>();
            _playerVCam.Follow = _playerTransform;
            PlayerVCamFramingTransposer = _playerVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            SetFrameTransposerProperties(angle, distance, canvasHeight);

            _healthTransform.gameObject.SetActive(isInYard);
            _nicknameTransform.SetBottom(nameTextHeight);
            _nicknameTransform.SetTop(-nameTextHeight);
            _hostCrown.SetActive(!isInYard && PhotonNetwork.IsMasterClient);
        }
        
        public void SetControlledMovement(Vector3 target, bool startGame)
        {
            photonView.RPC(nameof(SyncIsControlled), RpcTarget.AllBuffered, true, target);
            IsBeingControlled = true;
            _target = target;
            _startGame = startGame;
        }
        
        public void PlayKickAudio()
        {
            if (!photonView.IsMine) return;
            
            photonView.RPC(nameof(PlayKickAudio_RPC), RpcTarget.All);
        }

        public void SyncIsReady(bool isActive, string userId)
        {
            photonView.RPC(nameof(SyncIsReady_RPC), RpcTarget.AllBuffered, isActive, userId);
        }
        
        public void SetPlayerRotation(Quaternion rotation)
        {
            _playerModel.rotation = rotation;
        }

        public void SetThrewSnowball(bool value)
        {
            _threwSnowball = value;
        }

        public void SetAnimatorParameter(string parameter)
        {
            Animator.SetTrigger(parameter);
        }

        public void SetAnimatorParameter(string parameter, bool value)
        {
            Animator.SetBool(parameter, value);
        }

        public void SetAnimatorParameter(string parameter, int value)
        {
            Animator.SetInteger(parameter, value);
        }

        public void SetAnimatorParameter(string parameter, float value)
        {
            Animator.SetFloat(parameter, value);
        }

        #endregion
        
        #region Utilities
        
        /// <summary>
        /// Track the layer of the object the player is standing on
        /// </summary>
        private void SetStandingGround()
        {
            if (!Physics.Raycast(_playerTransform.position, _playerTransform.TransformDirection(Vector3.down), out var hit, 1.0f)) return;
            
            _currentStandingGround = hit.collider.gameObject.layer;
            if (!_isSliding)
            {
                _playerCurrentVelocity = CharacterControllerComponent.velocity;
            }
            _isSliding = _currentStandingGround == LayerMask.NameToLayer("Ice");
        }

        /// <summary>
        /// Set VCam's FrameTransposer properties
        /// Put it in separate function to modify at any time
        /// If you want to tweak more properties, add more arguments to this function
        /// and the SetCamera(...) above
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        /// <param name="canvasHeight"></param>
        private void SetFrameTransposerProperties(float angle, float distance, float canvasHeight)
        {
            PlayerVCamFramingTransposer.m_CameraDistance = distance;
            _playerVCam.transform.rotation = Quaternion.Euler(angle, 0f, 0f);
            _canvasTransform.rotation = Quaternion.Euler(angle, 0f, 0f);
            _canvasTransform.localPosition = new Vector3(0f, canvasHeight, 0.23f);
            CameraLockX = _playerVCam.State.RawPosition.x;
            IsInFollowZone = true;
        }

        /// <summary>
        /// Utility function that returns the nearest interactable to the player
        /// </summary>
        /// <returns></returns>
        private INetworkInteractable ReturnNearestInteractable()
        {
            INetworkInteractable interactable = null;
            const int maxColliders = 10; //maximum number of objects near to the player that can be looped through
            var hitColliders = new Collider[maxColliders];
            var numColliders = Physics.OverlapSphereNonAlloc(_playerTransform.position, 1.5f, hitColliders);

            if (numColliders < 1) return null;

            //Loop through 10 nearest objects and check if any of them are interactables that implement IInteractable
            for (var i = 0; i < numColliders; i++)
            {
                if (!hitColliders[i].gameObject.TryGetComponent<INetworkInteractable>(out var interactableObject) || interactableObject.IsActive) continue;

                interactable = interactableObject;
            }

            return interactable;
        }
        
        /// <summary>
        /// Respawn player after death delay
        /// </summary>
        private void Respawn()
        {
            _playerModel.gameObject.SetActive(true);
            _worldUI.gameObject.SetActive(true);
            _currentHealth = _maxHealth;
            _playerTransform.position = ArenaManager.Instance.GetPlayerSpawnPoint(TeamID);
            CharacterControllerComponent.enabled = true;
            IsDead = false;
            photonView.RPC(nameof(ResetHeartsVisibility_RPC), RpcTarget.All);
        }
        
        /// <summary>
        /// Increase throw force every frame when aiming snowball
        /// </summary>
        private void IncreaseThrowForce()
        {
            _throwForce += Time.deltaTime * _forceIncreaseTimeRate;
            _playerSnowball.SetSnowballForce(_throwForce);
        }

        private void IncreaseThrowAngle()
        {
            _throwAngle += Time.deltaTime * _angleIncreaseTimeRate;
            _playerSnowball.SetSnowballAngle(_throwAngle);
        }
        
        /// <summary>
        /// Computes the animator parameters for the walk animations blend tree
        /// </summary>
        private void ComputeWalkAnimation()
        {
            var moveAngle = Vector2.SignedAngle(_inputMovement, new Vector2(0, 1));
            if (moveAngle < 0)
            {
                moveAngle = 360f + moveAngle;
            }

            var rotationAngle = _playerModel.eulerAngles.y;
            if (rotationAngle < 0)
            {
                rotationAngle = 360f + rotationAngle;
            }

            var deltaDegrees = moveAngle - rotationAngle;
            if (deltaDegrees < 0)
            {
                deltaDegrees = 360f + deltaDegrees;
            }

            var deltaRadians = Mathf.Deg2Rad * deltaDegrees;
            var deltaVector = new Vector2(Mathf.Sin(deltaRadians), Mathf.Cos(deltaRadians));
            _deltaVector = Vector2.Lerp(_deltaVector, deltaVector, 20f * Time.deltaTime);

            Animator.SetFloat(DeltaX, _deltaVector.x);
            Animator.SetFloat(DeltaY, _deltaVector.y);
        }
        
        /// <summary>
        /// Simulate snowman melting by changing transform
        /// and destroy it when time's up
        /// NOTE: This function is called on an RPC method
        /// </summary>
        /// <param name="snowGuy"></param>
        private static IEnumerator DestroySnowman(GameObject snowGuy)
        {
            var timer = ArenaManager.Instance.SnowmanTimer;

            while (timer > 0f)
            {
                snowGuy.transform.Translate(Vector3.down * Time.deltaTime / 2f, Space.World);
                timer -= Time.deltaTime;

                yield return null;
            }

            Destroy(snowGuy);
        }

        /// <summary>
        /// Set the visibility of the hearts according to the player's health
        /// NOTE: This function is called on an RPC method
        /// </summary>
        private void SetHeartsVisibility()
        {
            for (var i = 1; i <= _maxHealth; i++)
            {
                _hearts[i - 1].color = i <= _currentHealth ? Color.white : new Color(1f, 1f, 1f, 0.0f);
            }
        }
        
        private void StopControlledMovement()
        {
            _target = Vector3.zero;

            if (!_startGame) return;
            
            _startGame = false;
            RoomManager.Instance.StartGame();
        }

        private void PlayHitAudio(int ballType)
        {
            switch (ballType)
            {
                case 0:
                    _audioSource.PlayOneShot(_hitBySnowballSound, 2f * AudioManager.Instance.Volume);
                    break;
                case 1:
                    _audioSource.PlayOneShot(_hitByIceballSound, 2f * AudioManager.Instance.Volume);
                    break;
                default:
                    _audioSource.PlayOneShot(_hitByRollballSound, 2f * AudioManager.Instance.Volume);
                    break;
            }
        }

        #endregion
        
        #region RPCs
        
        /// <summary>
        /// Student gets damaged when snowball or combat item is thrown at them successfully
        /// </summary>
        /// <param name="damage"></param>
        [PunRPC]
        private void GetDamaged_RPC(int damage)
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

            if (_currentHealth > 0) return;
            
            // Resetting general settings
            IsDead = true;
            _isWalking = false;
            CharacterControllerComponent.enabled = false;
            _worldUI.gameObject.SetActive(false);
            _playerModel.gameObject.SetActive(false);

            // Resets the current interactable
            if (photonView.IsMine)
            {
                _audioSource.pitch = 1f;
                _audioSource.Stop();
                AudioManager.Instance.PlayOneShot(_deathSound);
                CurrentInteractable?.Exit();
                CurrentInteractable = null;
            }

            // Resetting throwing settings
            if (HasSnowball && _playerSnowball != null)
            {
                _playerSnowball.DisableLineRenderer();
                _isAiming = false;
                _throwForce = _minForce;
                _throwAngle = _minAngle;
                _currentObjectInHand = null;
                _playerSnowball.DestroySnowball();
                _playerSnowball = null;
                HasSnowball = false;
                Animator.SetBool(HasSnowballHash, false);
            }

            // Spawns the snowman
            var snowMan = Instantiate(ArenaManager.Instance.SnowmanPrefab, _playerTransform.position, _playerTransform.rotation);
            StartCoroutine(DestroySnowman(snowMan));

            if (photonView.IsMine)
            {
                ScoreManager.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, ScoreManager.DeathsKey);
                ArenaManager.Instance.IncrementTeamDeathCount(TeamID);
                Invoke(nameof(Respawn), _deathTimeDelay);
            }
                
            ArenaManager.Instance.SetLeadingShirt(ScoreManager.Instance.GetLeadingTeam());
        }

        [PunRPC]
        private void ResetHeartsVisibility_RPC()
        {
            for (var i = 1; i <= _maxHealth; i++)
            {
                _hearts[i - 1].color = Color.white;
            }
        }
        
        [PunRPC]
        private void SetWalkHashBool_RPC(bool walking)
        {
            Animator.SetBool(IsWalkingHash, walking);
        }

        [PunRPC]
        private void SetDigHashBool_RPC(bool digging)
        {
            Animator.SetBool(IsDiggingHash, digging);
        }

        [PunRPC]
        private void SetHasSnowballHashBool_RPC(bool hasSnowball)
        {
            Animator.SetBool(HasSnowballHash, hasSnowball);
        }
        
        [PunRPC]
        private void SyncPlayerInfo(string playerID, byte teamID)
        {
            PlayerID = playerID;
            TeamID = teamID;
        }

        [PunRPC]
        private void SyncIsControlled(bool isBeingControlled, Vector3 target)
        {
            IsBeingControlled = isBeingControlled;
            _target = target;
        }

        [PunRPC]
        private void SetBoolRPC(string boolean, bool value)
        {
            Animator.SetBool(boolean, value);
        }

        [PunRPC]
        private void PlaySnowballThrowAnimation()
        {
            Animator.SetTrigger(ThrowSnowball);
        }

        [PunRPC]
        private void SyncIsReady_RPC(bool isActive, string userId)
        {
            if (userId != PlayerID) return;
            
            _isReadyBubble.SetActive(isActive);
        }
        
        [PunRPC]
        private void PlayKickAudio_RPC()
        {
            _audioSource.PlayOneShot(_kickSound, 2f * AudioManager.Instance.Volume);
        }

        #endregion
    }
}