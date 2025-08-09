using System;

namespace Core.Behaviour.BehaviourInjection {
    public class BaseBehaviourInjection<TAccept, TReturn> {
        private Func<TAccept, TReturn> _currentBehavior;
        private readonly Func<TAccept, TReturn> _defaultBehavior;

        public BaseBehaviourInjection(Func<TAccept, TReturn> defaultBehavior) {
            _currentBehavior = defaultBehavior;
            _defaultBehavior = defaultBehavior;
        }
        
        public BaseBehaviourInjection(Func<TAccept, TReturn> defaultBehavior, 
                                      Func<TAccept, TReturn> activeBehaviour) {
            _currentBehavior = activeBehaviour;
            _defaultBehavior = defaultBehavior;
        }

        public void ChangeToDefaultBehaviour() => 
            _currentBehavior = _defaultBehavior;

        public TReturn Perform(TAccept value) => _currentBehavior(value);

        public void ChangeBehaviour(Func<TAccept, TReturn> newBehaviour) => 
            _currentBehavior = newBehaviour;
    }

    public class BaseBehaviourInjection<T> {
        private Action<T> _currentBehavior;
        private readonly Action<T> _defaultBehavior;
        
        public BaseBehaviourInjection(Action<T> defaultBehavior) {
            _currentBehavior = defaultBehavior;
            _defaultBehavior = defaultBehavior;
        }
        
        public BaseBehaviourInjection(Action<T> defaultBehavior, 
                                      Action<T> activeBehaviour) {
            _currentBehavior = activeBehaviour;
            _defaultBehavior = defaultBehavior;
        }

        public void ChangeToDefaultBehaviour() => 
            _currentBehavior = _defaultBehavior;

        public void Perform(T value) => _currentBehavior(value);

        public void ChangeBehaviour(Action<T> newBehaviour) => 
            _currentBehavior = newBehaviour;
    }
}