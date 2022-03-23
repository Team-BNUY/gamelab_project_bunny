using System;
using System.Collections.Generic;
using AI.Core;
using Photon.Pun;
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
        [SerializeField] private List<ActionSpot> actionSpots;
        
        private Gang _gang;
        private List<string> _movingActions = new List<string> {"Intimidate", "Join Another Gang", "Cry", "AnimationAction"};
        
        protected override void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
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

            state = new State("performedAnimationAction", 1);
            states = new StateSet(state);
            goal = new Goal(states, false);
            goals.Add(goal, 3);
            
            state = new State("cried", 1);
            states = new StateSet(state);
            goal = new Goal(states, false);
            goals.Add(goal, 4);
            
            state = new State("welcomedNewbie", 1);
            states = new StateSet(state);
            goal = new Goal(states, false);
            goals.Add(goal, 5);
            
            // Creating actions
            base.Start();
        }

        private void OnTriggerEnter(Collider other)
        {
            var projectile = other.CompareTag("Projectile");
            if (!projectile) return;
            
            beliefStates.AddState("hitByProjectile", 1);
            InterruptGoal();
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

        public List<ActionSpot> ActionSpots
        {
            get => actionSpots;
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
