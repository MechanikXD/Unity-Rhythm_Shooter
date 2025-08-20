using Core.Music;
using Interactable.Damageable;
using UnityEngine;

namespace Interactable.Status {
    public abstract class StatusBase : MonoBehaviour {
        [SerializeField] protected int _durationInBeats;
        protected DamageableBehaviour Attached;
        protected int CurrentDuration;

        public virtual void ApplyStatus(DamageableBehaviour damageable, Transform parent) {
            if (Attached == null) {
                parent.gameObject.AddComponent(this.GetType());  
                Attached = damageable;
                Conductor.Instance.AddRepeatingAction(GetKey(), OnEachBeat);
            }
            else {
                this.enabled = true;
            }
            
            CurrentDuration = _durationInBeats;
            OnStatusApply();
        }
        
        protected abstract void OnStatusApply();

        public virtual void RepeatedApply() {
            // Refresh duration
            CurrentDuration = _durationInBeats;
        }
        protected virtual void EachBeatAction() {}

        protected virtual void OnEachBeat() {
            if (CurrentDuration <= 0) return;
            
            EachBeatAction();
            // Decrease duration of this status
            CurrentDuration--;
            if (CurrentDuration <= 0) {
                Attached.RemoveStatus(this);
            }
        }
        protected abstract void OnStatusRemoved();

        public virtual void RemoveStatus() {
            OnStatusRemoved();
            this.enabled = false;
        }

        private string GetKey() {
            return $"{Attached.GetInstanceID()}{gameObject.GetInstanceID()}";
        }

        private void OnDisable() {
            Conductor.Instance.RemoveRepeatingAction(GetKey());
        }
    }
}