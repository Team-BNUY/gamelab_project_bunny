using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class Agent : MonoBehaviour
    {
        [SerializeField] private List<ActionData> _actionsData;

        protected Dictionary<Goal, int> goals = new Dictionary<Goal, int>();
        
        private List<Action> _actions = new List<Action>();
        private StateSet _beliefStates = new StateSet();
        private Action _currentAction;
        private Planner _planner;
        private Queue<Action> _actionQueue;
        private Goal _currentGoal;
        private AnimationState _animationState;
        private Animator _animator;
        
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Locomotion = Animator.StringToHash("Locomotion");
        private static readonly int Speed = Animator.StringToHash("Speed");
        
        /// <summary>
        /// Finds the references
        /// </summary>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        /// <summary>
        /// Initializes the Agent's list of actions
        /// </summary>
        protected virtual void Start()
        {
            CreateActions();
        }
        
        /// <summary>
        /// Computes a plan of Action and performs each Action in the queue
        /// </summary>
        private void LateUpdate()
        {
            // If an action is running
            if(_currentAction is {Running: true})
            {
                // Performs the action if the agent has arrived to destination or if the current action does not have a target location
                var distanceToTarget = Vector3.Distance(_currentAction.Target, transform.position);
                
                if (_currentAction.HasTarget && (!_currentAction.NavMeshAgent.hasPath || !(distanceToTarget < 0.2f))) return;
                
                _currentAction.Perform();
                //SetAnimatorParameters();

                return;
            }

            // If the agent has no plan of action
            if(_planner == null || _actionQueue == null)
            {
                _planner = new Planner();
                
                // Sorts the goal states by priority (higher number = higher priority) from those that have a plan
                var sortedGoals = goals.OrderByDescending(g => g.Value);
                foreach(var goal in sortedGoals)
                {   
                    _actionQueue = _planner.Plan(_actions, goal.Key.StateSet, _beliefStates);

                    if (_actionQueue == null) continue;
                    
                    // Sets the agent's current goal as the first goal by priority order that can be reached through the agent's usable actions 
                    _currentGoal = goal.Key;
                    break;
                }
            }

            // Resets the planner if the goal state is reached to allow for a new plan for a new goal to be calculated
            if(_actionQueue is {Count: 0})
            {
                // Removes the current goal from the agent's set of goals if it is marked as to be removed once accomplished
                if (_currentGoal.Temporary)
                {
                    goals.Remove(_currentGoal);
                }

                _planner = null;
            }
        
            if (_actionQueue == null || _actionQueue.Count <= 0) return;
            
            // If the action queue is not empty, dequeues the next actions and sets it as the current action
            _currentAction = _actionQueue.Dequeue();
            // If the current action's pre-perform conditions have been met
            if(_currentAction.PrePerform())
            {
                // If the current action has no target
                if(!_currentAction.HasTarget)
                {
                    _currentAction.Running = true;
                    //SetAnimatorParameters();

                    return;
                }

                if (_currentAction.Target == Vector3.zero)
                {
                    Debug.LogWarning("You most likely forgot to set a target for " + _currentAction.Name + "!");
                }
                
                // If the current action's destination location is not the zero vector, makes the agent go towards its target location
                _currentAction.Running = true;
                _currentAction.NavMeshAgent.SetDestination(_currentAction.Target);
                //SetAnimatorParameters();
            }
            // Resets the planner to allow for a new plan for a new goal to be calculated
            else
            {
                _actionQueue = null;
            }
        }

        /// <summary>
        /// Triggers the <paramref name="animatorParameter"/>
        /// </summary>
        /// <param name="animatorParameter"></param>
        public void SetAnimatorParameter(string animatorParameter)
        {
            _animator.SetTrigger(animatorParameter);
        }

        /// <summary>
        /// Completes the current action, leaving place to the next one
        /// </summary>
        public void CompleteAction()
        {
            _currentAction.Running = false;
            if(_currentAction.PostPerform() == false)
            {
                _planner = null;
                _actionQueue = null;    
            }
            
            //SetAnimatorParameters();
        }

        /// <summary>
        /// Interrupts the current goal to allow computation of a new one
        /// </summary>
        public void InterruptGoal()
        {
            _actionQueue = null;
            _currentAction.Running = false;
            _currentAction = null;
        }
        
        /// <summary>
        /// Initializes the Agent's actions from its list of ActionData
        /// </summary>
        private void CreateActions()
        {
            foreach(var data in _actionsData)
            {
                data.Create(this);
            }
        }

        /// <summary>
        /// Sets the agent's animator parameters
        /// </summary>
        private void SetAnimatorParameters() // TODO Uncomment calls as soon as the agent has an animator controller
        {
            _animator.SetFloat(Speed, GetComponent<NavMeshAgent>().speed);
            switch(_animationState)
            {
                case AnimationState.Idle:
                    _animator.SetBool(Idle, true);
                    _animator.SetBool(Locomotion, false);
                    break;
                case AnimationState.Locomotion:
                    _animator.SetBool(Idle, false);
                    _animator.SetBool(Locomotion, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        // Properties
        
        public List<Action> Actions => _actions;

        public StateSet BeliefStates => _beliefStates;

        public AnimationState AnimationState
        {
            get => _animationState;
            set => _animationState = value;
        }
    }
}

/// <summary>
/// The possible animation states
/// </summary>
public enum AnimationState { Idle, Locomotion }
