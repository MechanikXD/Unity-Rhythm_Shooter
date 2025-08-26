using Interactable.Damageable;
using UnityEngine;

namespace Interactable.Status {
    public abstract class StatusEffect : ScriptableObject {
        [SerializeField] protected int _durationInBeats;

        public int Duration => _durationInBeats;
        
        public abstract void OnStatusApply(DamageableBehaviour attachedTo);
        public abstract void EachBeatAction(DamageableBehaviour attachedTo);
        public abstract void OnRepeatedApply(DamageableBehaviour attachedTo);
        public abstract void OnStatusRemoved(DamageableBehaviour attachedTo);
    }
}