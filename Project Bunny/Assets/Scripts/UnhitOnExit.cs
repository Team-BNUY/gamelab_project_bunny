using AI.Agents;
using Player;
using UnityEngine;

public class UnhitOnExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Student>()?.Unhit();
        animator.GetComponent<Teacher>()?.Unhit();
        animator.GetComponent<NetworkStudentAnimator>()?.Unhit();
    }
}
