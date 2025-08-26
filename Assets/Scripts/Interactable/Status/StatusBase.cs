using Core.Music;
using Interactable.Damageable;

namespace Interactable.Status {
    public class StatusBase {
        private readonly StatusEffect _status;
        private readonly DamageableBehaviour _attached;
        private bool _isActive;
        private int _currentDuration;

        public bool IsActive => _isActive;

        public StatusBase(DamageableBehaviour damageable, StatusEffect status) {
            _attached = damageable;
            _status = status;
        }

        public void ApplyStatus() {
            Conductor.NextBeatEvent += OnEachBeat;
            _currentDuration = _status.Duration;
            _status.OnStatusApply(_attached);
            _isActive = true;
        }

        public void RepeatedApply() {
            if (!_isActive) return;
            
            _status.OnRepeatedApply(_attached);
            _currentDuration = _status.Duration;
        }

        private void OnEachBeat() {
            if (!_isActive) return;
            
            _status.EachBeatAction(_attached);
            // Decrease duration of this status
            _currentDuration--;
            if (_currentDuration <= 0) {
                _attached.RemoveStatus(_status);
            }
        }

        public void RemoveStatus() {
            _status.OnStatusRemoved(_attached);
            Conductor.NextBeatEvent -= OnEachBeat;
            _isActive = false;
        }
    }
}