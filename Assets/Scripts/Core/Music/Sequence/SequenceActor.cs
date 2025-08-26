using System;
using System.Collections.Generic;
using System.Linq;
using Core.Music.Sequence.Components;

namespace Core.Music.Sequence {
    public class SequenceActor {
        private readonly List<Func<bool>> _conditions;
        private readonly Action<ActionSequence> _action;
        public Trigger Trigger { get; }

        public void DoAction(ActionSequence sequence) {
            if (_conditions == null || _conditions.All(condition => condition()))
                    _action(sequence);
        }

        public SequenceActor(Trigger trigger, IEnumerable<Func<bool>> condition, Action<ActionSequence> action) {
            Trigger = trigger;
            _conditions = condition.ToList();
            _action = action;
        }
        
        public SequenceActor(Trigger trigger, Func<bool> condition, Action<ActionSequence> action) {
            Trigger = trigger;
            _conditions = new List<Func<bool>> { condition };
            _action = action;
        }
        
        public SequenceActor(Trigger trigger, Action<ActionSequence> action) {
            Trigger = trigger;
            _conditions = null;
            _action = action;
        }
    }
}