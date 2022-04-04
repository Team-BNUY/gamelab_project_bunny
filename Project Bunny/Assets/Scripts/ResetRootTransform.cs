using UnityEngine;

public class ResetRootTransform : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.localPosition = Vector3.zero;
    }
}
