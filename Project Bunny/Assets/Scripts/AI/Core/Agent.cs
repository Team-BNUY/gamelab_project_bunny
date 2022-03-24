using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Core
{
    public class Agent : MonoBehaviourPunCallbacks
    {
        [Header("Testing")]
        public string action;
        
        [SerializeField] private List<ActionData> _actionsData;

        protected readonly Dictionary<Goal, int> goals = new Dictionary<Goal, int>();
        protected readonly StateSet beliefStates = new StateSet();
        protected Action currentAction;
        protected Animator animator;
        protected AnimationState animationState;

        private readonly List<Action> _actions = new List<Action>();
        private Planner _planner;
        private Queue<Action> _actionQueue;
        private Goal _currentGoal;
        
        private static readonly int Walking = Animator.StringToHash("Walking");
        private static readonly int Running = Animator.StringToHash("Running");

        /// <summary>
        /// Finds the references
        /// </summary>
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        
        /// <summary>
        /// Initializes the Agent's list of actions
        /// </summary>
        protected virtual void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            CreateActions();
        }
        
        /// <summary>
        /// Computes a plan of Action and performs each Action in the queue
        /// </summary>
        private void LateUpdate()
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

            action = currentAction?.Name;

            if (_currentGoal == null && currentAction == null)
            {
                animationState = AnimationState.Idle;
                SetAnimatorParameters();
            }

            // If an action is running
            if(currentAction is {Running: true})
            {
                // Performs the action if the agent has arrived to destination or if the current action does not have a target location
                var position = transform.position;
                var distanceToTarget = Vector2.Distance(new Vector2(currentAction.Target.x, currentAction.Target.z), new Vector2(position.x, position.z));
                
                if (currentAction.HasTarget && distanceToTarget > 0.2f) return;
                
                currentAction.Perform();
                SetAnimatorParameters();

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
                    _actionQueue = _planner.Plan(_actions, goal.Key.StateSet, beliefStates);

                    if (_actionQueue == null)
                    {
                        currentAction = null;
                        continue;
                    }
                    
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
            currentAction = _actionQueue.Dequeue();
            // If the current action's pre-perform conditions have been met
            if(currentAction.PrePerform())
            {
                // If the current action has no target
                if(!currentAction.HasTarget)
                {
                    currentAction.Running = true;

                    return;
                }

                if (currentAction.Target == Vector3.zero)
                {
                    Debug.LogWarning("You most likely forgot to set a target for " + currentAction.Name + "!");
                }
                
                // If the current action's destination location is not the zero vector, makes the agent go towards its target location
                currentAction.Running = true;
                currentAction.NavMeshAgent.SetDestination(currentAction.Target);
                SetAnimatorParameters();
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
        /// <param name="animatorParameter">The animator parameter's name</param>
        public void SetAnimatorParameter(string animatorParameter)
        {
            animator.SetTrigger(animatorParameter);
        }
        
        /// <summary>
        /// Sets the boolean <paramref name="animatorParameter"/> to <paramref name="value"/>
        /// </summary>
        /// <param name="animatorParameter">The animator parameter's name</param>
        /// <param name="value">The boolean value to set the animator parameter</param>
        public void SetAnimatorParameter(string animatorParameter, bool value)
        {
            animator.SetBool(animatorParameter, value);
        }
        
        /// <summary>
        /// Sets the integer <paramref name="animatorParameter"/> to <paramref name="value"/>
        /// </summary>
        /// <param name="animatorParameter">The animator parameter's name</param>
        /// <param name="value">The integer value to set the animator parameter</param>
        public void SetAnimatorParameter(string animatorParameter, int value)
        {
            animator.SetInteger(animatorParameter, value);
        }

        /// <summary>
        /// Completes the current action, leaving place to the next one
        /// </summary>
        public void CompleteAction()
        {
            if (currentAction == null) return;
            
            currentAction.Running = false;
            SetAnimatorParameters();
            
            if (currentAction.PostPerform()) return;
            
            _planner = null;
            _actionQueue = null;
        }

        /// <summary>
        /// Interrupts the current goal to allow computation of a new one
        /// </summary>
        protected void InterruptGoal()
        {
            _actionQueue = null;

            if (currentAction != null)
            {
                currentAction.Running = false;
                currentAction.OnInterrupt();
            }
           
            currentAction = null;
        }

        protected void SetAnimatorParameters()
        {
            switch(animationState)
            {
                case AnimationState.Idle:
                    animator.SetBool(Walking, false);
                    animator.SetBool(Running, false);
                    break;
                case AnimationState.Walk:
                    animator.SetBool(Walking, true);
                    animator.SetBool(Running, false);
                    break;
                case AnimationState.Run:
                    animator.SetBool(Running, true);
                    animator.SetBool(Walking, false);
                    break;
                default:
                    animator.SetBool(Walking, false);
                    animator.SetBool(Running, false);
                    break;
            }
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
        
        // Properties

        public Action CurrentAction
        {
            get => currentAction;
        }
        
        public List<Action> Actions
        {
            get => _actions;
        }

        public StateSet BeliefStates
        {
            get => beliefStates;
        }

        public AnimationState AnimationState
        {
            set => animationState = value;
        }
    }
}

/// <summary>
/// The possible animation states
/// </summary>
public enum AnimationState { Idle, Walk, Run }
