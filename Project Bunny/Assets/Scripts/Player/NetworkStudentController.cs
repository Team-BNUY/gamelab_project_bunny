using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

namespace Player
{
    public class NetworkStudentController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Components")]
        [SerializeField] private Transform _playerModel;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private CinemachineVirtualCamera _playerVirtualCamera;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private PlayerInput _playerInput;
        
        [Header("Input")]
        [SerializeField] private float _movementSpeed;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;

        [Header("Network")]
        private PhotonView _view;

        private void Awake()
        {
            if (_characterController == null)
            {
                _characterController = GetComponent<CharacterController>();
            }
            if (_view == null)
            {
                _view = GetComponent<PhotonView>();
            }
            if (_playerInput == null)
            {
                _playerInput = GetComponent<PlayerInput>();
            }

            _playerInput.actionEvents[0].AddListener(OnMove);
            _playerInput.actionEvents[1].AddListener(OnLook);
        }

        private void FixedUpdate()
        {
            if (!_view.IsMine) return;

            MoveStudent();
        }

        /// <summary>
        /// Attach unique instantiated camera with player
        /// TODO: Find a better system to attach instantiated camera to player
        /// </summary>
        /// <param name="cam"></param>
        public void SetCamera(GameObject cam)
        {
            _playerCamera = cam.transform.GetChild(0).GetComponent<Camera>();
            _playerVirtualCamera = cam.GetComponent<CinemachineVirtualCamera>();
            _playerVirtualCamera.Follow = gameObject.transform;
        }

        #region InputSystem

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
            if (!_view.IsMine) return;
            var inputMovement = value.ReadValue<Vector2>();
            _currentPosition = new Vector3(inputMovement.x, 0f, inputMovement.y);
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Translate mouse 2D Vector input action into angular rotation
        /// </summary>
        public void OnLook(InputAction.CallbackContext value)
        {
            if (!_view.IsMine) return;
            var mousePosition = Mouse.current.position.ReadValue();
            var rotation = MousePosToRotationInput(mousePosition);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
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

        // ReSharper disable Unity.PerformanceAnalysis
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(_movementSpeed);
            }
            else
            {
                // Network player, receive data
                _movementSpeed = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}

