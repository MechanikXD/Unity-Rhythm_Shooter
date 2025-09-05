using Core.Music;
using Enemy.Base;
using Enemy.Types.SkeletonTank.States;
using Interactable.AttackCollider;
using UnityEngine;

namespace Enemy.Types.SkeletonTank {
    public class SkeletonTank : EnemyBase {
        [Header("Enemy Specific:")]
        [SerializeField] private EnemyAttackCollider _attackCollider;

        [Header("Behaviour:")]
        [SerializeField] private float _normalIdleTime = 1.5f;

        [SerializeField] private float _shieldingTime = 3f;

        [Header("Animations:")]
        [SerializeField] private string _idleAnimationKey = "Paladin Idle";

        [SerializeField] private string _walkAnimationKey = "Paladin Walk";
        [SerializeField] private string _deathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _blockStart;
        [SerializeField] private string _blockLoopKey = "Block Loop";
        [SerializeField] private string _blockExitKey = "Block Exit";

        [SerializeField] private AnimationClip _attackWindUpAnimation;
        [SerializeField] private AnimationClip _attackAnimation;

        [Header("Animator Param Keys:")]
        [SerializeField] private string _slamWindUpSpeedKey = "SlamWindUp";

        [SerializeField] private string _slamAttackSpeedKey = "SlamAttack";
        private int _slamWindUp;
        private int _slamAttack;

        [Header("Sounds:")]
        [SerializeField] private AudioClip[] _swingSounds;

        protected override EnemyState[] InitializeStates() {
            var idleState = new Idle(this, _normalIdleTime, _idleAnimationKey);
            var shieldIdle = new Shielding(this, _shieldingTime, _blockStart, _blockLoopKey,
                _blockExitKey);
            var attackState = new Attack(this, _attackWindUpAnimation.name, _attackAnimation.name,
                _attackCollider, _swingSounds);
            var walkToPlayer = new ChasePlayer(this, _walkAnimationKey);

            return new EnemyState[] {
                idleState,
                shieldIdle,
                attackState,
                walkToPlayer
            };
        }

        private void UpdateAnimatorParams() {
            _slamWindUp = Animator.StringToHash(_slamWindUpSpeedKey);
            _slamAttack = Animator.StringToHash(_slamAttackSpeedKey);
        }

        protected override void UpdateAnimationSpeed() {
            UpdateAnimatorParams();
            var crotchet = Conductor.Instance.SongData.Crotchet;

            Animator.SetFloat(_slamWindUp, _attackWindUpAnimation.length / crotchet);
            Animator.SetFloat(_slamAttack, _attackAnimation.length / (2 * crotchet));
        }

        public override void Die() {
            base.Die();
            _animator.CrossFade(_deathAnimationKey, _crossFade, -1, 0f);

            Destroy(gameObject, _deathAnimationKey.Length);
        }
    }
}