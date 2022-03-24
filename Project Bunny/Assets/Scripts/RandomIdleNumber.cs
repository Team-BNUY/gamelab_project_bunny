using UnityEngine;

public class RandomIdleNumber : StateMachineBehaviour
{
    [SerializeField] private int _min;
    [SerializeField] private int _max;
    
    private static readonly int RandomAnim = Animator.StringToHash("Random Idle");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var random = Random.Range(_min, _max + 1);
        animator.SetInteger(RandomAnim, random);
    }
}
