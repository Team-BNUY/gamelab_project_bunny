using UnityEngine;
using UnityEngine.InputSystem;


namespace Input
{
    public class StudentController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private GameObject _playerModel;

        [Header("Player")]
        [SerializeField] private float _turnRate;
 
        private Vector3 _currentPosition;



        private void Awake()
        {
            if (_characterController == null)
            {
                _characterController = gameObject.GetComponent<CharacterController>();
            }
        }

        private void FixedUpdate()
        {
            MoveStudent();
            RotatePlayer();
        }

        #region InputSystem
        
        private void MoveStudent()
        {
            _characterController.Move(_currentPosition * (_movementSpeed * Time.deltaTime));
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            var inputMovement = value.ReadValue<Vector2>();
            _currentPosition = new Vector3(inputMovement.x, 0f, inputMovement.y);
        }

        private void RotatePlayer()
        {
            var gamepad = Gamepad.current;
            if(gamepad == null)
            {
                var mousePosition = Mouse.current.position.ReadValue();
                var rotation = MousePosToRotationInput(mousePosition);
                
                _playerModel.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            }
            else
            {
                if (_playerCamera is null) return;
                var inputRotationR = gamepad.rightStick.ReadValue();
                var inputRotationL = gamepad.leftStick.ReadValue();

                var finalRotation = Quaternion.identity;
                    
                if(inputRotationR.magnitude > 0.1f)
                {
                    var stickDirection = new Vector3(_playerCamera.transform.position.x, 0, _playerCamera.transform.position.y) + (new Vector3(inputRotationR.x, 0, inputRotationR.y) * 500f);
                    var stickPosition = new Vector3(_characterController.transform.position.x, 0, _characterController.transform.position.y) + stickDirection;
                    finalRotation = Quaternion.LookRotation(stickPosition);
                }
                else
                {
                    if(inputRotationL.magnitude > 0.1f)
                    {
                        var stickDirection = _playerCamera.transform.position + (new Vector3(inputRotationL.x, 0, inputRotationL.y) * 500f);
                        var stickPosition = _characterController.transform.position + stickDirection;
                        finalRotation = Quaternion.LookRotation(stickPosition);
                    }
                        
                }

                if((inputRotationR.magnitude > 0.1f) || (inputRotationL.magnitude > 0.1f))
                {
                    _playerModel.transform.rotation = Quaternion.RotateTowards(_playerModel.transform.rotation, finalRotation, _turnRate * Time.deltaTime);
                }
            }
            
        }
        
        private float MousePosToRotationInput(Vector2 mousePos)
        {
            var target = transform;
            if (_playerCamera is { })
            {
                var objectPos = _playerCamera.WorldToScreenPoint(target.position);

                mousePos.x = mousePos.x - objectPos.x;
                mousePos.y = mousePos.y - objectPos.y;
            }

            var angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            return 90 - angle;
        }
        
        #endregion
    }
}
