using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Cinemachine;


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

        [Header("Camera")]

        [SerializeField] 
        [Range(0.0f, 90f)] private float _cameraAngle;

        
        private Vector3 _currentPosition;



        private void Awake()
        {
            if (_characterController == null)
            {
                _characterController = gameObject.GetComponent<CharacterController>();
            }
            
            if(_playerCamera != null)
            {
                _playerCamera.transform.eulerAngles = new Vector3(cameraAngle,0,0);
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

        public void RotatePlayer()
        {
            var gamepad = Gamepad.current;
            if(gamepad == null)
            {
                var mousePosition = Mouse.current.position.ReadValue();
                var rotation = MousePosToRotationInput(mousePosition);
                Debug.Log(rotation);
                _playerModel.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            }
            else
            {
                if (_playerCamera is { })
                {
                    Vector2 inputRotationR = gamepad.rightStick.ReadValue();
                    Vector2 inputRotationL = gamepad.leftStick.ReadValue();

                    Quaternion finalRotation = Quaternion.identity;
                    
                    if(inputRotationR.magnitude > 0.1f)
                    {
                        Vector3 stickDirection = new Vector3(_playerCamera.transform.position.x, 0, _playerCamera.transform.position.y) + (new Vector3(inputRotationR.x, 0, inputRotationR.y) * 500f);
                        Vector3 stickPosition = new Vector3(_characterController.transform.position.x, 0, _characterController.transform.position.y) + stickDirection;
                        finalRotation = Quaternion.LookRotation(stickPosition);
                    }
                    else
                    {
                        if(inputRotationL.magnitude > 0.1f)
                        {
                            Vector3 stickDirection = _playerCamera.transform.position + (new Vector3(inputRotationL.x, 0, inputRotationL.y) * 500f);
                            Vector3 stickPosition = _characterController.transform.position + stickDirection;
                            finalRotation = Quaternion.LookRotation(stickPosition);
                        }
                        
                    }

                    if(inputRotationR.magnitude > 0.1f || inputRotationL.magnitude > 0.1f)
                    _playerModel.transform.rotation = Quaternion.RotateTowards(_playerModel.transform.rotation, finalRotation, turnRate * Time.deltaTime);
                }
            }
            
        }
        
        private float MousePosToRotationInput(Vector2 mousePos)
        {
            var target = transform;
            if (_playerCamera is { })
            {
                Vector3 objectPos = _playerCamera.WorldToScreenPoint(target.position);

                mousePos.x = mousePos.x - objectPos.x;
                mousePos.y = mousePos.y - objectPos.y;
            }

            var angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            return 90 - angle;
            
        }
        
        #endregion
    }
}
