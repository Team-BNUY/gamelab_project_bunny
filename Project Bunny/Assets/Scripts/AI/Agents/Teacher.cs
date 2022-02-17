using System.Collections.Generic;
using Input;
using UnityEngine;

namespace AI.Agents
{
    public class Teacher : Agent
    {
        [SerializeField] private List<StudentController> _badStudents = new List<StudentController>();

        protected override void Start()
        {
            var sentToTimeout = new State("caughtStudent", 1);
            var states = new StateSet(sentToTimeout);
            var goal = new Goal(states, false);
            goals.Add(goal, 1);
            
            base.Start();
        }
        
        // Properties
        
        public List<StudentController> BadStudents => _badStudents;
    }
}
