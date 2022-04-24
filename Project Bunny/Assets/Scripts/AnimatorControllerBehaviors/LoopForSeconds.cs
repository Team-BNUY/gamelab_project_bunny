using System.Collections;
using System.Collections.Generic;
using AI.Agents;
using UnityEngine;

public class LoopForSeconds : StateMachineBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private string _trigger;
    private float _timer;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = _time;
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        
        animator.GetComponent<Student>().SetAnimatorParameter(_trigger);
    }
}
