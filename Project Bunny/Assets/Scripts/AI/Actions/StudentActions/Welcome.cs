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
            var transform = _student.transform;
            var position = transform.position;
            var targetLookRotation = _faceAway ? position - _newbie.transform.position : _newbie.transform.position - position;
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
            _student.SetAnimatorParameter("Random", random);
            _student.SetAnimatorParameter(_animationTrigger);
        }
    }
}
