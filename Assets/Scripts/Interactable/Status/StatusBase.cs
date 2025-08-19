using Core.Music;
using Interactable.Damageable;
using UnityEngine;

namespace Interactable.Status {
    public abstract class StatusBase : MonoBehaviour {
        [SerializeField] protected int _durationInBeats;
        protected DamageableBehaviour Attached;
        protected int CurrentDuration;

        public virtual void ApplyStatus(DamageableBehaviour damageable) {
            Attached = damageable;
            Conductor.Instance.AddRepeatingAction(GetInstanceID().ToString(), OnEachBeat);
            CurrentDuration = _durationInBeats;
            OnStatusApply();
        }
        
        protected abstract void OnStatusApply();
        public abstract void RepeatedApply();
        protected virtual void EachBeatAction() {}

        protected virtual void OnEachBeat() {
            EachBeatAction();
            CurrentDuration--;
            if (CurrentDuration <= 0) {
                Attached.RemoveStatus(this);
            }
        }
        protected abstract void OnStatusRemoved();

        public virtual void RemoveStatus() {
            OnStatusRemoved();
            Conductor.Instance.RemoveRepeatingAction(GetInstanceID().ToString());
            Attached = null;
            Destroy(this);
        }
    }
}