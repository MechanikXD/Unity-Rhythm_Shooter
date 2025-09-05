using Core.Music;
using Enemy.Base;
using Enemy.Types.SkeletonArcher.States;
using UnityEngine;

namespace Enemy.Types.SkeletonArcher {
    public class SkeletonArcher : EnemyBase {
        [Header("Enemy Specific:")]
        [SerializeField] private Transform _arrowSpawnPoint;
        [SerializeField] private EnemyArrow _enemyAttack;

        [Header("Behaviour:")]
        [SerializeField] private float _idleTime = 1f;
        [SerializeField] private float _arrowHeightCorrection = 0.2f;
        [SerializeField] private Vector2 _repositionBounds = new Vector2(7f, 20f);
        
        [Header("Animations:")]
        [SerializeField] private string _idleAnimationKey = "Archer Idle";
        [SerializeField] private string _runAnimationKey = "Archer Run";
        [SerializeField] private string _deathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _attackStateEnter;
        [SerializeField] private AnimationClip _attackStateLoop;
        [SerializeField] private AnimationClip _attackStateExit;
        
        [Header("Sounds:")]
        [SerializeField] private AudioClip[] _arrowLoadSounds;
        [SerializeField] private AudioClip[] _arrowReleaseSounds;

        [Header("Animator Param Keys:")]
        [SerializeField] private string _attackStartSpeedKey = "AttackStartSpeed";

        private int _attackStartSpeed;

        protected override EnemyState[] InitializeStates() {
            var idleState = new Idle(this, _idleTime, _idleAnimationKey);
            var repositionState = new Reposition(this, _repositionBounds, _runAnimationKey);
            var attackState =
                new Attack(this, _arrowSpawnPoint, _enemyAttack, _arrowHeightCorrection, 
                    _attackStateEnter.name, _attackStateLoop.name, _attackStateExit, 
                    _arrowLoadSounds, _arrowReleaseSounds);

            return new EnemyState[] {
                idleState,
                repositionState,
                attackState
            };
        }

        protected override void UpdateAnimationSpeed() {
            _attackStartSpeed = Animator.StringToHash(_attackStartSpeedKey);
            
            _animator.SetFloat(_attackStartSpeed, 
                _attackStateEnter.length / Conductor.Instance.SongData.Crotchet);
        }
        
        public override void Die() {
            base.Die();
            _animator.CrossFade(_deathAnimationKey, _crossFade, -1, 0f);
            
            Destroy(gameObject, _deathAnimationKey.Length);
        }
    }
}