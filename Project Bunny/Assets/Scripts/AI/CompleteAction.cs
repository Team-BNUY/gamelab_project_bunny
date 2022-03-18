using AI.Core;
using UnityEngine;

namespace AI
{
    public class CompleteAction : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var agent = animator.GetComponent<Agent>();
            if (!agent) return;
            
            agent.CompleteAction();
        }
    }
}
