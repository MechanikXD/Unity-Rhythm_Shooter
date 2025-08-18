namespace Interactable.Damageable {
    public interface IDamageable {
        public void TakeDamage(DamageInfo damageInfo);

        public void Die();
    }
}