using System.Diagnostics.CodeAnalysis;

namespace Core.Behaviour.FiniteStateMachine {
    /// <summary>
    /// Basic implementation of state machine
    /// </summary>
    public class StateMachine {
        private State _currentState;
        
        /// <summary>
        /// State in which this state machine is in right now. Readonly.
        /// </summary>
        public State CurrentState => _currentState;
        
        /// <summary>
        /// This instance of state machine should be bound to monoBehavior script to work properly
        /// But can work by itself just fine.
        /// </summary>
        /// <param name="startingState"> State from which stateMachine will start working </param>
        public void Initialize([NotNull] State startingState) {
            _currentState = startingState;
            _currentState.EnterState();
        }
        
        // ReSharper disable Unity.PerformanceAnalysis <- We don't actually call this function every frame
        /// <summary>
        /// Changes state of this state machine.
        /// </summary>
        /// <param name="newState"> new state in state machine </param>
        public void ChangeState([NotNull] State newState) {
            _currentState.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }
    }
}