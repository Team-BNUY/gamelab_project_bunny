using UnityEngine;
using UnityEngine.InputSystem;
using Interfaces;

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
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private PlayerInput _playerInput;
        
        [Header("Movement")]
        [SerializeField] [Min(0)] private float _movementSpeed;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;
        

        [Header("Properties")]
        [SerializeField] private float _studentHealth;
        private GameObject _currentObjectInHand;
        private IInteractable _currentInteractable;

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
        private bool _isDigging;
        private bool _hasSnowball;
        private bool _isAiming;

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
        /// Change player's position and orientation in global axes
        /// </summary>
        private void MoveStudent()
        {
            // If the player's character controller is disabled, then don't move them. Otherwise, move them. 
            if (_characterController.enabled)
            {
                _characterController.Move(_currentPosition * (_movementSpeed * Time.deltaTime));
                
            }
            _playerModel.rotation = _currentRotation;
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

            var prefabToSpawn = _currentStandingGround == LayerMask.NameToLayer("Ground") ? _snowballPrefab : _iceballPrefab;
            _currentObjectInHand = Instantiate(prefabToSpawn, _playerHand.position, _playerHand.rotation, _playerHand);
            // TODO: Object pooling to avoid using GetComponent at Instantiation
            _playerSnowball = _currentObjectInHand.GetComponent<Snowball>();
            _playerSnowball.SetSnowballThrower(this);
            _hasSnowball = true;
            _isDigging = false;
            _digSnowballTimer = 0.0f;
        }

        private void IncreaseThrowForce()
        {
            _throwForce += Time.deltaTime * _forceIncreaseTimeRate;
            _playerSnowball.SetSnowballForce(_throwForce);
        }

        /// <summary>
        /// Throws snowball once activated
        /// </summary>
        private void ThrowSnowball()
        {
            if (_playerSnowball == null) return;
            
            _isAiming = false;
            _throwForce = _minForce;
            _playerSnowball.ThrowSnowball();
            _hasSnowball = false;
            _currentObjectInHand = null;
            _playerSnowball = null;
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
            var inputMovement = context.ReadValue<Vector2>();
            var gravity = Physics.gravity.y * Time.deltaTime * 100f;
            _currentPosition = new Vector3(inputMovement.x, 0f, inputMovement.y);
            _currentPosition.y += gravity;
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Translate mouse 2D Vector input action into angular rotation
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void OnLook()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var rotation = MousePosToRotationInput(mousePosition);
            _currentRotation = Quaternion.Euler(0f, rotation, 0f);
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
            
            //  Can't dig at surface with no ice or snow
            if (_currentStandingGround != LayerMask.NameToLayer("Ground") &&
                _currentStandingGround != LayerMask.NameToLayer("Ice")) return;
            
            // If action is being performed, start digging
            if (context.performed)
            {
                _isDigging = true;
            }
            // If digging is abruptly cancelled, cancel action and reset timer
            if (context.canceled)
            {
                _isDigging = false;
                _digSnowballTimer = 0.0f;
            }
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
                    ThrowSnowball();
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
                // TODO: Interact with other items here
                // When you press 'E', it checks to see if there is an
                // interactable nearby and if there is, assume control of it. 
                
                if (_currentInteractable == null)
                {
                    _currentInteractable = ReturnNearestInteractable();
                    _currentInteractable?.Enter();
                }
                else
                {
                    _currentInteractable.Exit();
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
        /// Utility function that returns the player camera
        /// </summary>
        /// <returns></returns>
        public Camera GetPlayerCamera()
        {
            return _playerCamera;
        }
        
        /// <summary>
        /// Utility function that uses mouse position to return angle between player and on-screen mouse pointer
        /// TODO: Put in Utilities script
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        private float MousePosToRotationInput(Vector2 mousePos)
        {
            var target = _playerModel.transform;
            if (_playerCamera is { })
            {
                var objectPos =  _playerCamera.WorldToScreenPoint(target.position);

                mousePos.x -= objectPos.x;
                mousePos.y -= objectPos.y;
            }

            var angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            return 90 - angle;
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

            //Loop through 3 nearest objects and check if any of them are interactables that implement IInteractable
            for (var i = 0; i < maxColliders; i++)
            {
                if (hitColliders[i].transform.gameObject.GetComponent<IInteractable>() != null)
                {
                    interactable = hitColliders[i].transform.gameObject.GetComponent<IInteractable>();
                }
            }
            
            return interactable;
        }
        
        #endregion
    }
}