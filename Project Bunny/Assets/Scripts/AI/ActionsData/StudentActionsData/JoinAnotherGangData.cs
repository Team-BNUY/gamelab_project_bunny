using AI.Actions.StudentActions;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.ActionsData.StudentActionsData
{
    [CreateAssetMenu(fileName = "Join Another Student", menuName = "AI/Action/Student/Join Another Student")]
    public class JoinAnotherGangData : ActionData
    {
        [SerializeField] private float _rotationSpeed = 10f;
        
        public override void Create(Agent agent)
        {
            if (agent is Student student)
            {
                var action = new JoinAnotherGang(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), student, hasTarget, _rotationSpeed);
                student.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned student action " + name + "to a non-student agent!");
            }
        }
    }
}
