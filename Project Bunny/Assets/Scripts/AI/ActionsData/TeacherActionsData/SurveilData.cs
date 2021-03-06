using AI.Actions.TeacherActions;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.ActionsData.TeacherActionsData
{
    [CreateAssetMenu(fileName = "Surveil", menuName = "AI/Action/Teacher/Surveil")]
    public class SurveilData : ActionData
    {
        [Header("Surveil")] 
        [SerializeField] [Min(0f)] private float _speed;

        [SerializeField] [Min(0f)] private float _lookAroundTime;
        [SerializeField] [Range(0f, 180f)] private float _walkingFieldOfView;
        [SerializeField] [Range(0f, 180f)] private float _surveilFieldOfView;

        public override void Create(Agent agent)
        {
            if (agent is Teacher teacher)
            {
                var action = new Surveil(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), teacher, hasTarget, _speed, _lookAroundTime, _walkingFieldOfView, _surveilFieldOfView);
                teacher.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned teacher action " + name + "to a non-teacher agent!");
            }
        }
    }
}
