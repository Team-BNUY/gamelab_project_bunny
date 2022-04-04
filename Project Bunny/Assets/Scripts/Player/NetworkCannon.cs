using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using Interfaces;
using Networking;
using Photon.Pun;

namespace Player
{
    public class NetworkCannon : MonoBehaviour, INetworkInteractable
    {
        [Header("Components")]
        [SerializeField] private Transform _playerSeat;
        [SerializeField] private Transform _cannonBallSeat;
        [SerializeField] private GameObject _aimArrow;
        public Transform CannonBallSeat => _cannonBallSeat;

        [Header("Properties")]
        [SerializeField] private float _newCameraDistance;
        private GameObject _cannonBallObject;
        private bool _isActive;
        private GameObject _player;
        private NetworkStudentController _currentStudentController;
        private CinemachineFramingTransposer _playerVCamSettings;
        private CharacterController _playerCharController;
        [SerializeField] [Min(0)] private float _rotationSpeed;

        [Header("Snowball")]
        [SerializeField] [Min(0)] private float _coolDownTime;
        [SerializeField] private float _minForce, _maxForce;
        [SerializeField] [Range(0f, 2.0f)] private float _forceIncreaseTimeRate;
        [SerializeField] private float _minAngle, _maxAngle;
        [SerializeField] [Range(0f, 8.0f)] private float _angleIncreaseTimeRate;
        private List<NetworkSnowball> _cannonballCollection = new List<NetworkSnowball>();
        private float _throwForce;
        private float _throwAngle;
        private bool _hasSnowball, _isAiming;
        private float _coolDownTimer;
        
        public bool IsActive => _isActive;

        #region Callbacks
        private void Awake()
        {
            _throwForce = _minForce;
            _throwAngle = _minAngle;
        }

        private void Update()
        {
            if (!_isActive) return;

            RotateSlingShot();
            CannonBallUpdate();

            if (!_hasSnowball) return;

            if (_hasSnowball)
            {
                _cannonballCollection[0].DrawTrajectory();
                IncreaseThrowForce();
                IncreaseThrowAngle();
            }
        }

        #endregion

        #region InterfaceMethods

        /// <summary>
        /// Slingshot's implementation of the Enter() method. Initializes key variables.
        /// </summary>
        public void Enter(NetworkStudentController currentStudentController)
        {
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
            
            if (_currentStudentController.photonView.IsMine)
            {
                _aimArrow.SetActive(true);
            }

            //If there is no cannonball already on the slingshot, then spawn one. 
            if (_cannonBallObject == null && _coolDownTimer >= _coolDownTime)
            {
                SpawnCannonBall();
            }
        }

        /// <summary>
        /// Slingshot's implementation of the Exit() method. Restores variables to default/null.
        /// </summary>
        public void Exit()
        {
            if (_currentStudentController.photonView.IsMine)
            {
                _aimArrow.SetActive(false);
            }
            
            // If already aiming while exiting, then just throw the current snowball and restore everything
            StartCannonBallThrow();
            ThrowSnowball();
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
            //If a cannonball is loaded, then begin the process of shooting it. 
            if (_hasSnowball)
            {
                //StartCannonBallThrow();
                ThrowSnowball();
                _hasSnowball = false;
            }
        }

        /// <summary>
        /// Slingshot's implementation of the Release() method
        /// </summary>
        public void Release()
        {
            //If a snowball is loaded and the player is not currently aiming, throw the cannonball.
            //if (!_hasSnowball || _currentCannonBall == null || !_isAiming) return;
            
            //_isAiming = false;
            //ThrowSnowball();
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
            var finalRotation = _currentStudentController.PlayerRotation * Quaternion.Euler(0f, 90f, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, Time.deltaTime * _rotationSpeed);
            _player.transform.position = _playerSeat.position;
            _currentStudentController.SetPlayerRotation(_playerSeat.rotation * Quaternion.Euler(0f, -90f, 0f));
        }

        /// <summary>
        /// Spawn a new cannonball and set values for relevant variables
        /// </summary>
        private void SpawnCannonBall()
        {
            _cannonBallObject = PhotonNetwork.Instantiate(ArenaManager.Instance.CannonBall.name, _cannonBallSeat.position, _cannonBallSeat.rotation);
            _cannonBallObject.transform.parent = _cannonBallSeat;
            // TODO: Object pooling to avoid using GetComponent at Instantiation
            _cannonballCollection = _cannonBallObject.GetComponentsInChildren<NetworkSnowball>().ToList();
            _cannonballCollection.ForEach(c => c.SetSnowballThrower(_currentStudentController));
            _hasSnowball = true;
            _coolDownTimer = 0f;
        }

        /// <summary>
        /// When aiming, increase the throw force of the cannonball
        /// </summary>
        private void IncreaseThrowForce()
        {
            //_throwForce += Time.deltaTime * _forceIncreaseTimeRate;
            _throwForce = _maxForce;
            _cannonballCollection.ForEach(c => c.SetSnowballForce(_throwForce));
        }
        
        private void IncreaseThrowAngle()
        {
            //_throwAngle += Time.deltaTime * _angleIncreaseTimeRate;
            _throwAngle = _maxAngle;
            _cannonballCollection.ForEach(c => c.SetSnowballAngle(_throwAngle));
        }

        /// <summary>
        /// Throw the snowball and restore related variables to default values.
        /// </summary>
        private void ThrowSnowball()
        {
            _isAiming = false;
            _throwForce = _minForce;
            _throwAngle = _minAngle;
            _hasSnowball = false;

            if (_cannonballCollection != null)
            {
                foreach (var cannonBall in _cannonballCollection)
                {
                    cannonBall.SetHoldingPlace(_cannonBallSeat);
                    cannonBall.transform.parent = null;
                    cannonBall.ThrowBurstSnowballs(_minForce, _maxForce, _minAngle, _maxAngle);
                }
            }
            
            PhotonNetwork.Destroy(_cannonBallObject);
            _cannonBallObject = null;
            _cannonballCollection?.Clear();
            _cannonballCollection = null;
        }

        #endregion
    }
}