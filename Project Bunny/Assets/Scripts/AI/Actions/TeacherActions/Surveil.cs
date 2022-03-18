using AI.Agents;
using AI.Core;
using UnityEngine;

public class Surveil : Action
{
    private Teacher _teacher;

    private Transform _waypoint;
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
        return _teacher.Waypoints.Length != 0;
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
        var random = Random.Range(0, _teacher.Waypoints.Length);
        _waypoint = _teacher.Waypoints[random];
        target = _waypoint.position;
        
        // Animator parameters
        _teacher.AnimationState = AnimationState.Walk;
        
        // View parameters
        _teacher.LookForward();
        navMeshAgent.speed = _speed;
        _teacher.FieldOfView = _walkingFieldOfView;
        
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
            // Animator parameters
            _teacher.AnimationState = AnimationState.Idle;
            _teacher.SetAnimatorParameter("LookingAround", true);
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
        _teacher.SetAnimatorParameter("LookingAround", false);
        
        return success;
    }

    public override void OnInterrupt()
    {
        _teacher.SetAnimatorParameter("LookingAround", false);
    }
}
