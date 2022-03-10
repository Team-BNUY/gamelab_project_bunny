using System;
using System.Collections.Generic;
using System.Linq;
using AI.Core;
using UnityEngine;
using Action = AI.Core.Action;

namespace AI.Agents
{
    public class Student : Agent
    {
        public static List<Gang> Gangs;

        [SerializeField] private LayerMask _studentLayer; // TODO Take from an eventual GameManager
        [SerializeField] private LayerMask _groundLayer; // TODO Take from an eventual GameManager
        private Gang _gang;
        private bool _occupied;
        
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
            
            state = new State("welcomedNewbie", 1);
            states = new StateSet(state);
            goal = new Goal(states, false);
            goals.Add(goal, 2);
            
            // Creating actions
            base.Start();
        }

        public LayerMask StudentLayer
        {
            get => _studentLayer;
        }
        
        public LayerMask GroundLayer
        {
            get => _groundLayer;
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
