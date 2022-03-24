using AI.Agents;
using AI.Core;
using Photon.Pun;
using Player;
using UnityEngine;

namespace AI.Actions.TeacherActions
{
    public class ChaseStudent : Action
    {
        private Teacher _teacher;
        private float _speed;
        private float _fieldfOfView;
        
        private NetworkStudentController _target;
        
        public ChaseStudent(string name, int cost, StateSet preconditionStates, StateSet afterEffectStates, Teacher teacher, bool hasTarget, float speed, float fieldfOfView)
             : base(name, cost, preconditionStates, afterEffectStates, teacher, hasTarget)
        {
            _teacher = teacher;
            _speed = speed;
            _fieldfOfView = fieldfOfView;
        }
        
        /// <summary>
        /// Sets Action's target as the Teacher's current student target
        /// </summary>
        /// <returns>True if the Teacher has a student target</returns>
        public override bool PrePerform()
        {
            // Resets parameters
            invoked = false;
            
            if (_teacher.TargetStudent)
            {
                _target = _teacher.TargetStudent;
            }
            
            if (!_target) return false;
            
            agent.AnimationState = AnimationState.Run;

            return true;
        }
        
        /// <summary>
        /// Sets the Teacher's field of view and chases the Action's target
        /// </summary>
        public override void Perform()
        {
            var targetPosition = _target.transform.position;
            var teacherPosition = _teacher.transform.position;
            var onYPlaneTargetPosition = new Vector3(targetPosition.x, teacherPosition.y, targetPosition.z);
            _teacher.LookAtDirection(onYPlaneTargetPosition - teacherPosition);
            _teacher.FieldOfView = _fieldfOfView;
            
            navMeshAgent.speed = _speed;
            navMeshAgent.SetDestination(targetPosition);

            if (Vector3.Distance(_target.transform.position, _teacher.transform.position) > 2f || _target.IsDead) return;
            
            _target.photonView.RPC("GetDamaged", RpcTarget.All, 3f);
            _teacher.BadStudents.Remove(_target);
            Teacher.LoseDeadPlayer(_target);
        }
        
        /// <summary>
        /// Resets the Action's target
        /// </summary>
        /// <returns>Always true, no possible failure for this post-processing</returns>
        public override bool PostPerform()
        {
            return success;
        }
    }
}
