using System.Collections.Generic;
using AI.Core;
using Networking;
using Photon.Pun;
using Player;
using UnityEngine;

namespace AI.Agents
{
    public class Student : Agent
    {
        [Header("Individual")]
        [SerializeField] [Min(0f)] private float pushForce = 10f;
        [SerializeField] private Collider _physicsCollider;
        private readonly List<string> _movingActions = new List<string> { "Intimidate", "Join Another Gang", "Cry", "AnimationAction" };
        
        [Header("Audio")]
        [SerializeField] private AudioClip _hitBySnowballSound;
        [SerializeField] private AudioClip _hitByIceBallSound;
        [SerializeField] private AudioClip _hitByRollBallSound;
        [SerializeField] private AudioClip _hurtSound;
        private AudioSource _audioSource;
        
        public Gang Gang { get; set; }
        public bool Occupied => Gang.Occupied;
        public Collider PhysicsCollider => _physicsCollider;
        
        protected override void Awake()
        {
            photonView.OwnershipTransfer = OwnershipOption.Takeover;
            _audioSource = GetComponent<AudioSource>();
            
            base.Awake();
        }

        protected override void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

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
            
            state = new State("stoodUp", 1);
            states = new StateSet(state);
            goal = new Goal(states, false);
            goals.Add(goal, 6);

            // Creating actions
            base.Start();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var projectile = collision.gameObject.CompareTag("Projectile");
            if (!projectile) return;
            
            if (collision.gameObject.TryGetComponent<NetworkGiantRollball>(out var rollBall) && rollBall.CanDamage)
            {
                photonView.RPC(nameof(GetHitByGiantRollBall), RpcTarget.All);
                PlayHitAudio(2);
                
                return;
            }
            
            // Hit by any projectile
            photonView.RPC(nameof(GetHitByProjectile), RpcTarget.All);
            
            var snowball = collision.gameObject.GetComponent<NetworkSnowball>();
            if (!snowball) return;
            
            if (snowball.StudentThrower.photonView.IsMine)
            {
                ScoreManager.IncrementPropertyCounter(PhotonNetwork.LocalPlayer, ScoreManager.BullyKey);
            }
            
            // Hit by a snowball
            var thrower = snowball.StudentThrower;
            var throwDirection = thrower.transform.position - transform.position;
            var angle = Vector3.SignedAngle(transform.forward, throwDirection, Vector3.up);
            if (angle < 0 && angle >= -45f || angle >= 0 && angle < 45f)
            {
                SetAnimatorParameter("HitFront", true, true);
            }
            else if (angle < -45f && angle >= -135f)
            {
                SetAnimatorParameter("HitLeft", true, true);
            }
            else if (angle >= 45f && angle < 135f)
            {
                SetAnimatorParameter("HitRight", true, true);
            }
            else
            {
                SetAnimatorParameter("HitBack", true, true);
            }

            SetAnimatorParameter("Hit", true, true);
            PlayHitAudio(snowball.IsIceBall ? 1 : 0);
        }

        private void OnCollisionStay(Collision collision)
        {
            var otherStudent = collision.gameObject.GetComponent<Student>();
            if (!otherStudent || otherStudent.currentAction != null && _movingActions.Contains(otherStudent.currentAction.Name)) return;

            var pushVector = -collision.GetContact(0).normal;
            collision.rigidbody.AddForce(pushVector * pushForce, ForceMode.Impulse);
        }

        public void Unhit()
        {
            SetAnimatorParameter("Hit", false, true);
            SetAnimatorParameter("KnockedDown", false, true);
        }

        public void UnhitSides()
        {
            SetAnimatorParameter("HitFront", false, true);
            SetAnimatorParameter("HitBack", false, true);
            SetAnimatorParameter("HitLeft", false, true);
            SetAnimatorParameter("HitRight", false, true);
        }
        
        private void PlayHitAudio(int ballType)
        {
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(PlayHitAudioRpc), RpcTarget.All, ballType);
            }
        }

        [PunRPC]
        private void PlayHitAudioRpc(int ballType)
        {
            switch (ballType)
            {
                case 0:
                    _audioSource.PlayOneShot(_hitBySnowballSound, 2f * AudioManager.Instance.Volume);
                    break;
                case 1:
                    _audioSource.PlayOneShot(_hitByIceBallSound, 2f * AudioManager.Instance.Volume);
                    break;
                default:
                    _audioSource.PlayOneShot(_hitByRollBallSound, 2f * AudioManager.Instance.Volume);
                    break;
            }

            var randomPitch = Random.Range(-0.15f, 0.15f);
            _audioSource.clip = _hurtSound;
            _audioSource.volume = AudioManager.Instance.Volume * 0.5f;
            _audioSource.pitch =  1f + randomPitch;
            _audioSource.Play();
        }

        [PunRPC]
        private void GetHitByProjectile()
        {
            beliefStates.AddState("hitByProjectile", 1);
            InterruptGoal();
        }
        
        [PunRPC]
        private void GetHitByGiantRollBall()
        {
            _physicsCollider.enabled = false;
            SetAnimatorParameter("KnockedDown", true, true);
            beliefStates.AddState("hitByRollBall", 1);
            InterruptGoal();
        }

        [PunRPC]
        private void SetTriggerRPC(string trigger)
        {
            animator.SetTrigger(trigger);
        }
        
        [PunRPC]
        private void SetBoolRPC(string boolean, bool value)
        {
            animator.SetBool(boolean, value);
        }
    }
}
