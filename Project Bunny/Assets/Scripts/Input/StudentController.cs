using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class StudentController : MonoBehaviour
    {
        public bool hasSomethingEquipped;

        public void EquipUnequipSomething()
        {
            hasSomethingEquipped = !hasSomethingEquipped;
        }
        
        [Header("Input")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _movementSpeed;
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

        public void OnLook()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var rotation = MousePosToRotationInput(mousePosition);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }
        
        private float MousePosToRotationInput(Vector2 mousePos)
        {
            var target = transform;
            if (Camera.main is { })
            {
                Vector3 objectPos = Camera.main.WorldToScreenPoint(target.position);

                mousePos.x = mousePos.x - objectPos.x;
                mousePos.y = mousePos.y - objectPos.y;
            }

            var angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            return 90 - angle;
        }
        
        #endregion
    }
}
