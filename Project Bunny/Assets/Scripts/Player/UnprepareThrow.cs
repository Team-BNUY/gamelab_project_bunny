using UnityEngine;

namespace Player
{
    public class UnprepareThrow : StateMachineBehaviour
    {
        private static readonly int PrepareThrow = Animator.StringToHash("PrepareThrow");

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(PrepareThrow, false);
        }
    }
}
