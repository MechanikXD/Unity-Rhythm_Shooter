using System.Collections;
using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonTank.States {
    public class ShieldIdle : EnemyState {
        private readonly float _idleTime;

        private readonly AnimationClip _start;
        private readonly string _loopKey;
        private readonly string _exitKey;
        
        public ShieldIdle(EnemyBase enemy, float idleTime, 
            AnimationClip start, string loopKey, string exit) : base(enemy) {
            _idleTime = idleTime;
            _start = start;
            _loopKey = loopKey;
            _exitKey = exit;
        }
        
        public override void EnterState() {
            Enemy.Rotation.SetObservationPoint(Enemy.PlayerTransform);
            Enemy.StartCoroutine(AnimationQueue());
        }

        private IEnumerator AnimationQueue() {
            Enemy.PlayAnimation(_start.name);
            yield return new WaitForSeconds(_start.length);

            var loopTime = _idleTime - _start.length * 2;
            if (loopTime > 0) {
                Enemy.PlayAnimation(_loopKey);
                yield return new WaitForSeconds(loopTime);
            }
            
            Enemy.PlayAnimation(_exitKey);
            yield return new WaitForSeconds(_start.length);
            
            if (Enemy.IsNearPlayer(EnemyBase.PlayerProximity + 2f)) {
                ChangeState<AttackState>();
            }
            else {
                ChangeState<WalkTowardPlayer>();
            }
        }
    }
}