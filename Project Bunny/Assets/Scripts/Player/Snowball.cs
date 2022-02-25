using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Snowball : MonoBehaviour
    {
        [SerializeField] private Rigidbody _snowballRigidbody;
        [SerializeField] private Transform _snowballTransform;
        [SerializeField] private LineRenderer _lineRenderer;

        private float _throwForce;
        private float _mass;
        private float _initialVelocity;
        private readonly float _collisionCheckRadius = 0.05f;

        // ReSharper disable once NotAccessedField.Local
        private StudentController _studentThrower;

        private void Awake()
        {
            if (_snowballRigidbody == null)
            {
                _snowballRigidbody = gameObject.GetComponent<Rigidbody>();
            }
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }
        
            _mass = _snowballRigidbody.mass;
            _lineRenderer.enabled = false;
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
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = SimulateArc().Count;
            for (var index = 0; index < _lineRenderer.positionCount; index++)
            {
                _lineRenderer.SetPosition(index, SimulateArc()[index]);
            }
        }
        
        /// <summary>
        /// Calculates the positions of the next steps using kinematics
        /// </summary>
        /// <returns>A list of Vector3 positions that will be used for the line renderer</returns>
        private List<Vector3> SimulateArc()
        {
            var lineRendererPoints = new List<Vector3>();
            
            const float maxDuration = 1f;
            const float timeStepInterval = 0.1f;
            const int maxSteps = (int)(maxDuration / timeStepInterval);
            var directionVector = transform.forward + new Vector3(0f, 0.2f, 0.0f);
            var launchPosition = _snowballTransform.position + _snowballTransform.forward;
            
            _initialVelocity = _throwForce / _mass * Time.fixedDeltaTime; //Velocity = Force / Mass * time
            
            for (var i = 0; i < maxSteps; ++i)
            {
                //Remember f(t) = (x0 + x*t, y0 + y*t - 9.81t²/2)
                //calculatedPosition = Origin + (transform.forward * (speed * which step * the length of a step);
                var calculatedPosition = launchPosition + directionVector * (_initialVelocity * i * timeStepInterval); //Move both X and Y at a constant speed per Interval
                calculatedPosition.y += Physics.gravity.y/2 * Mathf.Pow(i * timeStepInterval, 2); //Subtract Gravity from Y

                lineRendererPoints.Add(calculatedPosition);

                if (CheckForCollision(calculatedPosition))
                {
                    break;
                }
            }
            return lineRendererPoints;
        }
        
        /// <summary>
        /// Check if current position in the world is within another object with a collider
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool CheckForCollision(Vector3 position)
        {
            // TODO: Use OverlapSphereNoAlloc()
            var hits = Physics.OverlapSphere(position, _collisionCheckRadius); //Measure collision via a small circle at the latest position, dont continue simulating Arc if hit
            return hits.Length > 0;
        }

        /// <summary>
        /// Throws Snowball by the Student
        /// </summary>
        public void ThrowSnowball()
        {
            transform.parent = null;
            _snowballRigidbody.isKinematic = false;
            // TODO: Direction will be handled via hand release on Animation
            var direction = new Vector3(0f, 0.2f, 0.0f);
            direction += _snowballTransform.forward;
            _snowballRigidbody.AddForce(direction.normalized * _throwForce);
            _lineRenderer.enabled = false;
        }

        /// <summary>
        /// Assigns student who threw the snowball
        /// Will be handy later on when calculating scores, etc
        /// </summary>
        /// <param name="student"></param>
        public void SetSnowballThrower(StudentController student)
        {
            _studentThrower = student;
        }
    }
}
