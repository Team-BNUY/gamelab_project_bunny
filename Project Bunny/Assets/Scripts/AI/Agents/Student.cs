using System.Collections.Generic;
using AI.Core;
using UnityEngine;

namespace AI.Agents
{
    public class Student : Agent
    {
        public static List<Gang> Gangs;

        [Header("Individual")] 
        [SerializeField] [Min(0f)] private float pushForce = 10f;
        
        [Header("Layers")] // TODO Take everything below from an eventual GameManager
        [SerializeField] private LayerMask _studentLayer;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private List<CryingSpot> _cryingSpots;
        
        private Gang _gang;
        private List<string> _movingActions = new List<string> {"Intimidate", "Join Another Gang"};
        
        protected override void Start()
        {
            // References
            Gangs ??= new List<Gang>(); // TODO Take from an eventual GameManager
            Gang.Found(this);

            // Goals
            var state = new State("joinedAnotherGang", 1);
            var states = new StateSet(state);
            var goal = new Goal(states, false);
            goals.Add(goal, 1);

            state = new State("intimidatedSomeone", 1);
            states = new StateSet(state);
            goal = new Goal(states, false);
            goals.Add(goal, 2);
            
            state = new State("welcomedNewbie", 1);
            states = new StateSet(state);
            goal = new Goal(states, false);
            goals.Add(goal, 3);
            
            state = new State("cried", 1);
            states = new StateSet(state);
            goal = new Goal(states, false);
            goals.Add(goal, 4);
            
            // Creating actions
            base.Start();
        }

        private void OnCollisionStay(Collision collision)
        {
            var otherStudent = collision.gameObject.GetComponent<Student>();
            if (!otherStudent || otherStudent.currentAction != null && _movingActions.Contains(otherStudent.currentAction.Name)) return;
            
            var pushVector = -collision.GetContact(0).normal;
            collision.rigidbody.AddForce(pushVector * pushForce, ForceMode.Impulse);
        }

        public LayerMask StudentLayer
        {
            get => _studentLayer;
        }
        
        public LayerMask GroundLayer
        {
            get => _groundLayer;
        }

        public LayerMask ObstacleLayer
        {
            get => _obstacleLayer;
        }

        public List<CryingSpot> CryingSpots
        {
            get => _cryingSpots;
        }

        public Gang Gang
        {
            get => _gang;
            set => _gang = value;
        }

        public bool Occupied
        {
            get => _gang.Occupied;
        }
    }
}
