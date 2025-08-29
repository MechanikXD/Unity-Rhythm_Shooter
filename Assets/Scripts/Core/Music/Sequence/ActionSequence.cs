using System;
using System.Collections.Generic;
using System.Linq;
using Core.Music.Sequence.Components;
using UnityEngine;

namespace Core.Music.Sequence {
    [Serializable]
    public class ActionSequence {
        private List<SequenceActor> _actions;
        private int _currentActorIndex;
        public bool IsFinished { get; private set; }
        public bool IsPaused { get; private set; }

        private Dictionary<Trigger, Action> _invoker;

        public ActionSequence(IList<SequenceActor> sequence) => Initialize(sequence);

        public ActionSequence(ActionSequenceBuilder sequenceBuilder) => Initialize(sequenceBuilder);

        public void Initialize(IList<SequenceActor> sequence) {
            if (sequence.Count < 2) {
                Debug.Log("Given sequence has only one action or none, thus sequence will not be created.");
                return;
            }
            _actions = sequence.ToList();
            _currentActorIndex = 0;
            IsFinished = false;
            
            _invoker = new Dictionary<Trigger, Action> {
                [Trigger.NextBeat] = () => Conductor.Instance.AddOnNextBeat(ActAndAdvance),
                [Trigger.NextHalfBeat] = () => Conductor.Instance.AddOnNextHalfBeat(ActAndAdvance),
                [Trigger.BeforeBeat] = () => Conductor.Instance.AddBeforeNextBeat(ActAndAdvance),
                [Trigger.AfterBeat] = () => Conductor.Instance.AddAfterNextBeat(ActAndAdvance)
            };
        }

        public void Initialize(ActionSequenceBuilder sequenceBuilder) =>
            Initialize(sequenceBuilder.GetSequence());

        private void ActAndAdvance() {
            if (IsFinished || IsPaused) return;
            
            _actions[_currentActorIndex].DoAction(this);
            _currentActorIndex++;
            // In case sequence was broken or halted during action
            if (IsFinished || IsPaused) return;
            // Check sequence reached it's end
            if (_currentActorIndex >= _actions.Count) {
                IsFinished = true;
                Debug.Log("Finishing sequence");
            }
            // If not - queue next actor
            else {
                Debug.Log("Starting next action");
                _invoker[_actions[_currentActorIndex].Trigger]();
            }
        }
        
        /// <summary>
        /// Starts sequence within this instance. Sequence must be provided via Initialize() method,
        /// Otherwise NullReference may occur.
        /// </summary>
        public void Start() {
            Debug.Log("Sequence was Started");
            if (_currentActorIndex == 0) _invoker[_actions[_currentActorIndex].Trigger]();
            else Debug.Log("Sequence was started prior. Maybe you meant to Resume() it?");
        }
        /// <summary>
        /// Completely stops sequence rendering it as finished.
        /// To temporarily stop the sequence use Pause() method
        /// </summary>
        public void Break() => IsFinished = true;
        /// <summary>
        /// Pauses this sequence. To continue, sequence must be manually resumed.
        /// </summary>
        public void Pause() => IsPaused = true;
        /// <summary>
        /// Resumes sequence from paused state. Won't work if sequence if finished.
        /// </summary>
        public void Resume() {
            IsPaused = false;
            if (!IsFinished) _invoker[_actions[_currentActorIndex].Trigger]();
        }
        /// <summary>
        /// Will restart sequence making it NOT finished and NOT paused unless was set as paused.
        /// Note: none of parameters within conditions or action will NOT be reset.
        /// </summary>
        /// <param name="setOnPause"> Use to set sequence on pause after rewinding </param>
        public void Restart(bool setOnPause=false) {
            Debug.Log("Sequence was restarted");
            IsFinished = false;
            _currentActorIndex = 0;
            
            if (setOnPause) IsPaused = true;
            else {
                IsPaused = false;
                _invoker[_actions[_currentActorIndex].Trigger]();
            }
        }
    }
}