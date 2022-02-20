using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class StudentController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform _playerModel;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private PlayerInput _playerInput;
        
        [Header("Movement")]
        [SerializeField] private float _movementSpeed;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;

        [Header("Snowball")]
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
                _playerInput = GetComponent<PlayerInput>();
            }
        }

        private void FixedUpdate()
        {
            MoveStudent();
            DigSnowball();
        }
        
        /// <summary>
        /// Attach unique instantiated camera with player
        /// </summary>
        /// <param name="cam"></param>
        public void SetCamera(Camera cam)
        {
            _playerCamera = cam;
        }
        
        #region InputSystem

        
        /// <summary>
        /// Change player's position and orientation in global axes
        /// </summary>
        private void MoveStudent()
        {
            _characterController.Move(_currentPosition * (_movementSpeed * Time.deltaTime));
            _playerModel.rotation = _currentRotation;
        }

        private void DigSnowball()
        {
            if (_isDigging)
            {
                Debug.Log("Digging");
            }
        }

        
        /// <summary>
        /// DO NOT CHANGE NAME: Translates 2D Vector input action into position coordinates in world space
        /// </summary>
        /// <param name="context"></param>
        // ReSharper disable once UnusedMember.Global
        public void OnMove(InputAction.CallbackContext context)
        {
            var inputMovement = context.ReadValue<Vector2>();
            _currentPosition = new Vector3(inputMovement.x, 0f, inputMovement.y);
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
            if (context.performed)
            {
                _isDigging = true;
            }
            if (context.canceled)
            {
                _isDigging = false;
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