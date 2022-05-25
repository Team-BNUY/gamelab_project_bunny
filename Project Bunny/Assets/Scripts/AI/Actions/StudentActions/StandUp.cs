using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.Actions.StudentActions
{
    public class StandUp : Action
    {
        private readonly Student _student;
        private float _timer;

        private readonly float _timeToStandUp;
        private readonly string _animationTrigger;
        private readonly int _animationVariants;
        
        public StandUp(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget, float timeToStandUp, string animationTrigger, int animationVariants)
            : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
            _timeToStandUp = timeToStandUp;
            _animationTrigger = animationTrigger;
            _animationVariants = animationVariants;
        }

        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;
            _timer = _timeToStandUp;
            _student.SetAnimatorParameter(_animationTrigger, false);

            _student.AnimationState = AnimationState.Idle;
            _student.Gang.InteractWith();
            
            return true;
        }

        public override void Perform()
        {
            _timer -= Time.deltaTime;

            if (_timer > 0f) return;
            
            if (invoked) return;
            invoked = true;
            
            // Animator parameters
            _student.AnimationState = AnimationState.Idle;

            var random = Random.Range(0, _animationVariants);
            _student.SetAnimatorParameter("Random", random);
            _student.SetAnimatorParameter(_animationTrigger, true);
        }

        public override bool PostPerform()
        {
            _student.SetAnimatorParameter(_animationTrigger, false);
            _student.BeliefStates.RemoveState("hitByRollBall");
            _student.PhysicsCollider.enabled = true;
            _student.Gang.SetFree();

            return true;
        }

        public override void OnInterrupt()
        {
            _student.SetAnimatorParameter("Random", 0);
            _student.SetAnimatorParameter(_animationTrigger, true);
            _student.BeliefStates.RemoveState("hitByRollBall");
            _student.PhysicsCollider.enabled = true;
            _student.Gang.SetFree();

            navMeshAgent.SetDestination(_student.transform.position);
        }
    }
}
