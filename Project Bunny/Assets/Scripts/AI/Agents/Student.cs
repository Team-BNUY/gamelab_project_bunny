using System.Collections.Generic;
using AI.Core;
using Photon.Pun;
using UnityEngine;

namespace AI.Agents
{
    public class Student : Agent
    {
        [Header("Individual")] 
        [SerializeField] [Min(0f)] private float pushForce = 10f;
        
        private Gang _gang;
        private List<string> _movingActions = new List<string> {"Intimidate", "Join Another Gang", "Cry", "AnimationAction"};
        
        protected override void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
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
            
            photonView.RPC("GetHitByProjectile", RpcTarget.All);
        }

        private void OnCollisionStay(Collision collision)
        {
            var otherStudent = collision.gameObject.GetComponent<Student>();
            if (!otherStudent || otherStudent.currentAction != null && _movingActions.Contains(otherStudent.currentAction.Name)) return;
            
            var pushVector = -collision.GetContact(0).normal;
            collision.rigidbody.AddForce(pushVector * pushForce, ForceMode.Impulse);
        }

        [PunRPC]
        private void GetHitByProjectile()
        {
            beliefStates.AddState("hitByProjectile", 1);
            InterruptGoal();
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
