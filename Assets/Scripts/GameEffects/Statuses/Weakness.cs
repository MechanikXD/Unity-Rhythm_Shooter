using Interactable.Damageable;
using Interactable.Status;
using UnityEngine;

namespace GameEffects.Statuses {
    [CreateAssetMenu(fileName = "Weakness", menuName = "Scriptable Objects/Statuses/Weakness")]
    public class Weakness : StatusEffect {
        [SerializeField] private int _flatDamageDecrease;
        [SerializeField] private float _percentDamageDecrease;

        public override void OnStatusApply(DamageableBehaviour attachedTo) {
            attachedTo.SetDamageIncrement(attachedTo.DamageIncrement - _flatDamageDecrease);
            attachedTo.SetDamageMultiplier(attachedTo.DamageMultiplier - _percentDamageDecrease);
        }

        public override void EachBeatAction(DamageableBehaviour _) { }

        public override void OnRepeatedApply(DamageableBehaviour _) { }

        public override void OnStatusRemoved(DamageableBehaviour attachedTo) {
            attachedTo.SetDamageIncrement(attachedTo.DamageIncrement + _flatDamageDecrease);
            attachedTo.SetDamageMultiplier(attachedTo.DamageMultiplier + _percentDamageDecrease);
        }
    }
}