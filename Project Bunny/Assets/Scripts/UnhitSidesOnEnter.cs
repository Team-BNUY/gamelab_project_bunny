using AI.Agents;
using Player;
using UnityEngine;

public class UnhitSidesOnEnter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Student>()?.UnhitSides();
        animator.GetComponent<Teacher>()?.UnhitSides();
        animator.GetComponent<NetworkStudentAnimator>()?.UnhitSides();
    }
}
