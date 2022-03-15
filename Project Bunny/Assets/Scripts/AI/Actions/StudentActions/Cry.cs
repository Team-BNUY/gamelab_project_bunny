using System.Linq;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.Actions.StudentActions
{
    public class Cry : Action
    {
        private Student _student;

        private float _duration;
        private float _timer;
        private CryingSpot _cryingSpot;
        
        public Cry(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget, float duration)
            : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
            _duration = duration;
        }

        public override bool IsAchievable()
        {
            return _student.CryingSpots.Count != 0 && _student.CryingSpots.Any(cs => !cs.Occupied);
        }

        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;
            _timer = _duration;
            
            _cryingSpot = FindClosest(_student.CryingSpots.Where(cs => !cs.Occupied).ToList(), navMeshAgent);

            if (!_cryingSpot) return false;
            
            _cryingSpot.Occupied = true;
            target = _cryingSpot.transform.position;
            
            Gang.Found(_student);
            _student.Gang.InteractWith();
            
            return true;
        }

        public override void Perform()
        {
            if (!invoked)
            {
                // _student.SetAnimatorParameter("Crying", true); TODO Uncomment when animator controller is set up
            }

            invoked = true;
            _timer -= Time.deltaTime;
            
            if (_timer > 0) return;
            
            _student.CompleteAction();
        }

        public override bool PostPerform()
        {
            _student.BeliefStates.RemoveState("intimidated");
            // _student.SetAnimatorParameter("Crying", false); TODO Uncomment when animator controller is set up
            _student.Gang.SetFree();
            _cryingSpot.Occupied = false;
            _cryingSpot = null;

            return true;
        }

        public override void OnInterrupt()
        {
            _student.BeliefStates.RemoveState("intimidated");
            // _student.SetAnimatorParameter("Crying", false); TODO Uncomment when animator controller is set up
            _student.Gang.SetFree();
            _cryingSpot.Occupied = false;
            _cryingSpot = null;
        }
    }
}
