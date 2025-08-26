using System;
using System.Collections.Generic;
using Core.Music.Sequence.Components;

namespace Core.Music.Sequence {
    public class ActionSequenceBuilder {
        private readonly List<SequenceActor> _actions = new List<SequenceActor>();

        public int Count => _actions.Count;

        public void Append(Trigger trigger, IEnumerable<Func<bool>> predicate, Action<ActionSequence> action) => 
            _actions.Add(new SequenceActor(trigger, predicate, action));

        public void Append(Trigger trigger, Func<bool> predicate, Action<ActionSequence> action) => 
            _actions.Add(new SequenceActor(trigger, predicate, action));

        public void Append(Trigger trigger, Action<ActionSequence> action) => 
            _actions.Add(new SequenceActor(trigger, action));
        
        public void Insert(int index, Trigger trigger, IEnumerable<Func<bool>> predicate, Action<ActionSequence> action) => 
            _actions.Insert(index, new SequenceActor(trigger, predicate, action));

        public void Insert(int index, Trigger trigger, Func<bool> predicate, Action<ActionSequence> action) => 
            _actions.Insert(index, new SequenceActor(trigger, predicate, action));

        public void Insert(int index, Trigger trigger, Action<ActionSequence> action) => 
            _actions.Insert(index, new SequenceActor(trigger, action));

        public void Remove(int index) => _actions.RemoveAt(index);

        public void Clear() => _actions.Clear();

        public IList<SequenceActor> GetSequence() => _actions;

        public ActionSequence ToSequence() => new ActionSequence(_actions);
    }
}