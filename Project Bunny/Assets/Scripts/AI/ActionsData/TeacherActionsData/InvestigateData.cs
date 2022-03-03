using AI.Actions.TeacherActions;
using AI.Agents;
using UnityEngine;

namespace AI.ActionsData.TeacherActionsData
{
    [CreateAssetMenu(fileName = "Investigate", menuName = "AI/Action/Teacher/Investigate")]
    public class InvestigateData : ActionData
    {
        [Header("Investigate")] 
        [SerializeField] [Range(0f, 180f)] private float _walkingFieldOfView;
        [SerializeField] [Range(0f, 180f)] private float _investigationFieldOfView;
        [SerializeField] [Min(0f)] private float _investigationTime;
    
        public override void Create(Agent agent)
        {
            if (agent is Teacher teacher)
            {
                var action = new Investigate(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), teacher, hasTarget, _investigationTime, _walkingFieldOfView, _investigationFieldOfView);
                teacher.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned teacher action " + name + "to a non-teacher agent!");
            }
        }
    }
}
