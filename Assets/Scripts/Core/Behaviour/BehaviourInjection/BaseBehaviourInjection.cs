using System;
using System.Collections.Generic;

namespace Core.Behaviour.BehaviourInjection {
    public class BaseBehaviourInjection<TAccept, TReturn> {
        private Func<TAccept, TReturn> _currentBehavior;
        private readonly Func<TAccept, TReturn> _defaultBehavior;

        private readonly Dictionary<string, Func<TAccept, TReturn>> _behaviours;

        public BaseBehaviourInjection(Func<TAccept, TReturn> defaultBehavior) {
            _behaviours = new Dictionary<string, Func<TAccept, TReturn>>
                { { "Default", defaultBehavior } };
            _currentBehavior = defaultBehavior;
            _defaultBehavior = defaultBehavior;
        }
        
        public BaseBehaviourInjection(Func<TAccept, TReturn> defaultBehavior, 
                                      Func<TAccept, TReturn> activeBehaviour) {
            _behaviours = new Dictionary<string, Func<TAccept, TReturn>>
                { { "Default", defaultBehavior } };
            _currentBehavior = activeBehaviour;
            _defaultBehavior = defaultBehavior;
        }

        public void ChangeToDefaultBehaviour() => 
            _currentBehavior = _defaultBehavior;

        public TReturn Perform(TAccept value) => _currentBehavior(value);

        public void ChangeBehaviour(Func<TAccept, TReturn> newBehaviour) => 
            _currentBehavior = newBehaviour;

        public void AddBehaviour(string key, Func<TAccept, TReturn> behaviour) =>
            _behaviours.Add(key, behaviour);
        
        public void ChangeBehaviour(string key) => ChangeBehaviour(_behaviours[key]);
    }

    public class BaseBehaviourInjection<T> {
        private Action<T> _currentBehavior;
        private readonly Action<T> _defaultBehavior;
        
        private readonly Dictionary<string, Action<T>> _behaviours;
        
        public BaseBehaviourInjection(Action<T> defaultBehavior) {
            _behaviours = new Dictionary<string, Action<T>> { { "Default", defaultBehavior } };
            _currentBehavior = defaultBehavior;
            _defaultBehavior = defaultBehavior;
        }
        
        public BaseBehaviourInjection(Action<T> defaultBehavior, Action<T> activeBehaviour) {
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
}