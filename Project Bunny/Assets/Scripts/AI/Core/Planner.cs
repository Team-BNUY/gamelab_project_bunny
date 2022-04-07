using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Core
{
    public class Planner
    {
        /// <summary>
        /// Creates a plan of Action out of the achievable <paramref name="actions"/> to reach the <paramref name="goalStates"/>
        /// </summary>
        /// <param name="actions">The possible actions</param>
        /// <param name="goalStates">The goal's StateSet</param>
        /// <param name="beliefStates">The beliefs of the agent</param>
        /// <returns></returns>
        public Queue<Action> Plan(List<Action> actions, StateSet goalStates, StateSet beliefStates)
        {
            // Adds all achievable actions to the usable actions
            var usableActions = actions.Where(a => a.IsAchievable()).ToList();

            // Debug.LogError("USABLE ACTIONS + AMOUNT: " + actions.Count);
            // foreach (var action in actions)
            // {
            //     Debug.LogError("ACTION: " + action.Name);
            // }
            //
            // Debug.LogError("GOAL STATES + AMOUNT: " + goalStates.States.Count());
            // foreach (var state in goalStates.States)
            // {
            //     Debug.LogError("GOAL STATE: " + state.Key);
            // }
            //
            // Debug.LogError("BELIEF STATES + AMOUNT: " + beliefStates.States.Count());
            // foreach (var state in beliefStates.States)
            // {
            //     Debug.LogError("BELIEF STATE: " + state.Key);
            // }
            
            // Initiates the empty list of leaves to pass by reference and the initial node of the graph (tree)
            var leaves = new List<Node>();
            var start = new Node(null, 0, World.Instance.States, beliefStates, null);
            
            //Debug.LogError("START NODE EXISTS: " + start.Action);

            // Builds the graph, checks whether or not a path is found to the goal state and extracts the leaves if so
            var success = BuildGraph(start, ref leaves, usableActions, goalStates);

            // If no path to the goal state was found
            if(success == false) return null;

            // Determines the leaf with the cheapest path cost
            Node cheapest = null;
            foreach (var leaf in leaves.Where(leaf => cheapest == null || leaf.Cost < cheapest.Cost))
            {
                cheapest = leaf;
            }

            // Back-propagates from the cheapest leaf to the root to extract the actions to perform form the agent's current state to the goal state
            var result = new List<Action>();
            var node = cheapest;
            while(node != null)
            {
                if (node.Action != null)
                {
                    result.Insert(0, node.Action);
                }
                
                node = node.Parent;
            }

            // Creates a queue out of the resulting actions to reach the goal state
            var queue = new Queue<Action>();
            foreach (var action in result)
            {
                queue.Enqueue(action);
            }

            return queue;
        }

        /// <summary>
        /// Builds all possible graphs out of the usable actions to find the goal state
        /// </summary>
        /// <param name="parent">The current node</param>
        /// <param name="leaves">The leaves of the graph</param>
        /// <param name="usableActions">The list of actions the agent can perform</param>
        /// <param name="goalStates">The goal state to reach by the agent</param>
        /// <returns>True if the goal is achievable (if a node with states containing the goal state may be reached within the graph)</returns>
        private bool BuildGraph(Node parent, ref List<Node> leaves, List<Action> usableActions, StateSet goalStates)
        {
            // Debug.LogError("PASSED PARENT: " + parent.Action);
            //
            // Debug.LogError("REFERENCED LEAVES + COUNT: " + leaves.Count);
            // foreach (var leaf in leaves)
            // {
            //     Debug.LogError("REFERENCED LEAF: " + leaf.Action.Name);
            // }
            //
            // Debug.LogError("PASSED USABLE ACTIONS + COUNT: " + usableActions.Count);
            // foreach (var action in usableActions)
            // {
            //     Debug.LogError("PASSED USABLE ACTION: " + action.Name);
            // }
            //
            // Debug.LogError("PASSED GOALS + COUNT: " + goalStates.States.Count());
            // foreach (var goal in goalStates.States)
            // {
            //     Debug.LogError("PASSED GOALS: " + goal.Key);
            // }
            //
            // Debug.LogError("ACHIEVABLE GIVEN TEST");
            // foreach (var action in usableActions)
            // {
            //     Debug.LogError("ACTION " + action.Name + " IS ACHIEVABLE GIVEN? " + action.IsAchievableGiven(parent.States));
            // }
            
            var foundPath = false;
            foreach (var action in usableActions.Where(a => a.IsAchievableGiven(parent.States)))
            {
                // Creates a StateSet out of the root node's states and adds the after effect states of the action to create the next node
                var currentState = new StateSet(parent.States);
                foreach (var state in action.AfterEffectStates.States)
                {
                    currentState.AddState(state);
                }

                // Creates the next node
                var node = new Node(parent, parent.Cost + action.Cost, currentState, action);

                // Sets the next node as a leaf if a path to the goal is found
                if(GoalAchieved(goalStates, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                // Removes the current action for the set of usable actions and continues building the graph to find a goal state
                else
                {
                    var subset = ActionSubset(usableActions, action);
                    var found = BuildGraph(node, ref leaves, subset, goalStates);

                    if (found)
                    {
                        foundPath = true;
                    }
                }
            }
            
            return foundPath;
        }

        /// <summary>
        /// Checks whether or not the goal is met in the current node's states
        /// </summary>
        /// <param name="goalStates">The goal states</param>
        /// <param name="states">The current states of the node</param>
        /// <returns>True if the goal is met in the current node's states</returns>
        private bool GoalAchieved(StateSet goalStates, StateSet states)
        {
            return goalStates.States.All(g => states.HasState(g.Key));
        }

        /// <summary>
        /// Removes the action of the parent's node from the list of actions used for the next node
        /// </summary>
        /// <param name="actions">All the current usable actions</param>
        /// <param name="removeMe">The action to remove from the usable actions list</param>
        /// <returns>The difference subset between the actions set and the action to remove</returns>
        private List<Action> ActionSubset(IEnumerable<Action> actions, Action removeMe)
        {
            return actions.Where(action => !action.Equals(removeMe)).ToList();
        }
    }
}
