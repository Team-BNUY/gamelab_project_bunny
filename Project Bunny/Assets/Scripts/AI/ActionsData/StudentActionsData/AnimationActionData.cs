using AI.Actions.StudentActions;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.ActionsData.StudentActionsData
{
    [CreateAssetMenu(fileName = "Animation Action", menuName = "AI/Action/Student/Animation Action")]
    public class AnimationActionData : ActionData
    {
        [Header("Animation Action")]
        [SerializeField] [Min(0f)] private float _runningSpeed = 4f;
        [SerializeField] private ActionSpotType _actionSpotType;
        
        [Header("Animator Parameter")]
        [SerializeField] private string _animationTrigger;
        [SerializeField] [Min(1)] private int _animationVariants;
        
        public override void Create(Agent agent)
        {
            if (agent is Student student)
            {
                var action = new AnimationAction(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), student, hasTarget, _runningSpeed, _actionSpotType, _animationTrigger, _animationVariants);
                student.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned student action " + name + "to a non-student agent!");
            }
        }
    }
}
