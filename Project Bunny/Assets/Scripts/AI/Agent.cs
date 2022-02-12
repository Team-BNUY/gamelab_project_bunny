using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class Agent : MonoBehaviour
    {
        [SerializeField] 
        private List<ActionData> actionsData;

        private Dictionary<Goal, int> goals = new Dictionary<Goal, int>();
        private List<Action> actions = new List<Action>();
        private StateSet beliefStates = new StateSet();
        private Action currentAction;
        private Planner planner;
        private Queue<Action> actionQueue;
        private Goal currentGoal;
        private AnimationState animationState;
        private Animator animator;
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Locomotion = Animator.StringToHash("Locomotion");
        private static readonly int Speed = Animator.StringToHash("Speed");
        
        /// <summary>
        /// Finds the references
        /// </summary>
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        
        /// <summary>
        /// Initializes the Agent's actions from its list of ActionData
        /// </summary>
        private void Start()
        {
            foreach(var data in actionsData)
            {
                data.Create(this);
            }
        }
        
        /// <summary>
        /// Computes a plan of Action and performs each Action in the queue
        /// </summary>
        private void LateUpdate()
        {
            // If an action is running
            if(currentAction is {Running: true})
            {
                // Performs the action if the agent has arrived to destination or if the current action does not have a target location
                var distanceToTarget = Vector3.Distance(currentAction.Target, transform.position);
                
                if (currentAction.HasTarget && (!currentAction.NavMeshAgent.hasPath || !(distanceToTarget < 0.2f))) return;
                
                currentAction.Perform();
                SetAnimatorParameters();

                return;
            }

            // If the agent has no plan of action
            if(planner == null || actionQueue == null)
            {
                planner = new Planner();

                // Sorts the goal states by priority (higher number = higher priority) from those that have a plan
                var sortedGoals = goals.OrderByDescending(g => g.Value);
                foreach(var goal in sortedGoals)
                {   
                    actionQueue = planner.Plan(actions, goal.Key.StateSet, beliefStates);

                    if (actionQueue == null) continue;
                    
                    // Sets the agent's current goal as the first goal by priority order that can be reached through the agent's usable actions 
                    currentGoal = goal.Key;
                    break;
                }
            }

            // Resets the planner if the goal state is reached to allow for a new plan for a new goal to be calculated
            if(actionQueue is {Count: 0})
            {
                // Removes the current goal from the agent's set of goals if it is marked as to be removed once accomplished
                if (currentGoal.Temporary)
                {
                    goals.Remove(currentGoal);
                }

                planner = null;
            }
        
            if (actionQueue == null || actionQueue.Count <= 0) return;
            
            // If the action queue is not empty, dequeues the next actions and sets it as the current action
            currentAction = actionQueue.Dequeue();
            // If the current action's pre-perform conditions have been met
            if(currentAction.PrePerform())
            {
                // If the current action has no target
                if(!currentAction.HasTarget)
                {
                    currentAction.Running = true;
                    SetAnimatorParameters();
                }

                // If the current action's destination location is a game object with a tag instead of a global position, sets the target location as the tagged game object's position
                if (currentAction.Target == Vector3.zero && !string.IsNullOrWhiteSpace(currentAction.TargetTag))
                {
                    currentAction.Target = GameObject.FindWithTag(currentAction.TargetTag).transform.position;
                }

                if (currentAction.Target == Vector3.zero) return;
                
                // If the current action's destination location is not the zero vector, makes the agent go towards its target location
                currentAction.Running = true;
                currentAction.NavMeshAgent.SetDestination(currentAction.Target);
                SetAnimatorParameters();
            }
            // Resets the planner to allow for a new plan for a new goal to be calculated
            else
            {
                actionQueue = null;
            }
        }

        /// <summary>
        /// Triggers the <paramref name="animatorParameter"/>
        /// </summary>
        /// <param name="animatorParameter"></param>
        public void SetAnimatorParameter(string animatorParameter)
        {
            animator.SetTrigger(animatorParameter);
        }

        /// <summary>
        /// Completes the current action, leaving place to the next one
        /// </summary>
        public void CompleteAction()
        {
            currentAction.Running = false;
            if(currentAction.PostPerform() == false)
            {
                planner = null;
                actionQueue = null;    
            }
            
            SetAnimatorParameters();
        }

        /// <summary>
        /// Interrupts the current goal to allow computation of a new one
        /// </summary>
        public void InterruptGoal()
        {
            actionQueue = null;
            currentAction.Running = false;
            currentAction = null;
        }
    
        /// <summary>
        /// Sets the agent's animator parameters
        /// </summary>
        private void SetAnimatorParameters()
        {
            animator.SetFloat(Speed, GetComponent<NavMeshAgent>().speed);
            switch(animationState)
            {
                case AnimationState.Idle:
                    animator.SetBool(Idle, true);
                    animator.SetBool(Locomotion, false);
                    break;
                case AnimationState.Locomotion:
                    animator.SetBool(Idle, false);
                    animator.SetBool(Locomotion, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        // Properties
        
        public List<Action> Actions => actions;

        public StateSet BeliefStates => beliefStates;

        public AnimationState AnimationState
        {
            get => animationState;
            set => animationState = value;
        }
    }
}

/// <summary>
/// The possible animation states
/// </summary>
public enum AnimationState { Idle, Locomotion }
