using AI.Agents;
using Player;
using UnityEngine;

public class UnhitOnEnter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Student>()?.Unhit();
        animator.GetComponent<Teacher>()?.Unhit();
        animator.GetComponent<NetworkStudentAnimator>()?.Unhit();
    }
}
