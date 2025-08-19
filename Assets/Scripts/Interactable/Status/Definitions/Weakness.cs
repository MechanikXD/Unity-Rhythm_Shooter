using UnityEngine;

namespace Interactable.Status.Definitions {
    public class Weakness : StatusBase {
        [SerializeField] private int _flatDamageDecrease;
        [SerializeField] private int _percentDamageDecrease;

        protected override void OnStatusApply() {
            Attached.ChangeDamageIncrement(Attached.DamageIncrement - _flatDamageDecrease);
            Attached.ChangeDamageMultiplier(Attached.DamageMultiplier - _percentDamageDecrease);
        }

        public override void RepeatedApply() {
            CurrentDuration = _durationInBeats;
        }

        protected override void OnStatusRemoved() {
            Attached.ChangeDamageIncrement(Attached.DamageIncrement + _flatDamageDecrease);
            Attached.ChangeDamageMultiplier(Attached.DamageMultiplier + _percentDamageDecrease);
        }
    }
}