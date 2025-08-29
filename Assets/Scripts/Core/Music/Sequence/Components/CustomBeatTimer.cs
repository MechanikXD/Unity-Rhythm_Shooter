using System;

namespace Core.Music.Sequence.Components {
    public readonly struct CustomBeatTimer {
        private readonly Action _actionAfterTimeout;
        private readonly int _crotchets;
        private readonly int _halfCrochets;

        public CustomBeatTimer(Action action, int crotchets, int halfCrotchets) {
            _actionAfterTimeout = action;
            _crotchets = crotchets + halfCrotchets / 2;
            _halfCrochets = halfCrotchets % 2;
        }

        public void StartTimer() {
            var counter = _crotchets;
            bool extendByHalfBeat = _halfCrochets == 1;
            var action = _actionAfterTimeout;
            
            void Wrapper() {
                counter--;
                if (counter > 0) return;

                Conductor.NextBeat -= Wrapper;
                if (extendByHalfBeat) Conductor.Instance.AddOnNextHalfBeat(action);
                else action();
            }

            Conductor.NextBeat += Wrapper;
        }
    }
}