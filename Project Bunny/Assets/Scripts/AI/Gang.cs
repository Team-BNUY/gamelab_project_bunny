using System.Collections.Generic;
using System.Linq;
using AI.Agents;
using UnityEngine;

namespace AI
{
    public class Gang
    {
        private List<Student> _members;

        public Gang(Student student)
        {
            _members = new List<Student> { student };
        }

        public void Join(Student student)
        {
            var previousGang = Student.Gangs.Find(g => student.Gang == g);
            previousGang?.RemoveStudent(student);

            student.Gang = this;
            _members.Add(student);
        }

        private void RemoveStudent(Student student)
        {
            if (_members.Contains(student))
            {
                _members.Remove(student);
            }
        }
        
        // Properties
        
        public bool Full
        {
            get => _members.Count <= 4;
        }

        public float Radius
        {
            get => _members.Count / 1.5f;
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
