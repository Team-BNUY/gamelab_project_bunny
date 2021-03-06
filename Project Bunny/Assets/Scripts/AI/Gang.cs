using System.Collections.Generic;
using System.Linq;
using AI.Agents;
using Arena;
using UnityEngine;

namespace AI
{
    public class Gang
    {
        // Events
        private delegate void StudentInteraction(Student student);
        private event StudentInteraction OnNewStudentJoined;

        private readonly List<Student> _members;
        private Student _newbie;
        private int _occupied;
        
        private Gang(Student student)
        {
            _members = new List<Student> { student };
            if (!ArenaManager.Instance.Gangs.Contains(this))
            {
                ArenaManager.Instance.Gangs.Add(this);
            }
            
            OnNewStudentJoined += WelcomeNewStudent;
        }

        public static void Found(Student student)
        {
            student.Gang?.Leave(student);
            student.Gang = new Gang(student);
        }
        
        public void Join(Student student)
        {
            var previousGang = ArenaManager.Instance.Gangs.Find(g => student.Gang == g);
            previousGang?.Leave(student);

            student.Gang = this;
            _members.Add(student);
            
            OnNewStudentJoined?.Invoke(student);
        }

        public Student GetRandomMemberExcept(Student student)
        {
            var otherMembers = _members.Where(m => m != student).ToArray();
            var random = Random.Range(0, otherMembers.Length);

            return otherMembers[random];
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
            
            if (_members.Count > 0) return;
            
            ArenaManager.Instance.Gangs.Remove(this);
            OnNewStudentJoined -= WelcomeNewStudent;
        }

        private void WelcomeNewStudent(Student newStudent)
        {
            _newbie = newStudent;
            foreach (var member in _members.Where(m => m != newStudent))  
            {
                var random = Random.Range(0, 3);
                member.BeliefStates.AddState(random == 0 ? "dislikesNewMember" : "likesNewMember", 1);
                member.BeliefStates.AddState("newStudentJoinedGang", 1);

                if (Full)
                {
                    PlayAlone(member);
                    return;
                }

                random = Random.Range(0, 8);
                switch (random)
                {
                    case 0:
                    case 1:
                        member.BeliefStates.ModifyState("suddenNeedToIntimidate", 1);
                        break;
                    case 2:
                    case 3:
                        PlayAlone(member);
                        break;
                    default:
                        member.BeliefStates.AddState("curiousAboutOthers", 1);
                        break;
                }
            }
        }

        private void PlayAlone(Student member)
        {
            member.BeliefStates.ModifyState("wantsToPlayAlone", 1);
            var random = Random.Range(0, 3);
            switch (random)
            {
                case 0:
                    member.BeliefStates.AddState("poleSeemsAttracting", 1);
                    break;
                case 1:
                case 2:
                    member.BeliefStates.AddState("wantsToSit", 1);
                    break;
            }
        }

        // Properties

        public List<Student> Members
        {
            get => _members;
        }
        
        public Student Newbie
        {
            get => _newbie;
        }
        
        public bool Occupied
        {
            get => _occupied != 0;
        }
        
        public bool Full
        {
            get => _members.Count >= ArenaManager.Instance.GangMaximumSize;
        }

        public int Size
        {
            get => _members.Count;
        }

        public float Radius
        {
            get
            {
                return _members.Count switch
                {
                    1 => 4,
                    2 => 5,
                    3 => 6,
                    4 => 6,
                    _ => 4
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
