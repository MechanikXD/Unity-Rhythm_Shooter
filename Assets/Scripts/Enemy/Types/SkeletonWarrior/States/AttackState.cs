using System.Collections.Generic;
using Core.Music;
using Core.Music.Sequence;
using Core.Music.Sequence.Components;
using Enemy.Base;
using Interactable.Damageable;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.Types.SkeletonWarrior.States {
    public class AttackState : EnemyState {
        private readonly ActionSequence _comboSequence;
        private readonly LayerMask _playerMask;

        private Vector3 _destination;
        private float _moveSpeed;

        private readonly string _windUpAnimationKey;
        private readonly float[] _forwardMovement = { 1.224f, 1.888f, 0.4048f };
        private const float ExitAnimationBackMovement = 0.6601f;
        private const float AttackDistance = 1.5f;
        
        public AttackState(EnemyBase enemy, IReadOnlyList<AnimationClip> animations) : base(enemy) {
            _windUpAnimationKey = animations[0].name;
            var comboAnimation = new[] { animations[1], animations[2], animations[3] };
            
            _playerMask = LayerMask.NameToLayer("Ignore Player");
            var sequenceBuilder = new ActionSequenceBuilder();

            var crotchet = Conductor.Instance.SongData.Crotchet;
            
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(comboAnimation[0].name);
                _moveSpeed = _forwardMovement[0] / crotchet;
                _destination += Enemy.Forward * _forwardMovement[0];
                AttackPlayer();
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(comboAnimation[1].name);
                _moveSpeed = _forwardMovement[1] / crotchet;
                _destination += Enemy.Forward * _forwardMovement[1];
                AttackPlayer();
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(comboAnimation[2].name);
                _moveSpeed = _forwardMovement[2] / crotchet;
                _destination += Enemy.Forward * _forwardMovement[2];
                AttackPlayer();
            });
            sequenceBuilder.Append(Trigger.NextBeat, _ => {
                Enemy.PlayAnimation(animations[4].name);
                _moveSpeed = ExitAnimationBackMovement / animations[4].length;
                _destination -= Enemy.Forward * ExitAnimationBackMovement;
                
                // Random 50/50 between idle and retreat
                var outState = Random.value <= 0.5f ? typeof(WarriorIdle) : typeof(Retreat);
                Enemy.StartCoroutine(ForceExitStateAfter(animations[4].length, outState));
            });

            _comboSequence = sequenceBuilder.ToSequence();
        }

        public override void EnterState() {
            _destination = Enemy.Position;
            PlayOrRestartSequence();
            Enemy.Rotation.LookAt(Enemy.PlayerTransform.position);
            Enemy.PlayAnimation(_windUpAnimationKey);
        }

        private void PlayOrRestartSequence() {
            if (_comboSequence.IsFinished) _comboSequence.Restart();
            else _comboSequence.Start();
        }

        public override void FrameUpdate() {
            if (Enemy.NearPoint(_destination, 0.5f)) return;

            Enemy.transform.position = Vector3.MoveTowards(Enemy.Position, _destination,
                _moveSpeed * Time.deltaTime);
        }

        private void AttackPlayer() {
            if (!Physics.Raycast(Enemy.Position, Vector3.forward, out var hit, AttackDistance,
                    _playerMask)) return;

            if (!hit.transform.gameObject.TryGetComponent<PlayerController>(out var player)) return;

            var info = DamageInfoBuilder.EnemyOnPlayer(Enemy);
            player.TakeDamage(info);
        }
    }
}