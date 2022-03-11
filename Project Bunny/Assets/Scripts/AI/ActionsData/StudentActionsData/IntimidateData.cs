using AI.Actions.StudentActions;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.ActionsData.StudentActionsData
{
    [CreateAssetMenu(fileName = "Intimidate", menuName = "AI/Action/Student/Intimidate")]
    public class IntimidateData : ActionData
    {
        [Header("Intimidate")]
        [SerializeField] private string _animationTrigger;
        [SerializeField] [Min(0f)] private float _rotationSpeed = 10f;
        
        public override void Create(Agent agent)
        {
            if (agent is Student student)
            {
                var action = new Intimidate(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), student, hasTarget, _animationTrigger, _rotationSpeed);
                student.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned student action " + name + "to a non-student agent!");
            }
        }
    }
}
