using AI.Agents;
using UnityEngine;

public class UnhitOnExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Student>().Unhit();
    }
}
