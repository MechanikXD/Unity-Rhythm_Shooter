using Interactable.Damageable;
using Interactable.Status;
using UnityEngine;

namespace GameEffects.Statuses {
    [CreateAssetMenu(fileName = "Gaze", menuName = "Scriptable Objects/Statuses/Gaze")]
    public class Gaze : StatusEffect {
        [SerializeField] private int _damagePerStack;
        private int _currentStack;
        
        public override void OnStatusApply(DamageableBehaviour _) => _currentStack = 1;

        public override void EachBeatAction(DamageableBehaviour attachedTo) {
            var info = DamageInfoBuilder.SourceLess(attachedTo, _damagePerStack * _currentStack);
            attachedTo.TakeDamage(info);
        }

        public override void OnRepeatedApply(DamageableBehaviour _) => _currentStack++;

        public override void OnStatusRemoved(DamageableBehaviour _) => _currentStack = 0;
    }
}