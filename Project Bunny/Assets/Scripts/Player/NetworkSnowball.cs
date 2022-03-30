using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Player;
using UnityEngine;

namespace Networking
{
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkSnowball : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] private Rigidbody _snowballRigidbody;
        [SerializeField] private SphereCollider _sphereCollider;
        [SerializeField] private Transform _snowballTransform;
        [SerializeField] private LineRenderer _trajectoryLineRenderer;
        [SerializeField, Min(0)] private float _initialDirection;
        [SerializeField] private int _damage;
    
        private bool _isDestroyable;
        private float _throwForce;
        private float _mass;
        private float _initialVelocity;
        private readonly float _collisionCheckRadius = 0.03f;

        private NetworkStudentController _studentThrower;
        private Transform _holdingPlace;
        private PhotonView _view;
        private bool _hasCollided;

        private void Awake()
        {
            _snowballTransform ??= transform;
            _view ??= GetComponent<PhotonView>();
            _snowballRigidbody ??= gameObject.GetComponent<Rigidbody>();
            _trajectoryLineRenderer ??= GetComponent<LineRenderer>();
            _sphereCollider ??= GetComponent<SphereCollider>();

            _mass = _snowballRigidbody.mass;
            _trajectoryLineRenderer.enabled = false;
            _hasCollided = false;
        }

        private void Update()
        {
            SetSnowballAtPlace();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_view.IsMine) return;
            if (!_isDestroyable) return;

            if (other.gameObject.TryGetComponent<NetworkStudentController>(out var otherStudent)
                && otherStudent != _studentThrower
                && !otherStudent.TeamID.Equals(_studentThrower.TeamID)
                && !otherStudent.IsDead)
            {
                if (_hasCollided) return;
                _hasCollided = true;
                
                otherStudent.GetDamaged(_damage);
            }

            if (otherStudent == _studentThrower) return;
            
            var go = PhotonNetwork.Instantiate(ArenaManager.Instance.SnowballBurst.name, _snowballTransform.position, Quaternion.identity);
            go.transform.rotation = Quaternion.LookRotation(other.contacts[0].normal);
            go.GetComponent<ParticleSystem>().Play();
            PhotonNetwork.Destroy(gameObject);
        }

        /// <summary>
        /// Instead of parenting snowball at hand,
        /// track position of student's hand and rotation of root of player
        /// </summary>
        private void SetSnowballAtPlace()
        {
            if (_isDestroyable) return;
            if (_holdingPlace == null) return;

            if (_holdingPlace.gameObject.TryGetComponent<NetworkStudentController>(out var student))
            {
                _snowballTransform.position = student.PlayerHand.position;
                _snowballTransform.rotation = student.PlayerRotation;
            }
            else if (_holdingPlace.gameObject.TryGetComponent<NetworkCannon>(out var cannon))
            {
                _snowballTransform.position = cannon.CannonBallSeat.position;
                _snowballTransform.rotation = cannon.CannonBallSeat.rotation;
            }
            else
            {
                _snowballTransform.position = _holdingPlace.position;
                _snowballTransform.rotation = _holdingPlace.rotation;
            }
        }

        /// <summary>
        /// Set Snowball Force through StudentController.cs
        /// </summary>
        /// <param name="force"></param>
        public void SetSnowballForce(float force)
        {
            _throwForce = force * 1000f;
        }

        /// <summary>
        /// Draw the predictive trajectory using the Simulation Arc method
        /// </summary>
        public void DrawTrajectory()
        {
            _trajectoryLineRenderer.enabled = true;
            _trajectoryLineRenderer.positionCount = SimulateArc().Count;
            for (var index = 0; index < _trajectoryLineRenderer.positionCount; index++)
            {
                _trajectoryLineRenderer.SetPosition(index, SimulateArc()[index]);
            }
        }

        /// <summary>
        /// Calculates the positions of the next steps using kinematics
        /// </summary>
        /// <returns>A list of Vector3 positions that will be used for the line renderer</returns>
        private List<Vector3> SimulateArc()
        {
            var lineRendererPoints = new List<Vector3>();

            const float maxDuration = 1.3f;
            const float timeStepInterval = 0.1f;
            const int maxSteps = (int)(maxDuration / timeStepInterval);
            var directionVector = transform.forward + new Vector3(0f, _initialDirection, 0.0f);
            var launchPosition = _snowballTransform.position + _snowballTransform.forward;

            _initialVelocity = _throwForce / _mass * Time.fixedDeltaTime; //Velocity = Force / Mass * time

            for (var i = 0; i < maxSteps; ++i)
            {
                //Remember f(t) = (x0 + x*t, y0 + y*t - 9.81t²/2)
                //calculatedPosition = Origin + (transform.forward * (speed * which step * the length of a step);
                var calculatedPosition = launchPosition + directionVector * (_initialVelocity * i * timeStepInterval); //Move both X and Y at a constant speed per Interval
                calculatedPosition.y += Physics.gravity.y / 2 * Mathf.Pow(i * timeStepInterval, 2); //Subtract Gravity from Y

                lineRendererPoints.Add(calculatedPosition);

                if (CheckForCollision(calculatedPosition))
                {
                    break;
                }
            }
            return lineRendererPoints;
        }

        /// <summary>
        /// Check if current position of the trajectory arc is within another object with a collider
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool CheckForCollision(Vector3 position)
        {
            // TODO: Use OverlapSphereNoAlloc()
            //Measure collision via a small circle at the latest position, dont continue simulating Arc if hit
            var hits = Physics.OverlapSphere(position, _collisionCheckRadius);
            hits = hits.ToList().FindAll(hit => hit.gameObject == _studentThrower.gameObject).ToArray();
            return hits.Length > 0;
        }

        /// <summary>
        /// Throws Snowball by the Student
        /// </summary>
        public void ThrowSnowball()
        {
            _isDestroyable = true;
            _sphereCollider.enabled = true;
            _snowballRigidbody.isKinematic = false;
            // TODO: Direction will be handled via hand release on Animation
            var direction = new Vector3(0f, _initialDirection, 0.0f);
            direction += _snowballTransform.forward;
            _snowballRigidbody.AddForce(direction.normalized * _throwForce);
        }

        /// <summary>
        /// Assigns student who threw the snowball
        /// Will be handy later on when calculating scores, etc
        /// </summary>
        /// <param name="student"></param>
        public void SetSnowballThrower(NetworkStudentController student)
        {
            _studentThrower = student;
        }

        /// <summary>
        /// Set the holding place of the snowball that will be placed at until it's thrown
        /// So far, it's either the player's hand or the cannon's slingshot placement
        /// </summary>
        /// <param name="place"></param>
        public void SetHoldingPlace(Transform place)
        {
            _holdingPlace = place;
        }

        /// <summary>
        /// Disable Line renderer when the aim key is released before the animation event for throwing starts
        /// </summary>
        public void DisableLineRenderer()
        {
            if (_trajectoryLineRenderer.enabled)
            {
                _trajectoryLineRenderer.enabled = false;
            }
        }

        public void DestroySnowball()
        {
            PhotonNetwork.Destroy(gameObject);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_isDestroyable);
            }
            else
            {
                _isDestroyable = (bool)stream.ReceiveNext();
            }
        }
    }
}
