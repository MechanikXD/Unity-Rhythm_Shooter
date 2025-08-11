using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.ScriptableObjects.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Managers {
    public class BeatQueue : IEnumerable<(Beat left, Beat right)> {
        private Queue<(Beat left, Beat right)> _beats;
        private readonly BeatSettings _defaultSettings;
        public int Length { get; }

        public BeatQueue(Image prefab, int count, RectTransform leftArea, RectTransform rightArea, BeatSettings defaultState) {
            Length = count;
            _beats = new Queue<(Beat left, Beat right)>(Length);
            _defaultSettings = defaultState;
            
            for (var i = 0; i < Length; i++) {
                var left = new Beat();
                left.Initialize(prefab, leftArea, false);
                left.SetDefaultState(defaultState);

                var right = new Beat();
                right.Initialize(prefab, rightArea, true);
                right.SetDefaultState(defaultState);
                _beats.Enqueue((left, right));
            }
        }
        
        // Starts new instances of beats
        public void StartNewBeats(float time, float offset) {
            var newActiveBeats = _beats.Dequeue();
            newActiveBeats.left.Animate(_defaultSettings, time + offset);
            newActiveBeats.right.Animate(_defaultSettings, time + offset);
            _beats.Enqueue(newActiveBeats);
        }

        public (Beat left, Beat right) PeekFront() {
            var list = _beats.ToList();
            return Length > 2 ? list[1] : list[0];
        }
        
        public (Beat left, Beat right) PeekBack() => _beats.Peek();

        public IEnumerator<(Beat left, Beat right)> GetEnumerator() {
            var list = _beats.ToList();
            var first = list[0];

            for (var i = 1; i < list.Count; i++) {
                yield return list[i];
            }

            yield return first;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}