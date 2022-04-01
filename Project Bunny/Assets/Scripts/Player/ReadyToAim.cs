using UnityEngine;

namespace Player
{
    public class ReadyToAim : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<NetworkStudentAnimator>().SetThrewSnowball(false);
        }
    }
}
