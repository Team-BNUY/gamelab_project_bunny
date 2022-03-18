using AI.Actions.StudentActions;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.ActionsData.StudentActionsData
{
    [CreateAssetMenu(fileName = "Welcome", menuName = "AI/Action/Student/Welcome")]
    public class WelcomeData : ActionData
    {
        [Header("Welcome")]
        [SerializeField] private bool _faceAway;
        [SerializeField] [Min(0f)] private float _rotationSpeed = 10f;
        
        [Header("Animator Parameters")]
        [SerializeField] private string _animationTrigger;
        [SerializeField] [Min(1)] private int _animationVariants;

        
        public override void Create(Agent agent)
        {
            if (agent is Student student)
            {
                var action = new Welcome(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), student, hasTarget, _faceAway, _rotationSpeed, _animationTrigger, _animationVariants);
                student.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned student action " + name + "to a non-student agent!");
            }
        }
    }
}
