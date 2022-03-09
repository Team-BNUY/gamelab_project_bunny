using AI.Agents;
using UnityEngine;

namespace AI.Actions.StudentActions
{
    public class JoinAnotherStudent : Action
    {
        private Student _student;
        
        public JoinAnotherStudent(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Student agent, bool hasTarget)
             : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
        {
            _student = agent;
        }

        public override bool PrePerform()
        {
            throw new System.NotImplementedException();
        }

        public override void Perform()
        {
            throw new System.NotImplementedException();
        }

        public override bool PostPerform()
        {
            throw new System.NotImplementedException();
        }
    }
}
