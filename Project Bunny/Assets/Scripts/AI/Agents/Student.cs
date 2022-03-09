using System.Collections.Generic;
using AI.Core;
using UnityEngine;

namespace AI.Agents
{
    public class Student : Agent
    {
        public static List<Gang> Gangs;

        [SerializeField] private LayerMask _groundLayer;
        private Gang _gang;
        
        protected override void Start()
        {
            // References
            Gangs ??= new List<Gang>(); // TODO Take from an eventual GameManager
            _gang ??= new Gang(this);
            if (!Gangs.Contains(_gang))
            {
                Gangs.Add(_gang);
            }
            
            // Creating actions
            base.Start();
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
    }
}
