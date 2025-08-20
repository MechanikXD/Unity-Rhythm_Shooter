using Core.Offerings.Components;
using Interactable.Damageable;
using UnityEngine;

namespace GameEffects.Offering_Effects {
    [CreateAssetMenu(fileName = "DecreaseDamageForHpLoss", menuName = "Scriptable Objects/Offering Effect/DRed On HP Loss")]
    public class DecreaseDamageForHpLoss : OfferingEffect {
        [SerializeField] private float _maxDamageReduction;
        private float _currentReductionProvided; 
        
        public override void SubscribeEffect() {
            // PlayerEvents.DamageTaken += Effect;
        }

        protected override void Effect(DamageableBehaviour damageable) {
            var hpLostRelative = 1 - damageable.CurrentHealth / damageable.MaxHealth;
            var newDamageReduction = _maxDamageReduction * hpLostRelative;
            damageable.SetDamageReduction(damageable.CurrentDamageReduction -
                _currentReductionProvided + newDamageReduction);
            _currentReductionProvided = newDamageReduction;
        }

        public override void UnsubscribeEffect() {
            // PlayerEvents.DamageTaken -= Effect;
        }
    }
}