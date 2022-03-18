using System.Linq;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.Actions.StudentActions
{
    public class Cry : Action
    {
        private Student _student;

        private float _runningSpeed;
        private float _duration;
        private float _timer;
        private CryingSpot _cryingSpot;
        
        private string _animationTrigger;
        private int _animationVariants;
        
        public Cry(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget, float runningSpeed, float duration, string animationTrigger, int animationVariants)
            : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
            _runningSpeed = runningSpeed;
            _duration = duration;
            _animationTrigger = animationTrigger;
            _animationVariants = animationVariants;
        }

        public override bool IsAchievable()
        {
            return _student.CryingSpots.Count != 0 && _student.CryingSpots.Any(cs => !cs.Occupied && Vector3.Distance(cs.transform.position, _student.transform.position) > 5f);
        }

        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;
            _timer = _duration;
            
            _cryingSpot = FindClosest(_student.CryingSpots.Where(cs => !cs.Occupied && Vector3.Distance(cs.transform.position, _student.transform.position) > 5f).ToList(), navMeshAgent);

            if (!_cryingSpot) return false;
            
            // General parameters
            navMeshAgent.speed = _runningSpeed;
            target = _cryingSpot.transform.position;
            _student.AnimationState = AnimationState.Run;
            
            Gang.Found(_student);
            _student.Gang.InteractWith();
            
            return true;
        }

        public override void Perform()
        {
            if (!invoked)
            {
                // Animator parameters
                _student.AnimationState = AnimationState.Idle;
                
                var random = Random.Range(0, _animationVariants);
                _student.SetAnimatorParameter("Random", random);
                _student.SetAnimatorParameter(_animationTrigger, true);
            }

            invoked = true;
            _timer -= Time.deltaTime;
            
            if (_timer > 0) return;
            
            _student.CompleteAction();
        }

        public override bool PostPerform()
        {
            _student.SetAnimatorParameter(_animationTrigger, false);

            _student.BeliefStates.RemoveState("intimidated");
            _student.Gang.SetFree();
            _cryingSpot = null;

            return true;
        }

        public override void OnInterrupt()
        {
            _student.SetAnimatorParameter(_animationTrigger, false);

            _student.BeliefStates.RemoveState("intimidated");
            _student.Gang.SetFree();
            _cryingSpot = null;
        }
    }
}
