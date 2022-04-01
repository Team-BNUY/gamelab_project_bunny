using System.Collections;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.Actions.StudentActions
{
    public class Intimidate : Action
    {
        private Student _student;

        private Student _victim;
        private float _rotationSpeed;
        
        private string _animationTrigger;
        private int _animationVariants;
        
        public Intimidate(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget, float rotationSpeed, string animationTrigger, int animationVariants)
             : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
            _animationTrigger = animationTrigger;
            _rotationSpeed = rotationSpeed;
            _animationVariants = animationVariants;
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
            
            // Animator parameters
            _student.AnimationState = AnimationState.Idle;
            
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
            navMeshAgent.SetDestination(_student.transform.position);
            _student.Gang.SetFree();
        }
        
        private IEnumerator RotateTowardsVictim()
        {
            var studentPosition = _student.transform.position;
            var victimPosition = _victim.transform.position;
            var adjustedStudentPosition = new Vector3(victimPosition.x, studentPosition.y, victimPosition.z);
            var targetLookRotation = adjustedStudentPosition - studentPosition;
            do
            {
                var lookRotation = Quaternion.LookRotation(targetLookRotation, Vector3.up);
                var lerpRotation = Quaternion.Lerp(_student.transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
                _student.transform.rotation = lerpRotation;

                yield return null;
            } 
            while (Vector3.Angle(targetLookRotation, _student.transform.forward) > 2f);
            
            // Animator parameters
            var random = Random.Range(0, _animationVariants);
            agent.SetAnimatorParameter("Random", random);
            agent.SetAnimatorParameter(_animationTrigger);
        }
    }
}
