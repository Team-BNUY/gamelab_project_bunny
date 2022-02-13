using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class StudentController : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        
        private Vector3 _currentMovement;
        
        public void OnMove(InputValue input)
        {
            Vector2 inputVec = input.Get<Vector2>();
            _currentMovement = new Vector3(inputVec.x, 0, inputVec.y);
            _characterController.Move(_currentMovement);
        }
    }
}
