using Interactable.Status;

namespace Interactable.Damageable {
    public interface IDamageable {
        public void TakeDamage(DamageInfo damageInfo);

        public void ApplyStatus(StatusEffect status);

        public void Die();
    }
}