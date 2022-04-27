using System.Collections;
using System.Linq;
using AI;
using AI.Agents;
using AI.Core;
using Arena;
using UnityEngine;

public class Surveil : Action
{
    private Teacher _teacher;

    private Waypoint _waypoint;
    private float _speed;
    private float _lookAroundTime;
    private float _walkingFieldOfView;
    private float _surveilFieldOfView;
    private float _timer;
    
    public Surveil(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Teacher agent, bool hasTarget, float speed, float lookAroundTime, float walkingFieldOfView, float surveilFieldOfView)
         : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
    {
        _speed = speed;
        _lookAroundTime = lookAroundTime;
        _walkingFieldOfView = walkingFieldOfView;
        _surveilFieldOfView = surveilFieldOfView;
        _teacher = agent;
    }
    
    /// <summary>
    /// Determines if the Teacher knows at least one waypoint
    /// </summary>
    /// <returns>True if the Teacher knows at least one waypoint</returns>
    public override bool IsAchievable()
    {
        return !_teacher.Stunned && ArenaManager.Instance.TeacherWaypoints.Length != 0;
    }
    
    /// <summary>
    /// Sets the Teacher's target and adjusts their view parameters
    /// </summary>
    /// <returns>Always true, no possible failing condition here</returns>
    public override bool PrePerform()
    {
        // Resets parameters
        invoked = false;
        _timer = _lookAroundTime;
        
        // Sets the target
        if (hasTarget)
        {
            var unoccupiedWaypoints = ArenaManager.Instance.TeacherWaypoints.Where(w => !w.Occupied).ToArray();
            var random = Random.Range(0, unoccupiedWaypoints.Length);
            _waypoint = unoccupiedWaypoints[random];
            target = _waypoint.transform.position;
            
            // Animator parameters
            _teacher.AnimationState = AnimationState.Walk;
        
            // View parameters
            _teacher.LookForward();
            navMeshAgent.speed = _speed;
            _teacher.FieldOfView = _walkingFieldOfView;
        }

        return true;
    }

    /// <summary>
    /// Adjusts the Teacher's field of view when arriving at a waypoint
    /// </summary>
    public override void Perform()
    {
        _teacher.FieldOfView = _surveilFieldOfView;
        
        if (!invoked)
        {
            if (hasTarget)
            {
                // Animator parameters
                _teacher.AnimationState = AnimationState.Idle;
                _teacher.SetAnimatorParameter("LookingAround", true);
            }
            else
            {
                _teacher.StartCoroutine(RotateTowardsHit());
            }
        }

        invoked = true;
        _timer -= Time.deltaTime;
            
        if (_timer > 0) return;
            
        agent.CompleteAction();
    }
    
    /// <summary>
    /// Returns true
    /// </summary>
    /// <returns>True</returns>
    public override bool PostPerform()
    {
        if (!hasTarget)
        {
            _teacher.BeliefStates.RemoveState("hitByProjectile");
        }
        
        _teacher.SetAnimatorParameter("LookingAround", false);
        
        return success;
    }

    public override void OnInterrupt()
    {
        if (!hasTarget)
        {
            _teacher.BeliefStates.RemoveState("hitByProjectile");
        }
        
        _teacher.SetAnimatorParameter("LookingAround", false);
        navMeshAgent.SetDestination(_teacher.transform.position);
    }
    
    private IEnumerator RotateTowardsHit()
    {
        var hitDirection = _teacher.HitDirection;
        Debug.Log(hitDirection);
        do
        {
            var lookRotation = Quaternion.LookRotation(hitDirection, Vector3.up);
            var lerpRotation = Quaternion.Lerp(_teacher.transform.rotation, lookRotation, 20f * Time.deltaTime);
            _teacher.transform.rotation = lerpRotation;

            yield return null;
        } 
        while (Vector3.Angle(hitDirection, _teacher.transform.forward) > 4f);
        
        // Animator parameters
        _teacher.AnimationState = AnimationState.Idle;
        _teacher.SetAnimatorParameter("LookingAround", true);
    }
}
