using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Interfaces;
using Networking;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Image = UnityEngine.UI.Image;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

        [Header("Clothing")]
        [SerializeField] private GameObject _teamShirt;
        [SerializeField] private GameObject[] _playerHats;
        [SerializeField] private GameObject _playerBoots;
        [SerializeField] private GameObject[] _playerPants;
        [SerializeField] private GameObject[] _playerHairStyles;
        [SerializeField] private GameObject[] _playerCoats;
        [SerializeField] private GameObject _playerSkin;
        [SerializeField] private Color[] skinColors;
        [SerializeField] private Color[] _colors;
        private int _skinColorIndex;

        private GameObject _currentHat;
        private int _hatIndex;

        private GameObject _currentPants;
        private int _pantIndex;
        private int _pantColorIndex;
        private Color _pantColor;

        private GameObject _currentHairStyle;
        private int _hairStyleIndex;
        private int _hairColorIndex;
        private Color _hairColor;

        private GameObject _currentCoat;
        private int _coatIndex;
        private int _coatColorIndex;
        private Color _coatColor;

        private List<Renderer> _playerHatRenderers;

        [Header("Movement")]
        [SerializeField] [Min(0)] private float _movementSpeed;
        [SerializeField] [Range(0f, 1f)] private float _movementFriction;
        private Vector3 _playerCurrentVelocity;
        private Vector3 _playerPosition;
        private Quaternion _playerRotation;
        private Vector2 _inputMovement;
        private Vector2 _deltaVector;
        private bool _isBeingControlled;
        private Vector3 _target = Vector3.zero;
        private bool _startGame;

        [Header("Player UI")] 
        [SerializeField] private Canvas _worldUI;
        [SerializeField] private RectTransform _canvasTransform;
        [SerializeField] private RectTransform _healthTransform;
        [SerializeField] private RectTransform _nicknameTransform;
        [SerializeField] private Image[] _hearts;
        [SerializeField] private int _maxHealth;
        private int _currentHealth;
        private GameObject _currentObjectInHand;
        private INetworkInteractable _currentInteractable;
        private INetworkTriggerable _currentTriggerable;

        [Header("Snowball")]
        [SerializeField] private Transform _playerHand;
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

        [Header("Booleans")]
        private bool _isWalking;
        private bool _isDigging;
        private bool _hasSnowball;
        private bool _isDead;
        private bool _isFrozen;
        private bool _isAiming;
        private bool _threwSnowball;
        private bool _isSliding;
        // List of readonly files. No need for them to have a _ prefix
        private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
        private static readonly int IsDiggingHash = Animator.StringToHash("isDigging");
        private static readonly int HasSnowballHash = Animator.StringToHash("hasSnowball");
        private static readonly int ThrowSnowball = Animator.StringToHash("ThrowSnowball");
        private static readonly int DeltaX = Animator.StringToHash("deltaX");
        private static readonly int DeltaY = Animator.StringToHash("deltaY");
        private static readonly int PrepareThrow = Animator.StringToHash("PrepareThrow");
        private static readonly int CancelPrepare = Animator.StringToHash("CancelPrepare");

        // PROPERTIES (REPLACE PUBLIC GETTERS)
        public CharacterController CharacterControllerComponent => _characterController;
        public Quaternion PlayerRotation => _playerRotation;
        public Transform PlayerHand => _playerHand;
        public bool HasSnowball => _hasSnowball;
        public bool IsDead => _isDead;
        public CinemachineFramingTransposer PlayerVCamFramingTransposer => _playerVCamFramingTransposer;
        public bool UsingCannon { get; set; }

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

            _playerHatRenderers = new List<Renderer>();

            foreach (var playerHat in _playerHats)
            {
                _playerHatRenderers.Add(playerHat.GetComponent<Renderer>());
            }

            _currentHat = _playerHats[0];
            _currentCoat = _playerCoats[0];
            _currentHairStyle = _playerHairStyles[0];
            _currentPants = _playerPants[0];

            _hatIndex = 0;
            _coatIndex = 0;
            _hairStyleIndex = 0;
            _pantIndex = 0;

            _pantColorIndex = 0;
            _hairColorIndex = 0;
            _coatColorIndex = 0;
            _skinColorIndex = 0;

            _hairColor = Color.black;
            _pantColor = Color.white;
            _coatColor = Color.white;
        }

        private void Start()
        {
            _isJerseyNull = _jersey == null;
            _throwForce = _minForce;
            _throwAngle = _minAngle;
            _currentHealth = _maxHealth;
            SetNameText();
            UpdateTeamColorVisuals();
            PhotonTeamsManager.PlayerJoinedTeam += OnPlayerJoinedTeam;
            PhotonTeamsManager.PlayerLeftTeam += OnPlayerLeftTeam;
            _isBeingControlled = false;
            _target = Vector3.zero;
            _startGame = false;

            if (photonView.IsMine)
            {
                photonView.RPC(nameof(SyncPlayerInfo), RpcTarget.AllBuffered, PlayerID, TeamID);
            }
        }

        private void Update()
        {
            if (!photonView.IsMine || _isDead) return;

            SetStandingGround();

            if (!_isDigging && !_isFrozen)
            {
                if (!_isBeingControlled)
                    MoveStudent();
                else if (_target != Vector3.zero)
                    MoveToPoint();
            }

            if (_isAiming && _hasSnowball && !_isFrozen)
            {
                if (_throwForce <= _maxForce && !_threwSnowball)
                {
                    IncreaseThrowForce();
                }

                if (_throwAngle <= _maxAngle && !_threwSnowball)
                {
                    IncreaseThrowAngle();
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
            if (this == null) return;
            if (!photonView.IsMine) return;
            photonView.RPC("UpdateTeamColorVisuals", RpcTarget.AllBuffered);
        }

        private void OnPlayerLeftTeam(Photon.Realtime.Player player, PhotonTeam team)
        {
            if (this == null) return;
            if (!photonView.IsMine) return;
            photonView.RPC("RestoreTeamlessColors", RpcTarget.AllBuffered);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out INetworkTriggerable triggerable))
            {
                _currentTriggerable ??= triggerable;
                if (photonView.IsMine) _currentTriggerable?.Enter();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out INetworkTriggerable triggerable) && _currentTriggerable == triggerable)
            {
                if(photonView.IsMine) _currentTriggerable?.Exit();
                _currentTriggerable = null;
            }
        }
        #endregion

        #region Actions


        /// <summary>
        /// Change player's position and orientation in global axes using Character Controller
        /// </summary>
        private void MoveStudent()
        {
            if (_isBeingControlled) return;

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

            _animator.SetFloat(DeltaX, _deltaVector.x);
            _animator.SetFloat(DeltaY, _deltaVector.y);
        }

        /// <summary>
        /// Change player's position and orientation in global axes using Character Controller
        /// </summary>
        private void MoveToPoint()
        {
            if (!this._isBeingControlled) return;

            var gravity = Physics.gravity.y * Time.deltaTime * 100f;
            _playerPosition.y += gravity;

            Vector3 offset = _target - transform.position;
            offset.y = 0;

            if (offset.magnitude > .1f)
            {
                _isWalking = true;
                photonView.RPC("SetWalkHashBool_RPC", RpcTarget.All, true);
                _characterController.Move(offset.normalized * (_movementSpeed * Time.deltaTime));
            }
            else
            {
                StopControlledMovement();
                _isWalking = false;
                photonView.RPC("SetWalkHashBool_RPC", RpcTarget.All, false);

            }

            _playerRotation = Quaternion.LookRotation(offset.normalized);
            _playerModel.rotation = _playerRotation;

            var moveAngle = Vector2.SignedAngle(offset.normalized, new Vector2(0, 1));
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

            _animator.SetFloat(DeltaX, _deltaVector.x);
            _animator.SetFloat(DeltaY, _deltaVector.y);
        }

        /// <summary>
        /// Activates Digging Stopwatch and creates snowball on hand once digging complete
        /// </summary>
        private void DigSnowball()
        {
            if (!photonView.IsMine || _isFrozen) return;

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
            photonView.RPC("SetDigHashBool_RPC", RpcTarget.All, false);
            photonView.RPC("SetHasSnowballHashBool_RPC", RpcTarget.All, true);
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
        /// Throws snowball once activated
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void ThrowStudentSnowball()
        {
            if (!photonView.IsMine || _playerSnowball == null || _isFrozen) return;

            _playerSnowball.DisableLineRenderer();
            _isAiming = false;
            _throwForce = _minForce;
            _throwAngle = _minAngle;
            _playerSnowball.ThrowSnowball();
            _hasSnowball = false;
            _currentObjectInHand = null;
            _playerSnowball = null;
            photonView.RPC("SetHasSnowballHashBool_RPC", RpcTarget.All, false);
            //_animator.SetBool(HasSnowballHash, false);

            if (photonView.IsMine)
            {
                ScoreManager.Instance.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, "ballsThrown");
            }
        }

        /// <summary>
        /// Sets the isFrozen boolean
        /// Can also be used to add additional things to be disabled/enabled
        /// </summary>
        /// <param name="isFrozen"></param>
        public void SetStudentFreezeState(bool isFrozen)
        {
            _isFrozen = isFrozen;
            _isAiming = false;
            _throwForce = _minForce;
            _throwAngle = _minAngle;
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
                // Resetting general settings
                _isDead = true;
                _isWalking = false;
                _characterController.enabled = false;
                _worldUI.gameObject.SetActive(false);
                _playerModel.gameObject.SetActive(false);

                // Resets the current interactable
                _currentInteractable?.Exit();
                _currentInteractable = null;

                // Resetting throwing settings
                if (_hasSnowball && _playerSnowball != null)
                {
                    _playerSnowball.DisableLineRenderer();
                    _isAiming = false;
                    _throwForce = _minForce;
                    _throwAngle = _minAngle;
                    _currentObjectInHand = null;
                    _playerSnowball.DestroySnowball();
                    _playerSnowball = null;
                    _hasSnowball = false;
                    _animator.SetBool(HasSnowballHash, false);
                }

                // Spawns the snowman
                var snowMan = Instantiate(ArenaManager.Instance.SnowmanPrefab, _studentTransform.position, _studentTransform.rotation);
                StartCoroutine(DestroySnowman(snowMan));

                if (photonView.IsMine)
                {
                    ScoreManager.Instance.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, "deaths");
                    Invoke(nameof(Respawn), DEATH_TIME_DELAY);
                }
            }
        }

        /// <summary>
        /// Simulate snowman melting by changing transform
        /// and destroy it when time's up
        /// NOTE: This function is called on an RPC method
        /// </summary>
        /// <param name="snowGuy"></param>
        /// <returns></returns>
        private IEnumerator DestroySnowman(GameObject snowGuy)
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
        /// Respawn player after death delay
        /// NOTE: This function is called on an RPC method
        /// </summary>
        private void Respawn()
        {
            _playerModel.gameObject.SetActive(true);
            _worldUI.gameObject.SetActive(true);
            _currentHealth = _maxHealth;
            _studentTransform.position = ArenaManager.Instance.GetPlayerSpawnPoint(this);
            _characterController.enabled = true;
            _isDead = false;
            photonView.RPC(nameof(ResetHeartsVisibilityRPC), RpcTarget.All);
        }

        /// <summary>
        /// Set the visibility of the hearts according to the player's health
        /// NOTE: This function is called on an RPC method
        /// </summary>
        private void SetHeartsVisibility()
        {
            for (var i = 1; i <= _maxHealth; i++)
            {
                _hearts[i - 1].color = i <= _currentHealth ? Color.white : new Color(1f, 1f, 1f, 0.5f);
            }
        }

        /// <summary>
        /// RPC version of the method above
        /// Used on a different call
        /// </summary>
        [PunRPC]
        private void ResetHeartsVisibilityRPC()
        {
            for (var i = 1; i <= _maxHealth; i++)
            {
                _hearts[i-1].color = Color.white;
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
                case true when _isFrozen:
                    photonView.RPC("SetWalkHashBool_RPC", RpcTarget.All, false);
                    break;
                case true when !_isFrozen:
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
            if (_playerCamera == null || _isFrozen) return;

            var mousePosAngle = Utilities.MousePosToRotationInput(_studentTransform, _playerCamera);
            _playerRotation = Quaternion.Euler(0f, mousePosAngle, 0f);
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Activates the Right Mouse Button
        /// </summary>
        /// <param name="context"></param>
        // ReSharper disable once UnusedMember.Global
        public void OnDig(InputAction.CallbackContext context)
        {
            //If the player is currently interacting with an interactable, don't dig
            if (_currentInteractable != null || _isFrozen) return;

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
                    photonView.RPC("SetDigHashBool_RPC", RpcTarget.All, true);
                }
            }
            // If digging is abruptly cancelled, cancel action and reset timer
            if (context.canceled)
            {
                _isDigging = false;
                _digSnowballTimer = 0.0f;
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
            if (_isFrozen) return;

            // If the action is performed, do something
            if (context.performed)
            {
                //If player has a snowball, then start aiming it. Otherwise call the Interactable's Click method.
                if (_hasSnowball && !_isDigging)
                {
                    _isAiming = true;

                    if (photonView.IsMine)
                    {
                        _animator.SetBool(PrepareThrow, true);
                    }
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
                    photonView.RPC(nameof(PlaySnowballThrowAnimation), RpcTarget.All);
                    _threwSnowball = true;
                    _animator.SetBool(PrepareThrow, false);
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
            if (!photonView.IsMine || !context.performed || _isFrozen) return;

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

        #endregion

        #region AnimationRPCs

        [PunRPC]
        public void SetWalkHashBool_RPC(bool walking) { _animator.SetBool(IsWalkingHash, walking); }
        [PunRPC]
        public void SetDigHashBool_RPC(bool digging){ _animator.SetBool(IsDiggingHash, digging);}
        [PunRPC]
        public void SetHasSnowballHashBool_RPC(bool hasSnowball) { _animator.SetBool(HasSnowballHash, hasSnowball); }

        #endregion

        #region RPC

        [PunRPC]
        private void SyncPlayerInfo(string playerID, byte teamID)
        {
            PlayerID = playerID;
            TeamID = teamID;
        }

        [PunRPC]
        private void SyncIsControlled(bool isBeingControlled, Vector3 target)
        {
            _isBeingControlled = isBeingControlled;
            _target = target;
        }

        //[PunRPC]

        [PunRPC]
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
            if (photonView.Owner.CustomProperties.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out var teamId) &&
                PhotonTeamsManager.Instance.TryGetTeamByCode((byte)teamId, out var team))
            {
                if (!_teamShirt.activeSelf) _teamShirt.SetActive(true);
                switch (team.Code)
                {
                    //Temporary color changing code
                    //TODO: Make this code more efficient so as to not use getComponent a lot and to also not use foreach
                    case 1:
                        _teamShirt.GetComponent<Renderer>().material.color = Color.blue;
                        _playerBoots.GetComponent<Renderer>().material.color = Color.blue;
                        foreach (var playerHatRenderer in _playerHatRenderers)
                        {
                            playerHatRenderer.material.color = Color.blue;
                        }
                        _nickNameText.color = Color.blue;
                        break;
                    case 2:
                        _teamShirt.GetComponent<Renderer>().material.color = Color.red;
                        _playerBoots.GetComponent<Renderer>().material.color = Color.red;
                        foreach (var playerHatRenderer in _playerHatRenderers)
                        {
                            playerHatRenderer.material.color = Color.red;
                        }
                        _nickNameText.color = Color.red;
                        break;
                }
            }
        }

        [PunRPC]
        public void SwitchHat()
        {
            Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;
            _currentHat.SetActive(false);
            if (_hatIndex + 1 >= _playerHats.Length)
            {
                _hatIndex = 0;
            }
            else _hatIndex++;

            _currentHat = _playerHats[_hatIndex];
            _currentHat.SetActive(true);

            if (properties.ContainsKey("hatIndex")) properties["hatIndex"] = _hatIndex;
            else properties.Add("hatIndex", _hatIndex);
        }


        [PunRPC]
        public void SwitchPants()
        {
            Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;

            _currentPants.SetActive(false);
            if (_pantIndex + 1 >= _playerPants.Length)
            {
                _pantIndex = 0;
            }
            else _pantIndex++;

            _currentPants = _playerPants[_pantIndex];
            _currentPants.SetActive(true);
            _currentPants.GetComponent<Renderer>().material.color = _pantColor;

            if (properties.ContainsKey("pantIndex")) properties["pantIndex"] = _pantIndex;
            else properties.Add("pantIndex", _pantIndex);
        }

        [PunRPC]
        public void SwitchPantsColor()
        {
            Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;

            if (_pantColorIndex + 1 >= _colors.Length) _pantColorIndex = 0;
            else _pantColorIndex++;
            _pantColor = _colors[_pantColorIndex];
            _currentPants.GetComponent<Renderer>().material.color = _pantColor;

            if (properties.ContainsKey("pantColorIndex")) properties["pantColorIndex"] = _pantColorIndex;
            else properties.Add("pantColorIndex", _pantColorIndex);
        }

        [PunRPC]
        public void SwitchCoat()
        {
            Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;

            _currentCoat.SetActive(false);
            if (_coatIndex + 1 >= _playerCoats.Length)
            {
                _coatIndex = 0;
            }
            else _coatIndex++;
            _currentCoat = _playerCoats[_coatIndex];
            _currentCoat.SetActive(true);
            _currentCoat.GetComponent<Renderer>().material.color = _coatColor;

            if (properties.ContainsKey("coatIndex")) properties["coatIndex"] = _coatIndex;
            else properties.Add("coatIndex", _coatIndex);
        }

        [PunRPC]
        public void SwitchCoatColor()
        {
            Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;

            if (_coatColorIndex + 1 >= _colors.Length)
            {
                _coatColorIndex = 0;
            }
            else
            {
                _coatColorIndex++;
            }
            _coatColor = _colors[_coatColorIndex];
            _currentCoat.GetComponent<Renderer>().material.color = _coatColor;

            if (properties.ContainsKey("coatColorIndex")) properties["coatColorIndex"] = _coatColorIndex;
            else properties.Add("coatColorIndex", _coatColorIndex);
        }

        [PunRPC]
        public void SwitchHair()
        {
            Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;

            _currentHairStyle.SetActive(false);
            if (_hairStyleIndex + 1 >= _playerHairStyles.Length)
            {
                _hairStyleIndex = 0;
            }
            else
            {
                _hairStyleIndex++;
            }

            _currentHairStyle = _playerHairStyles[_hairStyleIndex];
            _currentHairStyle.SetActive(true);

            if (properties.ContainsKey("hairIndex")) properties["hairIndex"] = _hairStyleIndex;
            else properties.Add("hairIndex", _hairStyleIndex);
        }

        [PunRPC]
        public void SwitchHairColor()
        {
            Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;

            if (_hairColorIndex + 1 >= _colors.Length) _hairColorIndex = 0;
            else _hairColorIndex++;
            _hairColor = _colors[_hairColorIndex];
            _currentHairStyle.GetComponent<Renderer>().material.color = _hairColor;

            if (properties.ContainsKey("hairColorIndex")) properties["hairColorIndex"] = _hairColorIndex;
            else properties.Add("hairColorIndex", _hairColorIndex);
        }

        [PunRPC]
        public void SwitchSkinColor()
        {
            Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;

            if (_skinColorIndex + 1 >= skinColors.Length)
            {
                _skinColorIndex = 0;
            }
            else
            {
                _skinColorIndex++;
            }

            _playerSkin.GetComponent<Renderer>().material.color = skinColors[_skinColorIndex];

            if (properties.ContainsKey("skinColorIndex")) properties["skinColorIndex"] = _skinColorIndex;
            else properties.Add("skinColorIndex", _skinColorIndex);
        }




        public void SwitchHat_RPC() { photonView.RPC("SwitchHat", RpcTarget.AllBuffered); }
        public void SwitchCoat_RPC() { photonView.RPC("SwitchCoat", RpcTarget.AllBuffered); }
        public void SwitchPants_RPC() { photonView.RPC("SwitchPants", RpcTarget.AllBuffered); }
        public void SwitchHair_RPC() { photonView.RPC("SwitchHair", RpcTarget.AllBuffered); }
        public void SwitchHairColor_RPC() { photonView.RPC("SwitchHairColor", RpcTarget.AllBuffered); }
        public void SwitchPantsColor_RPC() { photonView.RPC("SwitchPantsColor", RpcTarget.AllBuffered); }
        public void SwitchCoatColor_RPC() { photonView.RPC("SwitchCoatColor", RpcTarget.AllBuffered); }
        public void SwitchSkinColor_RPC() { photonView.RPC("SwitchSkinColor", RpcTarget.AllBuffered); }

        [PunRPC]
        public void SetHat(int index)
        {
            _currentHat.SetActive(false);
            _currentHat = _playerHats[index];
        }

        [PunRPC]
        public void SetCoat(int index, int colorIndex)
        {
            _currentCoat.SetActive(false);
            _currentCoat = _playerCoats[index];
            _currentCoat.GetComponent<Renderer>().material.color = _colors[colorIndex];
            _currentCoat.SetActive(true);
        }

        [PunRPC]
        public void SetHair(int index, int colorIndex)
        {
            _currentHairStyle.SetActive(false);
            _currentHairStyle = _playerHairStyles[index];
            _currentHairStyle.GetComponent<Renderer>().material.color = _colors[colorIndex];
            _currentHairStyle.SetActive(true);
        }

        [PunRPC]
        public void SetPants(int index, int colorIndex)
        {
            _currentPants.SetActive(false);
            _currentPants = _playerPants[index];
            _currentPants.GetComponent<Renderer>().material.color = _colors[colorIndex];
            _currentPants.SetActive(true);
        }

        [PunRPC]
        public void SetSkinColor(int colorIndex)
        {
            _playerSkin.GetComponent<Renderer>().material.color = skinColors[colorIndex];
        }

        /// <summary>
        /// Temporary function for restoring a player's colors to all white to show they are teamless
        /// </summary>
        [PunRPC]
        public void RestoreTeamlessColors()
        {
            _teamShirt.GetComponent<Renderer>().material.color = Color.white;
            _playerBoots.GetComponent<Renderer>().material.color = Color.white;

            foreach (var playerHatRenderer in _playerHatRenderers)
            {
                playerHatRenderer.material.color = Color.white;
            }

            _nickNameText.color = Color.white;
            _teamShirt.SetActive(false);
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
        /// <param name="isInYard"></param>
        /// <param name="nameTextHeight"></param>
        // ReSharper disable once UnusedMember.Global
        public void SetCamera(GameObject cam, float angle, float distance, bool isInYard, float nameTextHeight)
        {
            _playerVCam = cam.GetComponentInChildren<CinemachineVirtualCamera>();
            _playerCamera = cam.GetComponentInChildren<Camera>();
            _playerVCam.Follow = _studentTransform;
            _playerVCam.Follow = _studentTransform;
            _playerVCamFramingTransposer = _playerVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            SetFrameTransposerProperties(angle, distance);
            
            _healthTransform.gameObject.SetActive(isInYard);
            _nicknameTransform.SetBottom(nameTextHeight);
            _nicknameTransform.SetTop(-nameTextHeight);
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

        public void LookAtTeacher(bool isTeacher)
        {
            _playerVCam.gameObject.SetActive(!isTeacher);
            ArenaManager.Instance.TeacherVirtualCamera.gameObject.SetActive(isTeacher);
            _isAiming = !isTeacher;
            
            if (_animator.GetCurrentAnimatorStateInfo(1).IsName("Prepare Snowball"))
            {
                _animator.SetTrigger(CancelPrepare);
            }

            if (_hasSnowball && _playerSnowball)
            {
                _playerSnowball.DisableLineRenderer();
            }
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

        public void SetThrewSnowball(bool value)
        {
            _threwSnowball = value;
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
                stream.SendNext(_isFrozen);
                stream.SendNext(_worldUI.gameObject.activeSelf);
                stream.SendNext(_playerModel.gameObject.activeSelf);
                stream.SendNext(_canvasTransform.rotation);
                stream.SendNext(_healthTransform.gameObject.activeSelf);
                stream.SendNext(_nicknameTransform.offsetMax);
                stream.SendNext(_nicknameTransform.offsetMin);
                stream.SendNext(_isBeingControlled);
                stream.SendNext(_target);
            }
            else
            {
                _hasSnowball = (bool) stream.ReceiveNext();
                _isDigging = (bool) stream.ReceiveNext();
                _isWalking = (bool) stream.ReceiveNext();
                _isAiming = (bool) stream.ReceiveNext();
                _currentHealth = (int) stream.ReceiveNext();
                _isDead = (bool) stream.ReceiveNext();
                _isFrozen = (bool) stream.ReceiveNext();
                _worldUI.gameObject.SetActive((bool) stream.ReceiveNext());
                _playerModel.gameObject.SetActive((bool) stream.ReceiveNext());
                _canvasTransform.rotation = (Quaternion) stream.ReceiveNext();
                _healthTransform.gameObject.SetActive((bool) stream.ReceiveNext());
                _nicknameTransform.offsetMax = (Vector2) stream.ReceiveNext();
                _nicknameTransform.offsetMin = (Vector2) stream.ReceiveNext();
                _isBeingControlled = (bool) stream.ReceiveNext();
                _target = (Vector3) stream.ReceiveNext();
            }
        }

        #endregion

        #region PublicCalls
        public void SetControlledMovement(Vector3 target, bool startGame)
        {
            photonView.RPC(nameof(SyncIsControlled), RpcTarget.AllBuffered, true, target);
            _isBeingControlled = true;
            _target = target;
            _startGame = startGame;
        }

        private void StopControlledMovement()
        {
            _target = Vector3.zero;

            if (_startGame)
            {
                _startGame = false;
                RoomManager.Instance.StartGame();
            }
        }

        #endregion
    }
}