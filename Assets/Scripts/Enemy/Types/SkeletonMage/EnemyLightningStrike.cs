using Enemy.Base;
using Interactable.AttackCollider;
using UnityEngine;

namespace Enemy.Types.SkeletonMage {
    public class EnemyLightningStrike : MonoBehaviour {
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private EnemyAttackCollider _attackCollider;

        private void Awake() {
            _attackCollider.DeactivateCollider();
        }

        public void Launch(EnemyBase owner) {
            _attackCollider.SetOwner(owner);
            var newParticle = Instantiate(_particle, transform.position, Quaternion.identity);
            newParticle.Play();
            _attackCollider.ActivateCollider();
            
            Destroy(gameObject, _particle.main.duration);
        }

        private void OnDestroy() => _attackCollider.DeactivateCollider();
    }
}