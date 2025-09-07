using System.Collections;
using Enemy.Base;
using UnityEngine;

namespace Enemy.Types.SkeletonTank.States {
    public class Shielding : EnemyState<SkeletonTank> {
        public Shielding(SkeletonTank enemy) : base(enemy) { }
        
        public override void EnterState() {
            Enemy.Rotation.SetObservationPoint(Enemy.PlayerTransform);
            Enemy.StartCoroutine(AnimationQueue());
        }

        /// <summary>
        /// Plays enter/exit animations as well as stays in loop animation in between.
        /// Performs everything for state itself... 
        /// </summary>>
        private IEnumerator AnimationQueue() {
            Enemy.PlayAnimation(Enemy.BlockStart);
            yield return new WaitForSeconds(Enemy.BlockStart.length);

            var loopTime = Enemy.ShieldingTime - Enemy.BlockStart.length * 2;
            if (loopTime > 0) {
                Enemy.PlayAnimation(Enemy.BlockLoopKey);
                yield return new WaitForSeconds(loopTime);
            }
            
            Enemy.PlayAnimation(Enemy.BlockExitKey);
            yield return new WaitForSeconds(Enemy.BlockStart.length);
            
            if (Enemy.IsNearPlayer(EnemyBase.PlayerProximity + 2f)) {
                ChangeState<Attack>();
            }
            else {
                ChangeState<ChasePlayer>();
            }
        }
    }
}