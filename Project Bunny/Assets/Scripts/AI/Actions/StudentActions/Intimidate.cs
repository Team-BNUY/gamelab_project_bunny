using System.Collections;
using AI.Agents;
using AI.Core;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Actions.StudentActions
{
    public class Intimidate : Action
    {
        private Student _student;

        private Student _victim;
        private string _animationTrigger;
        private float _rotationSpeed;
        
        public Intimidate(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget, string animationTrigger, float rotationSpeed)
             : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
            _animationTrigger = animationTrigger;
            _rotationSpeed = rotationSpeed;
        }

        public override bool IsAchievable()
        {
            return !_student.Gang.Occupied && _student.Gang.Size > 1;
        }

        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;

            _victim = _student.Gang.GetRandomMemberExcept(_student);

            if (!_victim) return false;

            if (_student.Gang.Size > 2)
            {
                var victimTransform = _victim.transform;
                var victimPosition = victimTransform.position;
                var distanceFromVictim = Vector3.Distance(_student.transform.position, victimPosition);
                var targetPosition = victimPosition + victimTransform.forward * Mathf.Min(distanceFromVictim, 2f);
                
                // Checks if the position is free TODO Add a raycast to make sure there is no accidental wall between the student and their victim
                var path = new NavMeshPath();
                if (!navMeshAgent.CalculatePath(targetPosition, path)) return false;
                
                target = targetPosition;
            }
            else
            {
                HasTarget = false;
            }

            _student.Gang.InteractWith();
            
            return true;
        }

        public override void Perform()
        {
            if (invoked) return;
            invoked = true;

            _student.StartCoroutine(RotateTowardsVictim());
        }

        public override bool PostPerform()
        {
            _victim.BeliefStates.AddState("intimidated", 1);
            _student.BeliefStates.ModifyState("suddenNeedToIntimidate", -1);
            _student.Gang.SetFree();
            
            return true;
        }

        public override void OnInterrupt()
        {
            _student.Gang.SetFree();
        }
        
        private IEnumerator RotateTowardsVictim()
        {
            var transform = _student.transform;
            var position = transform.position;
            var targetLookRotation = _victim.transform.position - position;
            do
            {
                var lookRotation = Quaternion.LookRotation(targetLookRotation, Vector3.up);
                var lerpRotation = Quaternion.Lerp(_student.transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
                _student.transform.rotation = lerpRotation;

                yield return null;
            } 
            while (Vector3.Angle(targetLookRotation, _student.transform.forward) > 2f);
            
            _student.CompleteAction(); // TODO Change with animation and complete action after the animation
        }
    }
}
