using System.Collections;
using Core.Behaviour.FiniteStateMachine;
using Core.Music;
using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using Enemy.States.Base;
using Interactable.Damageable;
using Player;
using UnityEngine;

namespace Enemy.Types.Skeleton.States {
    public class ComboState : EnemyState {
        private readonly ActionSequence _comboSequence;
        private readonly LayerMask _playerMask;

        private Vector3 _destination;
        private float _moveSpeed;

        private readonly string _windUpAnimationKey;
        private readonly float[] _forwardMovement = { 1.224f, 1.888f, 0.4048f };
        private const float ExitAnimationBackMovement = 0.6601f;
        private const float AttackDistance = 1.5f;
        
        public ComboState(StateMachine stateMachine, EnemyBase enemy, EnemyState[] outStates,
            AnimationClip[] animations)
            : base(stateMachine, enemy, outStates) {
            _windUpAnimationKey = animations[0].name;
            var comboAnimation = new[] { animations[1], animations[2], animations[3] };
            
            _playerMask = LayerMask.NameToLayer("Ignore Player");
            var sequenceBuilder = new ActionSequenceBuilder();

            var crotchet = Conductor.Instance.SongData.Crotchet;
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(comboAnimation[0].name);
                _moveSpeed = _forwardMovement[0] / crotchet;
                _destination += Enemy.Forward * _forwardMovement[0];
                AttackForward();
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(comboAnimation[1].name);
                _moveSpeed = _forwardMovement[1] / crotchet;
                _destination += Enemy.Forward * _forwardMovement[1];
                AttackForward();
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(comboAnimation[2].name);
                _moveSpeed = _forwardMovement[2] / crotchet;
                _destination += Enemy.Forward * _forwardMovement[2];
                AttackForward();
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(animations[4].name);
                _moveSpeed = ExitAnimationBackMovement / animations[4].length;
                _destination -= Enemy.Forward * ExitAnimationBackMovement;
                Enemy.StartCoroutine(ForceExitStateAfter(animations[4].length, 0));
            });

            _comboSequence = sequenceBuilder.ToSequence();
        }

        private IEnumerator ForceExitStateAfter(float delay, int stateIndex) {
            yield return new WaitForSeconds(delay);
            AttachedStateMachine.ChangeState(OutStates[stateIndex]);
        }


        public override void EnterState() {
            _destination = Enemy.Position;
            PlayOrRestartSequence();
            Enemy.Agent.updateRotation = false;
            Enemy.PlayAnimation(_windUpAnimationKey);
        }

        public override void ExitState() {
            Enemy.Agent.updateRotation = true;
            // Slight move forward to prevent rotation
            Enemy.Agent.SetDestination(Enemy.Position + Enemy.Forward * 0.01f);
        }

        private void PlayOrRestartSequence() {
            if (_comboSequence.IsFinished) _comboSequence.Restart();
            else _comboSequence.Start();
        }

        public override void FrameUpdate() {
            if (AtDestination()) return;

            Enemy.transform.position = Vector3.MoveTowards(Enemy.Position, _destination,
                _moveSpeed * Time.deltaTime);
        }

        private bool AtDestination() => 
            Vector3.Distance(Enemy.Position, _destination) < 0.5f;

        private void AttackForward() {
            if (!Physics.Raycast(Enemy.Position, Vector3.forward, out var hit, AttackDistance,
                    _playerMask)) return;

            if (!hit.transform.gameObject.TryGetComponent<PlayerController>(out var player)) return;

            var damageInfo = new DamageInfo(Enemy, player, Enemy.CurrentDamage,
                Enemy.Position, hit.point);
            player.TakeDamage(damageInfo);
        }
    }
}