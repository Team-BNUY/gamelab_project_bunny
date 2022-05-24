using AI.Actions.StudentActions;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.ActionsData.StudentActionsData
{
    [CreateAssetMenu(fileName = "Stand Up", menuName = "AI/Action/Student/Stand Up")]
    public class StandUpData : ActionData
    {
        [Header("Stand Up")]
        [SerializeField] [Min(0f)] private float _timeToStandUp = 4f;
        
        [Header("Animator Parameter")]
        [SerializeField] private string _animationTrigger;
        [SerializeField] [Min(1)] private int _animationVariants;
        
        public override void Create(Agent agent)
        {
            if (agent is Student student)
            {
                var action = new StandUp(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), student, hasTarget, _timeToStandUp, _animationTrigger, _animationVariants);
                student.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned student action " + name + "to a non-student agent!");
            }
        }
    }
}
