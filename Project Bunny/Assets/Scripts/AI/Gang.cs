using System.Collections.Generic;
using System.Linq;
using AI.Agents;
using UnityEngine;

namespace AI
{
    public class Gang
    {
        private List<Student> _members;
        private int _occupied;

        private Gang(Student student)
        {
            _members = new List<Student> { student };
            if (!Student.Gangs.Contains(this))
            {
                Student.Gangs.Add(this);
            }
        }

        public static void Found(Student student)
        {
            student.Gang?.Leave(student);
            student.Gang = new Gang(student);
        }
        
        public void Join(Student student)
        {
            var previousGang = Student.Gangs.Find(g => student.Gang == g);
            previousGang?.Leave(student);

            student.Gang = this;
            _members.Add(student);
        }
        
        public void InteractWith()
        {
            _occupied++;
        }
        
        public void SetFree()
        {
            if (_occupied > 0)
            {
                _occupied--;
            }
        }

        private void Leave(Student student)
        {
            if (!_members.Contains(student)) return;
            
            _members.Remove(student);
            if (_members.Count == 0)
            {
                Student.Gangs.Remove(this);
            }
        }

        // Properties

        public bool Occupied
        {
            get => _occupied != 0;
        }
        
        public bool Full
        {
            get => _members.Count > 4;
        }

        public float Radius
        {
            get
            {
                return _members.Count switch
                {
                    1 => 2,
                    2 => 3,
                    3 => 4,
                    4 => 4,
                    _ => 2
                };
            }
        }

        public Vector3 Center
        {
            get
            {
                var positionsSum = _members.Aggregate(Vector3.zero, (sum, member) => sum + member.transform.position);
                return positionsSum / _members.Count;
            }
        }
    }
}
