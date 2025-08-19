using Interactable.Damageable;
using UnityEngine;

namespace Interactable.Status.Definitions {
    public class Gaze : StatusBase {
        [SerializeField] private int _damagePerStack;
        private int _currentStack;
        
        protected override void OnStatusApply() {
            _currentStack = 1;
        }

        protected override void EachBeatAction() {
            var damageInfo = new DamageInfo(null, new IDamageable[] { Attached },
                (_damagePerStack * _currentStack), Vector3.zero, Attached.Position);
            Attached.TakeDamage(damageInfo);
        }

        public override void RepeatedApply() {
            CurrentDuration = _durationInBeats;
            _currentStack++;
        }

        protected override void OnStatusRemoved() {
            _currentStack = 0;
        }
    }
}