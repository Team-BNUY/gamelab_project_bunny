using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

namespace Player
{
    public class NetworkStudentController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Input")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _movementSpeed;

        private PhotonView view;
        private Vector3 _currentPosition;

        private void Awake()
        {
            if (_characterController == null)
            {
                _characterController = GetComponent<CharacterController>();
            }

            if (view == null)
            {
                view = GetComponent<PhotonView>();
            }

            GetComponent<PlayerInput>().actionEvents[0].AddListener(OnMove);
            GetComponent<PlayerInput>().actionEvents[1].AddListener(OnLook);
        }

        private void FixedUpdate()
        {
            if (!view.IsMine) return;

            MoveStudent();
        }

        #region InputSystem

        private void MoveStudent()
        {
            _characterController.Move(_currentPosition * (_movementSpeed * Time.deltaTime));
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            if (!view.IsMine) return;
            var inputMovement = value.ReadValue<Vector2>();
            _currentPosition = new Vector3(inputMovement.x, 0f, inputMovement.y);
        }

        public void OnLook(InputAction.CallbackContext value)
        {
            if (!view.IsMine) return;
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this._movementSpeed);
            }
            else
            {
                // Network player, receive data
                this._movementSpeed = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}

