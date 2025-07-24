using Core.Behaviour.FiniteStateMachine;

namespace Core.Game.States {
    public class GameStates {
        public State HubState { get; }
        public State TestFieldState { get; }
        public State BattleState { get; }
        public State BossBattleState { get; }
        public State RoamingState { get; }

        public GameStates(StateMachine stateMachine) {
            HubState = new Hub(stateMachine);
            TestFieldState = new TestField(stateMachine);
            BattleState = new Battle(stateMachine);
            BossBattleState = new BossBattle(stateMachine);
            RoamingState = new Roaming(stateMachine);
        }
    }
}