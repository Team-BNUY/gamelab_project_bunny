using UnityEngine;
using UnityEngine.InputSystem;
using Interfaces;
using Cinemachine;

namespace Player
{
    public class Cannon : MonoBehaviour, IInteractable
    {
        [Header("Components")] 
        [SerializeField] private GameObject _playerSeat;
        [SerializeField] private Transform _cannonBallSeat;

        [Header("Properties")]
        [SerializeField] private float _newCameraDistance;
        private GameObject _currentCannonBall;
        private bool _isActive;
        private GameObject _player;
        private StudentController _studentController;
        private Camera _playerCam;
        private CinemachineVirtualCamera _playerVirtualCamera;
        private CinemachineComponentBase _playerVCamSettings;
        private CharacterController _playerCharController;
        [SerializeField] [Min(0)] private float _rotationSpeed;

        [Header("Snowball")]
        // TODO: Dynamically instantiate and attach prefab from a Manager
        [SerializeField] private GameObject _cannonBallPrefab;
        [SerializeField] [Min(0)] private float _coolDownTime;
        [SerializeField] private float _minForce;
        [SerializeField] private float _maxForce;
        [SerializeField] [Range(0f, 2.0f)] private float _forceIncreaseTimeRate;
        private Snowball _playerSnowball;
        private float _throwForce;
        private bool _hasSnowball;
        private bool _isAiming;
        private float _coolDownTimer;

        #region Callbacks
        private void Awake()
        {
            _throwForce = _minForce;
        }
        
        private void Update()
        {
            if (!_isActive) return;
            
            RotateSlingShot();
            CannonBallUpdate();

            if (!_isAiming || !_hasSnowball) return;
            
            if (_throwForce <= _maxForce)
            {
                IncreaseThrowForce();
            }
            _playerSnowball.DrawTrajectory();
        }
        
        #endregion
        
        #region InterfaceMethods

        /// <summary>
        /// Slingshot's implementation of the Enter() method. Initializes key variables.
        /// </summary>
        public void Enter(StudentController currentStudentController)
        {
            //Initialize key variables
            _isActive = true;
            _studentController = currentStudentController;
            _player = _studentController.transform.gameObject;
            
            //Get all necessary Camera components of the player
            _playerCam = _studentController.GetPlayerCamera();
            _playerVirtualCamera = _studentController.GetVirtualCamera();
            _playerVCamSettings = _studentController.GetVirtualCameraComponentBase();
            
            //Setting the new distance of the player camera when assuming control of the Slingshot
            if (_playerVCamSettings is CinemachineFramingTransposer transposer)
            {
                transposer.m_CameraDistance = _newCameraDistance; 
            }
            
            //Disable player controller in order to set the player's position manually
            _playerCharController = _player.GetComponent<CharacterController>();
            _playerCharController.enabled = false;

            //If there is no cannonball already on the slingshot, then spawn one. 
            if (_currentCannonBall == null && _coolDownTimer<=0.0f)
            {
                SpawnCannonBall();
            }
        }

        /// <summary>
        /// Slingshot's implementation of the Exit() method. Restores variables to default/null.
        /// </summary>
        public void Exit()
        {
            //Restore key variables to null/default value
            _isActive = false;
            _player = null;
            _studentController = null;

            //Restoring the original camera distance of the player's camera when quitting control of Slingshot.
            if (_playerVCamSettings is CinemachineFramingTransposer transposer && _playerVCamSettings != null)
            {
                transposer.m_CameraDistance = 25; // your value
            }

            //Restore key variables to null/default value
            _playerVirtualCamera = null;
            _playerVCamSettings = null;
            _playerCam = null;
            _playerCharController.enabled = true;
            _playerCharController = null;
        }

        /// <summary>
        /// Slingshot's implementation of the Click() method
        /// </summary>
        public void Click()
        {
            //If a cannonball is loaded, then begin the process of shooting it. 
            if (_hasSnowball)
            {
                StartCannonBallThrow();
            }
        }

        /// <summary>
        /// Slingshot's implementation of the Release() method
        /// </summary>
        public void Release()
        {
            //If a snowball is loaded and the player is not currently aiming, throw the cannonball.
            if (_hasSnowball && _currentCannonBall != null && _isAiming)
            {
                ThrowSnowball();
            }
        }

        #endregion
        
        #region CannonBallLogic
        
        /// <summary>
        /// Start aiming the cannonball
        /// </summary>
        private void StartCannonBallThrow()
        {
            _isAiming = true;
        }

        /// <summary>
        /// Start the snowball timer and spawn a new snowball when the timer is up
        /// </summary>
        private void CannonBallUpdate()
        {
            //If there is no cannonball currently loaded, then increase the timer. 
            if (!_hasSnowball)
            {
                _coolDownTimer += Time.deltaTime;
            }
            
            //If the cooldown timer is up, then spawn a new cannonball. If not, return. 
            if (_coolDownTimer < _coolDownTime) return;

            SpawnCannonBall();
        }
        
        /// <summary>
        /// Rotate the Slingshot with variable speed towards the mouse cursor.
        /// </summary>
        private void RotateSlingShot()
        {
            //var mousePosAngle = Utilities.MousePosToRotationInput(this.transform, _playerCam);
            var finalRotation = _studentController.playerRotation * Quaternion.Euler(0f, 90f, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation , Time.deltaTime * _rotationSpeed);
            _player.transform.position = _playerSeat.transform.position;
        }

        /// <summary>
        /// Spawn a new cannonball and set values for relevant variables
        /// </summary>
        private void SpawnCannonBall()
        {
            var prefabToSpawn = _cannonBallPrefab;
            _currentCannonBall = Instantiate(prefabToSpawn, _cannonBallSeat.position, _cannonBallSeat.rotation, _cannonBallSeat);
            // TODO: Object pooling to avoid using GetComponent at Instantiation
            _playerSnowball = _currentCannonBall.GetComponent<Snowball>();
            _playerSnowball.SetSnowballThrower(_studentController);
            _hasSnowball = true;
            _coolDownTimer = 0f;
        }
        
        /// <summary>
        /// When aiming, increase the throw force of the cannonball
        /// </summary>
        private void IncreaseThrowForce()
        {
            _throwForce += Time.deltaTime * _forceIncreaseTimeRate;
            _playerSnowball.SetSnowballForce(_throwForce);
        }
        
        /// <summary>
        /// Throw the snowball and restore related variables to default values.
        /// </summary>
        private void ThrowSnowball()
        {
            if (_playerSnowball == null) return;
            
            _isAiming = false;
            _throwForce = _minForce;
            _playerSnowball.ThrowSnowball();
            _hasSnowball = false;
            _currentCannonBall = null;
            _playerSnowball = null;
        }
        
        #endregion

        #region Utilities

        #endregion
        
        

    }
}


