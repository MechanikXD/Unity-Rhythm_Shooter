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
            var damageInfo = new DamageInfo(null, new IDamageable[] { attachedTo },
                _damagePerStack * _currentStack, Vector3.zero, attachedTo.Position);
            attachedTo.TakeDamage(damageInfo);
        }

        public override void OnRepeatedApply(DamageableBehaviour _) => _currentStack++;

        public override void OnStatusRemoved(DamageableBehaviour _) => _currentStack = 0;
    }
}