using Core.Music;
using Enemy.Base;
using Enemy.States.Base;
using Enemy.States.Shared;
using Enemy.Types.SkeletonWarrior.States;
using UnityEngine;

namespace Enemy.Types.SkeletonWarrior {
    public class SkeletonMelee : EnemyBase {
        private IdleState _idleState;
        private AttackState _attackState;
        private ChasePlayer _chaseState;
        private StepBack _stepBackState;
        
        private const string IdleAnimationKey = "Skele Idle";
        private const string WalkAnimationKey = "Skele Walk";
        private const string DeathAnimationKey = "Skely Death";

        [SerializeField] private AnimationClip _windUp;
        [SerializeField] private AnimationClip _combo1;
        [SerializeField] private AnimationClip _combo2;
        [SerializeField] private AnimationClip _combo3;
        [SerializeField] private AnimationClip _fromCombo;
        
        private readonly static int Combo1Speed = Animator.StringToHash("Combo1Speed");
        private readonly static int Combo2Speed = Animator.StringToHash("Combo2Speed");
        private readonly static int Combo3Speed = Animator.StringToHash("Combo3Speed");

        protected override void Awake() {
            base.Awake();
            
            _idleState = new IdleState(EnemyStateMachine, this, null, 2, IdleAnimationKey);
            EnemyStateMachine.Initialize(_idleState);
        }

        protected void Start() {
            _stepBackState = new StepBack(EnemyStateMachine, this, null, _moveSpeed, WalkAnimationKey);
            
            _attackState = new AttackState(EnemyStateMachine, this,
                new EnemyState[] { _idleState, _stepBackState },
                new[] { _windUp, _combo1, _combo2, _combo3, _fromCombo });
            
            _chaseState = new ChasePlayer(EnemyStateMachine, this, 
                new EnemyState[] { _attackState }, WalkAnimationKey);
            
            _idleState.SetOutStates(new EnemyState[] { _attackState, _chaseState });
            _stepBackState.SetOutStates(new EnemyState[] {_idleState, _chaseState});
            
            var crotchet = Conductor.Instance.SongData.Crotchet;
            _animator.SetFloat(Combo1Speed, _combo1.length / crotchet);
            _animator.SetFloat(Combo2Speed, _combo2.length / crotchet);
            _animator.SetFloat(Combo3Speed, _combo3.length / crotchet);
        }

        protected override void EnterParriedState() { }

        protected override void UpdateMoveSpeedOnCharacter() {
            _chaseState.SetMoveSpeed(CurrentSpeed);
            _stepBackState.SetMoveSpeed(CurrentSpeed);
        }

        public override void Die() {
            _animator.CrossFade(DeathAnimationKey, _crossFade, -1, 0f);
            var info = new EnemyDefeatedInfo(this.GetType(), GetInstanceID(), Position, IsTarget);
            
            EnemyEvents.OnEnemyDefeated(info);
            if (IsTarget) EnemyEvents.OnTargetDefeated(info);
            else EnemyEvents.OnNormalDefeated(info);
            
            Destroy(gameObject, DeathAnimationKey.Length);
        }
    }
}