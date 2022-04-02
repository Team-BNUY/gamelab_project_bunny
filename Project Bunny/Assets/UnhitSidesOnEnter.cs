using AI.Agents;
using UnityEngine;

public class UnhitSidesOnEnter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Student>().UnhitSides();
    }
}
