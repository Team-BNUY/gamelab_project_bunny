using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public abstract class Action
    {
        private string _name;
        private int _cost;
        private StateSet _preconditionStates, _afterEffectStates;
        private bool _hasTarget;
        private bool _running;

        protected NavMeshAgent navMeshAgent;
        protected Agent agent;
        protected Vector3 target;
        protected bool invoked;
        protected bool success = true;

        /// <summary>
        /// Constructor to initialize the members that need a non-default value
        /// </summary>
        /// <param name="name">The name of the Action</param>
        /// <param name="cost">The cost of the Action</param>
        /// <param name="preconditionStates">The preconditions that have to be met for the Action to be considered</param>
        /// <param name="afterEffectStates">The after effects of the Action for the Action chain</param>
        /// <param name="agent">The agent that can perform the action</param>
        /// <param name="hasTarget">Whether or not the Action has a target position to reach</param>
        /// <param name="targetTag">The tag of the Action's target transform to reach</param>
        public Action(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Agent agent, bool hasTarget)
        {
            _name = name;
            _cost = cost;
            _preconditionStates = preconditionStates;
            _afterEffectStates = afterEffectStates;
            _hasTarget = hasTarget;
            this.agent = agent;
            navMeshAgent = agent.GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Checks if the Action is doable by matching its preconditions to the given <paramref name="conditionStates"/> (of the parent node)
        /// </summary>
        /// <param name="conditionStates">The conditions of the parent node</param>
        /// <returns>True if the Action is achievable given the <paramref name="conditionStates"/></returns>
        public bool IsAchievableGiven(StateSet conditionStates)
        {
            return _preconditionStates.States.All(s => conditionStates.HasState(s.Key));
        }

        /// <summary>
        /// Determines if an Action is achievable
        /// </summary>
        /// <returns>True if the Action is achievable</returns>
        public virtual bool IsAchievable()
        {
            return true;
        }

        public abstract void Perform();

        public abstract bool PrePerform();
    
        public abstract bool PostPerform();

        /// <summary>
        /// Calculates the length of a nav mesh <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path that needs calculation</param>
        /// <returns>The length of the path</returns>
        public static float GetPathLength(NavMeshPath path)
        {
            var length = 0f;

            if (path.status == NavMeshPathStatus.PathInvalid || path.corners.Length <= 1) return length;
            
            for (var i = 1; i < path.corners.Length; ++i)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            return length;
        }

        /// <summary>
        /// Finds the closest object of type T on the nav mesh used by <paramref name="nmAgent"/>
        /// </summary>
        /// <param name="elements">The elements of objects to search in</param>
        /// <param name="nmAgent">The nav mesh agent that uses the nav mesh to calculate the path distance</param>
        /// <typeparam name="TP">The type of the object</typeparam>
        /// <returns>The closest object of type T on the nav mesh</returns>
        public static TP FindClosest<TP>(List<TP> elements, NavMeshAgent nmAgent) where TP : MonoBehaviour
        {
            var closest = default(TP);

            if (elements.Count == 0) return null;
            
            var smallestLength = int.MaxValue;
            foreach (var element in elements)
            {
                var path = new NavMeshPath();
                nmAgent.CalculatePath(element.transform.position, path);
                
                var pathLength = (int)GetPathLength(path);
                
                if (pathLength >= smallestLength) continue;
                
                smallestLength = pathLength;
                closest = element;
            }

            return closest;
        }
        
        // Properties

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public Vector3 Target
        {
            get => target;
            set => target = value;
        }

        public NavMeshAgent NavMeshAgent
        {
            get => navMeshAgent;
        }

        public StateSet AfterEffectStates
        {
            get => _afterEffectStates;
        }            

        public bool HasTarget
        {
            get => _hasTarget;
        }

        public bool Running
        {
            get => _running;
            set => _running = value;
        }

        public int Cost
        {
            get => _cost;
        }
    }
}
