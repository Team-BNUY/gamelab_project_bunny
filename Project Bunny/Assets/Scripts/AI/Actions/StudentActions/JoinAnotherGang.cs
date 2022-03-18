using System.Collections;
using System.Linq;
using AI.Agents;
using AI.Core;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Actions.StudentActions
{
    public class JoinAnotherGang : Action
    {
        private Student _student;

        private Gang _gang;
        private float _speed;
        private float _rotationSpeed;

        private string _animationTrigger;
        private int _animationVariants;
        
        public JoinAnotherGang(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget, float speed, float rotationSpeed, string animationTrigger, int animationVariants)
             : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
            _speed = speed;
            _rotationSpeed = rotationSpeed;
            _animationTrigger = animationTrigger;
            _animationVariants = animationVariants;
        }

        public override bool IsAchievable()
        {
            var gangAvailable = Student.Gangs.Count != 0 && Student.Gangs.Any(g => !g.Full && _student.Gang != g && !g.Occupied);
            return !_student.Occupied && gangAvailable;
        }

        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;
            
            // Finds a gang to join
            if (!IsAchievable()) return false; 
            
            var openGangs = Student.Gangs.Where(g => !g.Full && _student.Gang != g && !g.Occupied).ToArray();
            var random = Random.Range(0, openGangs.Length);
            _gang = openGangs[random];
            
            if (_gang == null) return false;
            
            // Calculates a position to reach within the gang
            var x = Random.Range(-1f, 1f);
            var z = Random.Range(-1f, 1f);
            var direction = new Vector3(x, 0f, z).normalized;
            var length = Random.Range(_gang.Radius - 1f, _gang.Radius); // TODO Make first parameter a "gang's circle's thickness" variable
            var prePosition = _gang.Center + direction * length;
            var yCheck = Physics.Raycast(prePosition, Vector3.up, out var groundInfo, float.PositiveInfinity, _student.GroundLayer);
            if (!yCheck)
            {
                yCheck = Physics.Raycast(prePosition, Vector3.down, out groundInfo, float.PositiveInfinity, _student.GroundLayer);
            }
            var y = yCheck ? groundInfo.point.y : _gang.Center.y;
            var position = new Vector3(prePosition.x, y, prePosition.z);
            
            // Checks if the position is free
            var path = new NavMeshPath();
            if (!navMeshAgent.CalculatePath(position, path) || Physics.CheckSphere(position, navMeshAgent.radius, _student.StudentLayer)) return false;
            
            // Checks if there is an obstacle between the student and the gang in the hypothetical join position
            foreach (var memberPosition in _gang.Members.Select(m => m.transform.position))
            {
                direction = memberPosition - position;
                var distance = Vector3.Distance(memberPosition, position);
                
                if (Physics.Raycast(position + Vector3.up * 2.5f, direction, distance, _student.ObstacleLayer)) return false; // TODO Take student height somewhere else
            }            
            
            // Makes the student found a new gang and sets the target gang as occupied
            Gang.Found(_student);
            _student.Gang.InteractWith();
            _gang.InteractWith();
            
            // General parameters
            target = position;
            navMeshAgent.speed = _speed;
            _student.AnimationState = AnimationState.Walk;
            
            return true;
        }

        public override void Perform()
        {
            if (invoked) return;
            invoked = true;
            
            _student.AnimationState = AnimationState.Idle;
            _student.StartCoroutine(RotateTowardsGangCenter());
        }

        public override bool PostPerform()
        {
            _student.BeliefStates.RemoveState("completedAnimationAction");

            _student.Gang.SetFree();
            _gang.Join(_student);
            _gang.SetFree();

            return true;
        }

        public override void OnInterrupt()
        {
            _student.Gang.SetFree();
            _gang.SetFree();
        }

        private IEnumerator RotateTowardsGangCenter()
        {
            var transform = _student.transform;
            var position = transform.position;
            var targetLookRotation = _gang.Center - position;
            do
            {
                var lookRotation = Quaternion.LookRotation(targetLookRotation, Vector3.up);
                var lerpRotation = Quaternion.Lerp(_student.transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
                _student.transform.rotation = lerpRotation;

                yield return null;
            } 
            while (Vector3.Angle(targetLookRotation, _student.transform.forward) > 5f);
            
            // Animator parameters
            var random = Random.Range(0, _animationVariants);
            _student.SetAnimatorParameter("Random", random);
            _student.SetAnimatorParameter(_animationTrigger);
        }
    }
}
