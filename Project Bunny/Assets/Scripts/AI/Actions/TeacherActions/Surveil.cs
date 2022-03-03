using AI;
using AI.Agents;
using UnityEngine;

public class Surveil : Action
{
    private Teacher _teacher;

    private Transform _waypoint;
    private float _speed;
    private float _walkingFieldOfView;
    private float _surveilFieldOfView;
    
    public Surveil(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Teacher agent, bool hasTarget, float speed, float walkingFieldOfView, float surveilFieldOfView)
         : base(name, cost, preconditionStates, afterEffectStates, agent, hasTarget)
    {
        _speed = speed;
        _walkingFieldOfView = walkingFieldOfView;
        _surveilFieldOfView = surveilFieldOfView;
        _teacher = agent;
    }

    public override bool IsAchievable()
    {
        return _teacher.Waypoints.Length != 0;
    }

    public override bool PrePerform()
    {
        var random = Random.Range(0, _teacher.Waypoints.Length);
        _waypoint = _teacher.Waypoints[random];
        target = _waypoint.position;
        
        // View parameters
        _teacher.LookForward();
        navMeshAgent.speed = _speed;
        _teacher.FieldOfView = _walkingFieldOfView;
        
        return true;
    }

    public override void Perform()
    {
        _teacher.FieldOfView = _surveilFieldOfView;
        
        agent.CompleteAction(); // TODO Play surveil/investigate animation and complete action from animation completion
    }

    public override bool PostPerform()
    {
        return success;
    }
}
