using System.Collections.Generic;
using Core.Behaviour.FiniteStateMachine;
using Core.Game.States;
using Core.Level.Room;
using Core.Offerings;
using Interactable.Damageable;
using Player;
using Unity.AI.Navigation;
using UnityEngine;

namespace Core.Game {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance;
        private StateMachine _stateMachine;
        private GameStates _states;
        [SerializeField] private PlayerController _playerReference;
        private RoomInfo _activeRoom;
        [SerializeField] private OfferingBase[] _offerings;

        public PlayerController Player => _playerReference;
        public DamageableBehaviour[] ActiveEnemies => _activeRoom.ActiveEnemies;
        public OfferingBase[] AllOfferings => _offerings;


        private Dictionary<(int unique, int global), RoomInfo> _levelRooms;

        private void Awake() {
            ToSingleton();
            OfferingManager.Initialize();
            OfferingManager.CreateNewHand();
            _stateMachine = new StateMachine();
            _states = new GameStates(_stateMachine);
            // TODO: Replace with last remembered state
            _stateMachine.Initialize(_states.RoamingState);
        }

        private void ToSingleton() {
            if (Instance != null) {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
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