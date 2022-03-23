using System.Collections;
using System.Linq;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.Actions.StudentActions
{
    public class AnimationAction : Action
    {
        private Student _student;

        private float _runningSpeed;
        private ActionSpot _actionSpot;
        private ActionSpotType _actionSpotType;
        
        private string _animationTrigger;
        private int _animationVariants;
        
        public AnimationAction(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget, float runningSpeed, ActionSpotType actionSpotType, string animationTrigger, int animationVariants)
            : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
            _runningSpeed = runningSpeed;
            _actionSpotType = actionSpotType;
            _animationTrigger = animationTrigger;
            _animationVariants = animationVariants;
        }

        public override bool IsAchievable()
        {
            return _student.ActionSpots.Count != 0 && _student.ActionSpots.Any(s => s.Type == _actionSpotType && !s.Occupied && Vector3.Distance(s.transform.position, _student.transform.position) > 5f);
        }

        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;
            
            _actionSpot = FindClosest(_student.ActionSpots.Where(s => s.Type == _actionSpotType && !s.Occupied && Vector3.Distance(s.transform.position, _student.transform.position) > 5f).ToList(), navMeshAgent);

            if (!_actionSpot) return false;
            
            // General parameters
            navMeshAgent.speed = _runningSpeed;
            target = _actionSpot.transform.position;
            _student.AnimationState = AnimationState.Run;
            
            _actionSpot.Use();
            Gang.Found(_student);
            _student.Gang.InteractWith();
            
            return true;
        }

        public override void Perform()
        {
            if (invoked) return;
            invoked = true;
            
            // Animator parameters
            _student.AnimationState = AnimationState.Idle;

            if (_actionSpot.InteractiveObject)
            {
                _student.StartCoroutine(RotateTowardsInteractive());
                return;
            }
            
            var random = Random.Range(0, _animationVariants);
            _student.SetAnimatorParameter("Random", random);
            _student.SetAnimatorParameter(_animationTrigger, true);
        }

        public override bool PostPerform()
        {
            _student.SetAnimatorParameter(_animationTrigger, false);

            _student.BeliefStates.ModifyState("wantsToPlayAlone", -1);
            _student.BeliefStates.ModifyState("poleSeemsAttracting", -1);
            _student.BeliefStates.AddState("completedAnimationAction", 1);

            _student.Gang.SetFree();
            _actionSpot.Free();
            _actionSpot = null;

            return true;
        }

        public override void OnInterrupt()
        {
            _student.SetAnimatorParameter(_animationTrigger, false);

            _student.BeliefStates.ModifyState("wantsToPlayAlone", -1);
            _student.BeliefStates.ModifyState("poleSeemsAttracting", -1);
            
            navMeshAgent.SetDestination(_student.transform.position);
            _student.Gang.SetFree();
            _actionSpot.Free();
            _actionSpot = null;
        }
        
        private IEnumerator RotateTowardsInteractive()
        {
            var studentPosition = _student.transform.position;
            var interactivePosition = _actionSpot.InteractiveObject.position;
            var adjustedInteractivePosition = new Vector3(interactivePosition.x, studentPosition.y, interactivePosition.z);
            var targetLookRotation = adjustedInteractivePosition - studentPosition;
            do
            {
                var lookRotation = Quaternion.LookRotation(targetLookRotation, Vector3.up);
                var lerpRotation = Quaternion.Lerp(_student.transform.rotation, lookRotation, 20f * Time.deltaTime);
                _student.transform.rotation = lerpRotation;

                yield return null;
            } 
            while (Vector3.Angle(targetLookRotation, _student.transform.forward) > 2f);
            
            // Animator parameters
            var random = Random.Range(0, _animationVariants);
            _student.SetAnimatorParameter("Random", random);
            _student.SetAnimatorParameter(_animationTrigger);
        }
    }
}
