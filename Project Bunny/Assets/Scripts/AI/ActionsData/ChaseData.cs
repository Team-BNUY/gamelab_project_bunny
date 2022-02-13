using AI.Actions;
using UnityEngine;

namespace AI.ActionsData
{
    [CreateAssetMenu(fileName = "Chase", menuName = "AI/Action/Chase")]
    public class ChaseData : ActionData
    {
        public override void Create(Agent agent)
        {
            var action = new Chase(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), agent, hasTarget, targetTag);
            agent.Actions.Add(action);
        }
    }
}
