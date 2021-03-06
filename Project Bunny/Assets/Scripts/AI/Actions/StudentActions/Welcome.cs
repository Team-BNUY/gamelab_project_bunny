using System.Collections;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.Actions.StudentActions
{
    public class Welcome : Action
    {
        private Student _student;

        private Student _newbie;
        private bool _faceAway;
        private float _rotationSpeed;
        
        private string _animationTrigger;
        private int _animationVariants;
        
        public Welcome(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget, bool faceAway, float rotationSpeed, string animationTrigger, int animationVariants)
            : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
            _faceAway = faceAway;
            _rotationSpeed = rotationSpeed;
            _animationTrigger = animationTrigger;
            _animationVariants = animationVariants;
        }

        public override bool IsAchievable()
        {
            return _student.Gang.Newbie;
        }

        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;
            
            // Animator parameters
            _student.AnimationState = AnimationState.Idle;
            
            _newbie = _student.Gang.Newbie;
            _student.Gang.InteractWith();

            return _newbie;
        }

        public override void Perform()
        {
            if (invoked) return;
            invoked = true;

            _student.StartCoroutine(RotateTowardsNewbie());
        }

        public override bool PostPerform()
        {
            _student.Gang.SetFree();
            _student.BeliefStates.ModifyState("newStudentJoinedGang", -1);
            _student.BeliefStates.ModifyState("likesNewMember", -1);
            _student.BeliefStates.ModifyState("dislikesNewMember", -1);

            return true;
        }

        public override void OnInterrupt()
        {
            _student.Gang.SetFree();
            _student.BeliefStates.ModifyState("newStudentJoinedGang", -1);
            _student.BeliefStates.ModifyState("likesNewMember", -1);
            _student.BeliefStates.ModifyState("dislikesNewMember", -1);
        }
        
        private IEnumerator RotateTowardsNewbie()
        {
            var studentPosition = _student.transform.position;
            var newbiePosition = _newbie.transform.position;
            var adjustedNewbiePosition = new Vector3(newbiePosition.x, studentPosition.y, newbiePosition.z);
            var targetLookRotation = _faceAway ? studentPosition - adjustedNewbiePosition : adjustedNewbiePosition - studentPosition;
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
