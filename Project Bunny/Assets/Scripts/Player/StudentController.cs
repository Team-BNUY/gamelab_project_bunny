using Cinemachine;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [SelectionBase]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class StudentController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform _playerModel;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private CinemachineVirtualCamera _playerVCam;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Animator _animator;
        private CinemachineComponentBase _playerVCamComponentBase;

        [Header("Movement")]
        [SerializeField] [Min(0)] private float _movementSpeed;
        [SerializeField] [Range(0f, 1f)] private float _movementFriction;
        private Vector3 _playerCurrentVelocity;
        private Vector3 _playerPosition;
        private Quaternion _playerRotation;
        private Vector2 _inputMovement;
        private Vector2 _deltaVector;
        
        [Header("Properties")]
        [SerializeField] private float _studentHealth;
        private GameObject _currentObjectInHand;
        private IInteractable _currentInteractable;
        private bool _isSliding;

        [Header("Snowball")]
        // TODO: Dynamically instantiate and attach prefab from a Manager
        [SerializeField] private GameObject _snowballPrefab;
        [SerializeField] private GameObject _iceballPrefab;
        [SerializeField] private Transform _playerHand;
        [SerializeField] [Min(0)] private float _digSnowballMaxTime;
        [SerializeField] private float _minForce;
        [SerializeField] private float _maxForce;
        [SerializeField] [Range(0f, 2.0f)] private float _forceIncreaseTimeRate;
        private Snowball _playerSnowball;
        private int _currentStandingGround;
        private float _throwForce;
        private float _digSnowballTimer;
        private bool _isAiming;

        [Header("Booleans")]
        private bool _isWalking;
        private bool _isDigging;
        private bool _hasSnowball;
        // List of readonly files. No need for them to have a _ prefix
        private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
        private static readonly int IsDiggingHash = Animator.StringToHash("isDigging");
        private static readonly int HasSnowballHash = Animator.StringToHash("hasSnowball");
        private static readonly int DeltaX = Animator.StringToHash("deltaX");
        private static readonly int DeltaY = Animator.StringToHash("deltaY");

        public Quaternion PlayerRotation => _playerRotation;
        public Transform PlayerHand => _playerHand;

        #region Callbacks
        
        private void Awake()
        {
            if (_characterController == null)
            {
                _characterController = gameObject.GetComponent<CharacterController>();
            }
            if (_playerInput == null)
            {
                _playerInput = gameObject.GetComponent<PlayerInput>();
            }
            if (_animator == null)
            {
                _animator = gameObject.GetComponent<Animator>();
            }

            _playerVCamComponentBase = _playerVCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        }

        private void Start()
        {
            _throwForce = _minForce;
        }

        private void Update()
        {
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

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.gameObject.TryGetComponent<GiantRollball>(out var giantRollball))
            {
                giantRollball.PushGiantRollball(transform);
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
        /// Activates Digging Stopwatch and creates snowball on hand once digging complete
        /// </summary>
        private void DigSnowball()
        {
            if (_isDigging && !_hasSnowball)
            {
                _digSnowballTimer += Time.deltaTime;
            }

            if (_digSnowballTimer < _digSnowballMaxTime) return;

            var prefabToSpawn = _snowballPrefab;
            if (_currentStandingGround == LayerMask.NameToLayer("Ice"))
            {
                prefabToSpawn = _iceballPrefab;
            }
            _currentObjectInHand = Instantiate(prefabToSpawn, _playerHand.position, _playerHand.rotation * Quaternion.Euler(0f, -90f, 0f));
            //_currentObjectInHand.transform.parent = _playerHand;
            // TODO: Object pooling to avoid using GetComponent at Instantiation
            _playerSnowball = _currentObjectInHand.GetComponent<Snowball>();
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
            if (_playerSnowball == null) return;
            
            _playerSnowball.DisableLineRenderer();
            _throwForce = _minForce;
            _playerSnowball.ThrowSnowball();
            _hasSnowball = false;
            _currentObjectInHand = null;
            _playerSnowball = null;
            _animator.SetBool(HasSnowballHash, false);
        }

        /// <summary>
        /// Student gets damaged when snowball or combat item is thrown at them successfully
        /// </summary>
        /// <param name="damage"></param>
        public void GetDamaged(float damage)
        {
            if (damage >= _studentHealth)
            {
                _studentHealth = 0.0f;
            }
            else
            {
                _studentHealth -= damage;
            }
        }

        /// <summary>
        /// Track the layer of the object the player is standing on
        /// </summary>
        private void SetStandingGround()
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out var hit, 1.0f))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
                _currentStandingGround = hit.collider.gameObject.layer;
                if (!_isSliding)
                {
                    _playerCurrentVelocity = _characterController.velocity;
                }
                _isSliding = _currentStandingGround == LayerMask.NameToLayer("Ice");
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
            if (_isWalking && _inputMovement.magnitude == 0.0f)
            {
                _animator.SetBool(IsWalkingHash, false);
            }
            else if (!_isWalking && _inputMovement.magnitude > 0.0f)
            {
                _animator.SetBool(IsWalkingHash, true);
            }
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Translate mouse 2D Vector input action into angular rotation
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void OnLook()
        {
            var mousePosAngle = Utilities.MousePosToRotationInput(transform, _playerCamera);
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
            if (_currentInteractable != null) return;
            
            // If player has snowball on hand or character is in the air, don't dig
            // TODO: Make it also so it can't dig when hand is occupied in general
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
                _animator.SetBool(IsWalkingHash, false);
                _animator.SetBool(IsDiggingHash, true);
            }
            // If digging is abruptly cancelled, cancel action and reset timer
            if (context.canceled)
            {
                _isDigging = false;
                _digSnowballTimer = 0.0f;
                _animator.SetBool(IsDiggingHash, false);
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
                if (_hasSnowball)
                {
                    _playerSnowball.DisableLineRenderer();
                    _isAiming = false;
                    _animator.SetTrigger("ThrowSnowball");
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

        
        #region Utilities

        /// <summary>
        /// Attach unique instantiated camera with player
        /// </summary>
        /// <param name="cam"></param>
        // ReSharper disable once UnusedMember.Global
        public void SetCamera(Camera cam)
        {
            _playerCamera = cam;
        }

        /// <summary>
        /// Getter function to get the player's Virtual Camera
        /// </summary>
        /// <returns></returns>
        public CinemachineVirtualCamera GetVirtualCamera()
        {
            return _playerVCam;
        }
        
        /// <summary>
        /// Getter function to get the CinemachineComponentBase of the player's Virtual Camera
        /// </summary>
        /// <returns></returns>
        public CinemachineComponentBase GetVirtualCameraComponentBase()
        {
            return _playerVCamComponentBase;
        }

        public CharacterController GetPlayerCharacterController()
        {
            return _characterController;
        }

        /// <summary>
        /// Utility function that returns the nearest interactable to the player
        /// </summary>
        /// <returns></returns>
        private IInteractable ReturnNearestInteractable() 
        {
            IInteractable interactable = null;
            var maxColliders = 3; //maximum number of objects near to the player that can be looped through
            var hitColliders = new Collider[maxColliders];
            var numColliders = Physics.OverlapSphereNonAlloc(transform.position, 1f, hitColliders);

            if (numColliders < 1) return null;
            
            //Loop through 3 nearest objects and check if any of them are interactables that implement IInteractable
            for (var i = 0; i < numColliders; i++)
            {
                if (hitColliders[i].gameObject.TryGetComponent<IInteractable>(out var interactableObject))
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
            _animator.SetBool(IsWalkingHash, _isWalking);
        }
        
        #endregion
    }
}