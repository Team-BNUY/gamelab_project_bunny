using AI.Agents;
using UnityEngine;

public class PlayStunSound : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var clip = animator.GetComponent<Teacher>().StunAudio;
        AudioManager.Instance.PlayOneShot(clip, 1.5f);
    }
}
