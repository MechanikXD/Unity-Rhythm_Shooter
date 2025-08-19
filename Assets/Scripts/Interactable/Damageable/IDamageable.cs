using Interactable.Status;

namespace Interactable.Damageable {
    public interface IDamageable {
        public void TakeDamage(DamageInfo damageInfo);

        public void ApplyStatus(StatusBase status);

        public void Die();
    }
}