namespace Core.StateMachine.Base {
    /// <summary>
    /// The abstract class for state of StateMachine.
    /// </summary>
    public abstract class State {
        protected readonly StateMachine AttachedStateMachine;
        protected State(StateMachine stateMachine) => AttachedStateMachine = stateMachine;

        /// <summary>
        /// Called each time this state is entered to
        /// </summary>
        public virtual void EnterState() {}
        /// <summary>
        /// Called each time this state is exited from
        /// </summary>
        public virtual void ExitState() {}
        
        /// <summary>
        /// This functions should be called each frame to compute game logic
        /// </summary>
        public virtual void FrameUpdate() {}
        /// <summary>
        /// This functions should be called each frame to compute game physics
        /// </summary>
        public virtual void FixedUpdate() {}
    }
}