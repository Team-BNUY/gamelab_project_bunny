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
        private float _gravity;
        private readonly float _collisionCheckRadius = 0.1f;

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

        public void SetSnowballParams(float force, float gravity)
        {
            _throwForce = force * 1000f;
            _gravity = gravity;
        }

        public void DrawTrajectory()
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = SimulateArc().Count;
            for (var a = 0; a < _lineRenderer.positionCount; a++)
            {
                _lineRenderer.SetPosition(a, SimulateArc()[a]); //Add each Calculated Step to a LineRenderer to display a Trajectory. Look inside LineRenderer in Unity to see exact points and amount of them
            }
        }
        
        private List<Vector3> SimulateArc()
        {
            var lineRendererPoints = new List<Vector3>(); //Reset LineRenderer List for new calculation
            
            const float maxDuration = 2f; //INPUT amount of total time for simulation
            const float timeStepInterval = 0.1f; //INPUT amount of time between each position check
            const int maxSteps = (int)(maxDuration / timeStepInterval); //Calculates amount of steps simulation will iterate for
            var directionVector = transform.up; //INPUT launch direction (This Vector3 is automatically normalized for us, keeping it in low and communicable terms)
            var launchPosition = _snowballTransform.position + _snowballTransform.up; //INPUT launch origin (Important to make sure RayCast is ignoring some layers (easiest to use default Layer 2))
            
            _initialVelocity = _throwForce / _mass * Time.fixedDeltaTime; //Velocity = Force / Mass * time
            
            for (var i = 0; i < maxSteps; ++i)
            {
                //Remember f(t) = (x0 + x*t, y0 + y*t - 9.81t²/2)
                //calculatedPosition = Origin + (transform.up * (speed * which step * the length of a step);
                var calculatedPosition = launchPosition + directionVector * (_initialVelocity * i * timeStepInterval); //Move both X and Y at a constant speed per Interval
                calculatedPosition.y += Physics.gravity.y/2 * Mathf.Pow(i * timeStepInterval, 2); //Subtract Gravity from Y

                lineRendererPoints.Add(calculatedPosition); //Add this to the next entry on the list

                if (CheckForCollision(calculatedPosition)) //if you hit something, stop adding positions
                {
                    break; //stop adding positions
                }
            }
            return lineRendererPoints;
        }
        
        private bool CheckForCollision(Vector2 position)
        {
            var hits = Physics.OverlapSphere(position, _collisionCheckRadius); //Measure collision via a small circle at the latest position, dont continue simulating Arc if hit
            return hits.Length > 0;
        }
    

        /// <summary>
        /// Throws Snowball by the Student
        /// </summary>
        /// <param name="force"></param>
        public void ThrowSnowball(float force)
        {
            transform.parent = null;
            _snowballRigidbody.isKinematic = false;
            // TODO: Direction will be handled via hand release on Animation
            var direction = new Vector3(0f, 0.2f, 0.0f);
            direction += _snowballTransform.forward;
            _snowballRigidbody.AddForce(direction.normalized * force * 1000f);
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
