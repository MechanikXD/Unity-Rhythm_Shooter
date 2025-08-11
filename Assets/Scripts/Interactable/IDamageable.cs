namespace Interactable {
    public interface IDamageable {
        public int CurrentHealth { get; protected set; }
        
        public void TakeDamage(DamageInfo damageInfo);

        public void Parried(DamageInfo damageInfo);

        public void TakeDirectDamage(int value) => CurrentHealth -= value;

        public void Die();
    }
}