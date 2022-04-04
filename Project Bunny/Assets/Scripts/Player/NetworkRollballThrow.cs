using UnityEngine;
using Cinemachine;
using Interfaces;
using Photon.Pun;

namespace Player
{
    public class NetworkRollballThrow : MonoBehaviour, INetworkInteractable
    {
        [Header("Components")]
        [SerializeField] private Transform _playerSeat;
        [SerializeField] private Transform _rollballSeat;

        [Header("Properties")]
        [SerializeField] private float _newCameraDistance;
        private GameObject _currentRollball;
        private bool _isActive;
        private GameObject _player;
        private NetworkStudentController _currentStudentController;
        private CinemachineFramingTransposer _playerVCamSettings;
        private CharacterController _playerCharController;
        [SerializeField] [Min(0)] private float _rotationSpeed;

        [Header("Snowball")]
        [SerializeField] [Min(0)] private float _coolDownTime;
        private NetworkGiantRollball _playerRollball;
        private float _coolDownTimer;

        public bool IsActive => _isActive;

        #region Callbacks

        private void Start()
        {
            SpawnRollBall();
        }
        
        private void Update()
        {
            CannonRollBallUpdate();
            
            if (!_isActive) return;

            RotateRollballAiming();
        }

        #endregion

        #region InterfaceMethods

        /// <summary>
        /// Slingshot's implementation of the Enter() method. Initializes key variables.
        /// </summary>
        public void Enter(NetworkStudentController currentStudentController)
        {
            if (_currentRollball == null || currentStudentController.HasSnowball) return;
            
            //Initialize key variables
            _isActive = true;
            _currentStudentController = currentStudentController;
            _currentStudentController.UsingCannon = true;
            _player = _currentStudentController.transform.gameObject;

            //Setting the new distance of the player camera when assuming control of the Slingshot
            _playerVCamSettings = _currentStudentController.PlayerVCamFramingTransposer;
            _playerVCamSettings.m_CameraDistance = _newCameraDistance;

            //Disable player controller in order to set the player's position manually
            _playerCharController = _currentStudentController.CharacterControllerComponent;
            _playerCharController.enabled = false;
        }

        /// <summary>
        /// Slingshot's implementation of the Exit() method. Restores variables to default/null.
        /// </summary>
        public void Exit()
        {
            if (!_currentStudentController) return;
            
            // If already aiming while exiting, then just throw the current snowball and restore everything
            _coolDownTimer = 0.0f;

            //Restore key variables to null/default value
            _isActive = false;
            _player = null;
            _currentStudentController.UsingCannon = false;
            _currentStudentController = null;

            //Restoring the original camera distance of the player's camera when quitting control of Slingshot.
            _playerVCamSettings.m_CameraDistance = 25;

            //Restore key variables to null/default value
            _playerVCamSettings = null;
            _playerCharController.enabled = true;
            _playerCharController = null;
        }

        /// <summary>
        /// Slingshot's implementation of the Click() method
        /// </summary>
        public void Click()
        {
            if (_isActive && _currentStudentController && _currentRollball)
            {
                _isActive = false;
                ThrowSnowball();
                _player = null;
                _currentStudentController.CurrentInteractable = null;
                _currentStudentController.UsingCannon = false;
                _currentStudentController = null;
                
                //Restoring the original camera distance of the player's camera when quitting control of Slingshot.
                _playerVCamSettings.m_CameraDistance = 25;

                //Restore key variables to null/default value
                _playerVCamSettings = null;
                _playerCharController.enabled = true;
                _playerCharController = null;
                _coolDownTimer = 0f;
            }
        }

        /// <summary>
        /// Slingshot's implementation of the Release() method
        /// </summary>
        public void Release()
        {

        }

        #endregion

        #region CannonBallLogic

        /// <summary>
        /// Start the snowball timer and spawn a new snowball when the timer is up
        /// </summary>
        private void CannonRollBallUpdate()
        {
            if (_playerRollball) return;
            
            //If there is no cannonball currently loaded, then increase the timer. 
            _coolDownTimer += Time.deltaTime;

            //If the cooldown timer is up, then spawn a new cannonball. If not, return. 
            if (_coolDownTimer < _coolDownTime) return;
            
            SpawnRollBall();
        }

        /// <summary>
        /// Rotate the Slingshot with variable speed towards the mouse cursor.
        /// </summary>
        private void RotateRollballAiming()
        {
            //var mousePosAngle = Utilities.MousePosToRotationInput(this.transform, _playerCam);
            var finalRotation = _currentStudentController.PlayerRotation * Quaternion.Euler(0f, 90f, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, Time.deltaTime * _rotationSpeed);
            _player.transform.position = _playerSeat.position;
            _currentStudentController.SetPlayerRotation(_playerSeat.rotation * Quaternion.Euler(0f, -90f, 0f));
        }

        /// <summary>
        /// Spawn a new cannonball and set values for relevant variables
        /// </summary>
        private void SpawnRollBall()
        {
            _currentRollball = PhotonNetwork.Instantiate(ArenaManager.Instance.GiantRollballPrefab.name, _rollballSeat.position, _rollballSeat.rotation);
            _playerRollball = _currentRollball.GetComponent<NetworkGiantRollball>();
        }

        /// <summary>
        /// Throw the snowball and restore related variables to default values.
        /// </summary>
        private void ThrowSnowball()
        {
            if (_playerRollball)
            {
                _playerRollball.PushGiantRollball(_currentStudentController.transform);
            }
            
            _currentRollball = null;
            _playerRollball = null;
        }

        #endregion
    }
}