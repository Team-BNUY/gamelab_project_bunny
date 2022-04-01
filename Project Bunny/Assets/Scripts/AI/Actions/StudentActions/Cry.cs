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
        private ActionSpot _actionSpot;
        private ActionSpotType _actionSpotType;
        
        private string _animationTrigger;
        private int _animationVariants;
        
        public Cry(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget, float runningSpeed, float duration, ActionSpotType actionSpotType, string animationTrigger, int animationVariants)
            : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
            _runningSpeed = runningSpeed;
            _duration = duration;
            _actionSpotType = actionSpotType;
            _animationTrigger = animationTrigger;
            _animationVariants = animationVariants;
        }

        public override bool IsAchievable()
        {
            return !hasTarget || ArenaManager.Instance.ActionSpots.Count != 0 && ArenaManager.Instance.ActionSpots.Any(s => s.Type == _actionSpotType && !s.Occupied && Vector3.Distance(s.transform.position, _student.transform.position) > 5f);
        }

        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;
            _timer = _duration;

            if (!hasTarget) return true;
            
            _actionSpot = FindClosest(ArenaManager.Instance.ActionSpots.Where(s => s.Type == _actionSpotType && !s.Occupied && Vector3.Distance(s.transform.position, _student.transform.position) > 5f).ToList(), navMeshAgent);

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

            if (!hasTarget)
            {
                _student.BeliefStates.RemoveState("hitByProjectile");
                return true;
            }
            
            _student.BeliefStates.RemoveState("intimidated");
            _student.BeliefStates.AddState("curiousAboutOthers", 1);
            
            _student.Gang.SetFree();
            _actionSpot.Free();
            _actionSpot = null;

            return true;
        }

        public override void OnInterrupt()
        {
            _student.SetAnimatorParameter(_animationTrigger, false);

            if (!hasTarget)
            {
                _student.BeliefStates.RemoveState("hitByProjectile");
                return;
            }

            navMeshAgent.SetDestination(_student.transform.position);
            _student.BeliefStates.RemoveState("intimidated");
            _student.Gang.SetFree();
            _actionSpot.Free();
            _actionSpot = null;
        }
    }
}
