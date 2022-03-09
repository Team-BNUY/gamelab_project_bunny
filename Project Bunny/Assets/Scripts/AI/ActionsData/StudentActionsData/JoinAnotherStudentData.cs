using AI.Actions.StudentActions;
using AI.Agents;
using UnityEngine;

namespace AI.ActionsData.StudentActionsData
{
    public class JoinAnotherStudentData : ActionData
    {
        public override void Create(Agent agent)
        {
            if (agent is Student student)
            {
                var action = new JoinAnotherStudent(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), student, hasTarget);
                student.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned student action " + name + "to a non-student agent!");
            }
        }
    }
}
