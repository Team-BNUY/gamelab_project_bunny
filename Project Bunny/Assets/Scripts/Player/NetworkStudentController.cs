using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

namespace Player
{
    public class NetworkStudentController : MonoBehaviourPunCallbacks
    {
        [Header("Components")]
        [SerializeField] private Transform _playerModel;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private PlayerInput _playerInput;
        private Camera _playerCamera;

        [Header("Movement")]
        [SerializeField] [Min(0)] private float _movementSpeed;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;

        [Header("Snowball")]
        // TODO: Dynamically instantiate and attach prefab from a Manager
        [SerializeField] private GameObject _snowballPrefab;
        [SerializeField] private Transform _playerHand;
        [SerializeField] [Min(0)] private float _digSnowballMaxTime;
        // ReSharper disable once NotAccessedField.Local
        private GameObject _playerSnowball;
        private float _digSnowballTimer;
        private bool _isDigging;
        private bool _hasSnowball;

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

            //Test Case
            //TODO: Add whatever UI we'll have for player name, health, whatever
            Debug.Log(PhotonNetwork.NickName);
        }

        private void FixedUpdate()
        {
            if (!_view.IsMine) return;

            if (!_isDigging)
            {
                MoveStudent();
            }
            DigSnowball();
        }

        /// <summary>
        /// Attach unique instantiated camera with player
        /// TODO: Find a better system to attach instantiated camera to player
        /// </summary>
        /// <param name="cam"></param>
        public void SetCamera(GameObject playerCamera)
        {
            if (!_view.IsMine) return;

            if (_playerCamera != null)
            {
                //TODO: object pooling
                _playerCamera = playerCamera.transform.GetChild(0).GetComponent<Camera>();
                playerCamera.GetComponent<CinemachineVirtualCamera>().Follow = gameObject.transform;
            }
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

        private void DigSnowball()
        {
            if (_isDigging && !_hasSnowball)
            {
                _digSnowballTimer += Time.deltaTime;
            }

            if (_digSnowballTimer < _digSnowballMaxTime) return;

            _playerSnowball = Instantiate(_snowballPrefab, _playerHand.position, _playerHand.rotation, _playerHand);
            _hasSnowball = true;
            _isDigging = false;
            _digSnowballTimer = 0.0f;
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
            _currentPosition = new Vector3(inputMovement.x, 0f, inputMovement.y);
        }

        /// <summary>
        /// DO NOT CHANGE NAME: Translate mouse 2D Vector input action into angular rotation
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public void OnLook(InputAction.CallbackContext arg)
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
            if (_hasSnowball) return;

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
                var objectPos = _playerCamera.WorldToScreenPoint(target.position);

                mousePos.x -= objectPos.x;
                mousePos.y -= objectPos.y;
            }

            var angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            return 90 - angle;
        }
    }
}