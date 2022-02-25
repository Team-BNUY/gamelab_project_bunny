using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    
    [SelectionBase]
    public class StudentController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform _playerModel;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private PlayerInput _playerInput;
        
        [Header("Movement")]
        [SerializeField] [Min(0)] private float _movementSpeed;
        [SerializeField] [Range(0f, 3f)] private float _playerGravity;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;

        [Header("Snowball")]
        // TODO: Dynamically instantiate and attach prefab from a Manager
        [SerializeField] private GameObject _snowballPrefab;
        [SerializeField] private Transform _playerHand;
        [SerializeField] [Min(0)] private float _digSnowballMaxTime;
        [SerializeField, Range(0f, 5f)] private float _throwForce;
        private GameObject _snowballObject;
        private Snowball _playerSnowball;
        private float _digSnowballTimer;
        private bool _isDigging;
        private bool _hasSnowball;


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
        }

        private void FixedUpdate()
        {
            if (!_isDigging)
            {
                MoveStudent();
            }

            if (_hasSnowball)
            {
                _playerSnowball.DrawTrajectory();
            }

            DigSnowball();
        }
        
        /// <summary>
        /// Attach unique instantiated camera with player
        /// </summary>
        /// <param name="cam"></param>
        // ReSharper disable once UnusedMember.Global
        public void SetCamera(Camera cam)
        {
            _playerCamera = cam;
        }


        #region Actions

        /// <summary>
        /// Change player's position and orientation in global axes
        /// </summary>
        private void MoveStudent()
        {
            _characterController.Move(_currentPosition * (_movementSpeed * Time.deltaTime));
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

            _snowballObject = Instantiate(_snowballPrefab, _playerHand.position, _playerHand.rotation, _playerHand);
            // TODO: Object pooling to avoid using GetComponent at Instantiation
            _playerSnowball = _snowballObject.GetComponent<Snowball>();
            _playerSnowball.SetSnowballThrower(this);
            _playerSnowball.SetSnowballParams(_throwForce , Physics.gravity.y);
            _hasSnowball = true;
            _isDigging = false;
            _digSnowballTimer = 0.0f;
        }

        /// <summary>
        /// Throws snowball once activated
        /// </summary>
        private void ThrowSnowball()
        {
            if (_playerSnowball == null) return;
            
            _playerSnowball.ThrowSnowball(_throwForce);
            _hasSnowball = false;
            _snowballObject = null;
            _playerSnowball = null;
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
            var gravity = _playerGravity * Time.deltaTime * 100f;
            _currentPosition = new Vector3(inputMovement.x, 0f, inputMovement.y);
            _currentPosition.y -= gravity;
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
        /// DO NOT CHANGE NAME: Enables for digging process to occur
        /// </summary>
        /// <param name="context"></param>
        // ReSharper disable once UnusedMember.Global
        public void OnDig(InputAction.CallbackContext context)
        {
            if (_hasSnowball || !_characterController.isGrounded) return;
            
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
        /// DO NOT CHANGE NAME: Activates the throwing of the snowball
        /// </summary>
        /// <param name="context"></param>
        // ReSharper disable once UnusedMember.Global
        public void OnThrow(InputAction.CallbackContext context)
        {
            if (!_hasSnowball) return;

            if (context.performed)
            {
                ThrowSnowball();
            }
        }

        #endregion
        
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
    }
}