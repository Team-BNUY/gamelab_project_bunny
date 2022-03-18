using AI.Actions.TeacherActions;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.ActionsData.TeacherActionsData
{
    [CreateAssetMenu(fileName = "Investigate", menuName = "AI/Action/Teacher/Investigate")]
    public class InvestigateData : ActionData
    {
        [Header("Investigate")] 
        [SerializeField] [Min(0f)] private float _investigationTime;
        [SerializeField] [Min(0f)] private float _speed;
        [SerializeField] [Range(0f, 180f)] private float _walkingFieldOfView;
        [SerializeField] [Range(0f, 180f)] private float _investigationFieldOfView;
        
        public override void Create(Agent agent)
        {
            if (agent is Teacher teacher)
            {
                var action = new Investigate(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), teacher, hasTarget, _investigationTime, _speed, _walkingFieldOfView, _investigationFieldOfView);
                teacher.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned teacher action " + name + "to a non-teacher agent!");
            }
        }
    }
}
