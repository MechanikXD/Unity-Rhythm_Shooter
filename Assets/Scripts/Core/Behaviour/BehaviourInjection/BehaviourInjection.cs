using System;
using System.Collections.Generic;

namespace Core.Behaviour.BehaviourInjection {
    public class BehaviourInjection<T> {
        private Action<T> _currentBehavior;
        private readonly Action<T> _defaultBehavior;
        
        private readonly Dictionary<string, Action<T>> _behaviours;
        
        public BehaviourInjection(Action<T> defaultBehavior) {
            _behaviours = new Dictionary<string, Action<T>> { { "Default", defaultBehavior } };
            _currentBehavior = defaultBehavior;
            _defaultBehavior = defaultBehavior;
        }
        
        public BehaviourInjection(Action<T> defaultBehavior, Action<T> activeBehaviour) {
            _behaviours = new Dictionary<string, Action<T>> { { "Default", defaultBehavior } };
            _currentBehavior = activeBehaviour;
            _defaultBehavior = defaultBehavior;
        }

        public void ChangeToDefaultBehaviour() => 
            _currentBehavior = _defaultBehavior;

        public void Perform(T value) => _currentBehavior(value);

        public void ChangeBehaviour(Action<T> newBehaviour) => 
            _currentBehavior = newBehaviour;
        
        public void AddBehaviour(string key, Action<T> behaviour) => _behaviours.Add(key, behaviour);
        
        public void ChangeBehaviour(string key) => ChangeBehaviour(_behaviours[key]);
    }

    public class BehaviourInjection {
        private Action _currentBehavior;
        private readonly Action _defaultBehavior;
        
        private readonly Dictionary<string, Action> _behaviours;
        
        public BehaviourInjection(Action defaultBehavior) {
            _behaviours = new Dictionary<string, Action> { { "Default", defaultBehavior } };
            _currentBehavior = defaultBehavior;
            _defaultBehavior = defaultBehavior;
        }
        
        public BehaviourInjection(Action defaultBehavior, Action activeBehaviour) {
            _behaviours = new Dictionary<string, Action> { { "Default", defaultBehavior } };
            _currentBehavior = activeBehaviour;
            _defaultBehavior = defaultBehavior;
        }

        public virtual void ChangeToDefaultBehaviour() => _currentBehavior = _defaultBehavior;

        public virtual void Perform() => _currentBehavior();

        public virtual void ChangeBehaviour(Action newBehaviour) => _currentBehavior = newBehaviour;
        
        public virtual void AddBehaviour(string key, Action behaviour) => _behaviours.Add(key, behaviour);
        
        public virtual void ChangeBehaviour(string key) => ChangeBehaviour(_behaviours[key]);
    }
}