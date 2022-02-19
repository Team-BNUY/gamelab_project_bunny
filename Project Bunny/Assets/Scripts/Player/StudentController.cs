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
        
        [Header("Input")]
        [SerializeField] private float _movementSpeed;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;
        

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

        
        /// <summary>
        /// DO NOT CHANGE NAME: Translates 2D Vector input action into position coordinates in world space
        /// </summary>
        /// <param name="value"></param>
        public void OnMove(InputAction.CallbackContext value)
        {
            var inputMovement = value.ReadValue<Vector2>();
            _currentPosition = new Vector3(inputMovement.x, 0f, inputMovement.y);
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Translate mouse 2D Vector input action into angular rotation
        /// </summary>
        public void OnLook()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var rotation = MousePosToRotationInput(mousePosition);
            _currentRotation = Quaternion.Euler(0f, rotation, 0f);
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

        #endregion
    }
}