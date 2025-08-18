using System.Collections.Generic;
using Core.Behaviour.FiniteStateMachine;
using Core.Game.States;
using Core.Level.Room;
using Enemy.Base;
using Interactable;
using Interactable.Damageable;
using JetBrains.Annotations;
using Player;
using UI.Managers;
using UnityEngine;

namespace Core.Game {
    public class GameManager : MonoBehaviour {
        private static GameManager _instance;
        private StateMachine _stateMachine;
        private GameStates _states;
        private PlayerController _playerReference;
        private CrosshairBeat _playerCrosshair;
        private RoomInfo _activeRoom;

        public PlayerController Player => _playerReference;
        public DamageableBehaviour[] ActiveEnemies => _activeRoom.ActiveEnemies;

        [CanBeNull] public CrosshairBeat PlayerCrosshair => _playerCrosshair;

        private Dictionary<(int unique, int global), RoomInfo> _levelRooms;
        
        public static GameManager Instance {
            get {
                if (_instance != null) return _instance;

                var gameManagerObject = new GameObject(nameof(GameManager));
                _instance = gameManagerObject.AddComponent<GameManager>();
                DontDestroyOnLoad(gameManagerObject);

                return _instance;
            }
        }

        private void Awake() {
            _stateMachine = new StateMachine();
            _states = new GameStates(_stateMachine);
            // TODO: Replace with last remembered state
            _stateMachine.Initialize(_states.RoamingState);

            _playerReference = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            _playerCrosshair = _playerReference.GetComponentInChildren<CrosshairBeat>();
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