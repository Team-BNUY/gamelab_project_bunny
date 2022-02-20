using AI.Actions.TeacherActions;
using AI.Agents;
using UnityEngine;

namespace AI.ActionsData.TeacherActionsData
{
    [CreateAssetMenu(fileName = "Chase Student", menuName = "AI/Action/Teacher/Chase Student")]
    public class ChaseStudentData : ActionData
    {
        [Header("Chase Student")] 
        [SerializeField] private float _speed;

        public override void Create(Agent agent)
        {
            if (agent is Teacher teacher)
            {
                var action = new ChaseStudent(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), teacher, hasTarget, _speed);
                teacher.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned teacher action " + name + "to a non-teacher agent!");
            }
        }
    }
}
