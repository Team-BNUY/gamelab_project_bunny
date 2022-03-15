using AI.Actions.StudentActions;
using AI.Agents;
using AI.Core;
using UnityEngine;

namespace AI.ActionsData.StudentActionsData
{
    [CreateAssetMenu(fileName = "Cry", menuName = "AI/Action/Student/Cry")]
    public class CryData : ActionData
    {
        [Header("Cry")] 
        [SerializeField] [Min(0f)] private float _duration = 5f;
        
        public override void Create(Agent agent)
        {
            if (agent is Student student)
            {
                var action = new Cry(name, cost, new StateSet(preconditionStates), new StateSet(afterEffectStates), student, hasTarget, _duration);
                student.Actions.Add(action);
            }
            else
            {
                Debug.LogError("Assigned student action " + name + "to a non-student agent!");
            }
        }
    }
}
