using Core.Music;
using Enemy.Base;
using Enemy.Types.SkeletonWarrior.States;
using Interactable.AttackCollider;
using UnityEngine;

namespace Enemy.Types.SkeletonWarrior {
    public class SkeletonWarrior : EnemyBase {
        [Header("Enemy Specific:")]
        // First is windup, last is exit animation. the rest are actual attacks
        [SerializeField] private AnimationClip[] _attackPattern;
        [SerializeField] private EnemyAttackCollider _attackCollider;

        [Header("Behaviour")]
        [SerializeField] private float _idleTime;
        [SerializeField] private float _retreatSpeedMultiplier;
        [SerializeField] private float[] _forwardMovementDuringAttack = 
            { 1.224f, 1.888f, 0.4048f, -0.6601f };
        [SerializeField] private Vector2 _fleeBounds = new Vector2(3f, 5f);
        
        // ReSharper disable StringLiteralTypo
        [Header("Animation Keys:")]
        [SerializeField] private string _idleAnimationKey = "Skele Idle";
        [SerializeField] private string _walkAnimationKey = "Skele Walk";
        [SerializeField] private string _deathAnimationKey = "Skely Death";
        // ReSharper restore StringLiteralTypo

        [Header("Animator param keys:")]
        [SerializeField] private string _firstAttackSpeedKey = "Combo1Speed";
        [SerializeField] private string _secondAttackSpeedKey = "Combo2Speed";
        [SerializeField] private string _thirdAttackSpeedKey = "Combo3Speed";
        private int _attack1SpeedHash;
        private int _attack2SpeedHash;
        private int _attack3SpeedHash;

        protected override EnemyState[] InitializeStates() {
            var idleState = new Idle(this, _idleTime, _idleAnimationKey);
            var retreat = new Retreat(this, _fleeBounds, _retreatSpeedMultiplier, _walkAnimationKey);
            var attackState = new Attack(this, _attackPattern, _attackCollider, _forwardMovementDuringAttack);
            var chaseState = new ChasePlayer(this, _walkAnimationKey);

            return new EnemyState[] {
                idleState,
                retreat,
                attackState,
                chaseState
            };
        }

        private void UpdateAnimatorParamHashes() {
            _attack1SpeedHash = Animator.StringToHash(_firstAttackSpeedKey);
            _attack2SpeedHash = Animator.StringToHash(_secondAttackSpeedKey);
            _attack3SpeedHash = Animator.StringToHash(_thirdAttackSpeedKey);
        }

        protected override void UpdateAnimationSpeed() {
            UpdateAnimatorParamHashes();
            var crotchet = Conductor.Instance.SongData.Crotchet;
            _animator.SetFloat(_attack1SpeedHash, _attackPattern[1].length / crotchet);
            _animator.SetFloat(_attack2SpeedHash, _attackPattern[2].length / crotchet);
            _animator.SetFloat(_attack3SpeedHash, _attackPattern[3].length / crotchet);
        }

        public override void Die() {
            base.Die();
            _animator.CrossFade(_deathAnimationKey, _crossFade, -1, 0f);
            
            Destroy(gameObject, _deathAnimationKey.Length);
        }
    }
}