using System.Collections.Generic;
using Core.Behaviour.FiniteStateMachine;
using Core.Behaviour.SingletonBehaviour;
using Core.Game.States;
using Core.Level.Room;
using Core.Offerings;
using Interactable.Damageable;
using Player;
using UnityEngine;

namespace Core.Game {
    public class GameManager : SingletonBase<GameManager> {
        private StateMachine _stateMachine;
        private GameStates _states;
        [SerializeField] private PlayerController _playerReference;
        private RoomInfo _activeRoom;
        [SerializeField] private OfferingBase[] _offerings;

        public PlayerController Player => _playerReference;
        public DamageableBehaviour[] ActiveEnemies => _activeRoom.ActiveEnemies;
        public OfferingBase[] AllOfferings => _offerings;


        private Dictionary<(int unique, int global), RoomInfo> _levelRooms;

        protected override void Awake() {
            base.Awake();
            OfferingManager.Initialize();
            OfferingManager.CreateNewHand();
            _stateMachine = new StateMachine();
            _states = new GameStates(_stateMachine);
            // TODO: Replace with last remembered state
            _stateMachine.Initialize(_states.RoamingState);
        }

        public void EnterBattleState(RoomInfo enteredRoom) {
            _activeRoom = enteredRoom;
            enteredRoom.StartCombat();
            _stateMachine.ChangeState(enteredRoom.IsBossBattle
                ? _states.BossBattleState
                : _states.BattleState);
        }
    }
}