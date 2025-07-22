using System.Collections.Generic;
using Core.Level.Room;
using UnityEngine;
using Random = System.Random;

namespace Core.Level.Generator {
    public class LevelGenerator : MonoBehaviour {
        private Transform _main;
        [SerializeField] private Transform _origin;
        [SerializeField] private LevelSettings _levelSettings;

        private List<Transform> _availableExits;
        private Random _seedGenerator;
        private Random _levelGenerator;
        private int _seed;

        private void Awake() {
            Init();
            BuildMainPath();
            BuildAdditionalRooms();
        }

        private void BuildMainPath() {
            PlaceStart();

            var mainPath = _levelSettings.GetMainPathRooms();
            var prevRoomCount = 1;
            foreach (var room in mainPath) {
                var exitIndex = _levelGenerator.Next(_availableExits.Count - prevRoomCount, 
                    _availableExits.Count);
                PlaceRandomRoom(room, _availableExits[exitIndex]);
                prevRoomCount = room.ExitCount - 1;
            }
            var finalExitIndex = _levelGenerator.Next(_availableExits.Count - prevRoomCount, 
                _availableExits.Count);
            PlaceRandomRoom(_levelSettings.FinalRoom, _availableExits[finalExitIndex]);
        }

        private void BuildAdditionalRooms() {
            var roomQueue = _levelSettings.GetAdditionalRoomQueue(_availableExits.Count);

            while (roomQueue.Count > 0) {
                var roomToPlace = roomQueue.Dequeue();
                PlaceRandomRoom(roomToPlace);
            }
            
            // TODO: Block rest of the exits 
        }

        private void Init() {
            _seedGenerator = new Random();
            _seed = _seedGenerator.Next();
            _levelGenerator = new Random(_seed);
            _availableExits = new List<Transform>();
        }

        private void PlaceStart() {
            var level = new GameObject("Level") {
                transform = {
                    position = _origin.position
                }
            };

            _main = level.transform;
            Debug.Log($"_levelSettings.StartRoom.ExitCount: {_levelSettings.StartRoom.ExitCount}");
            var start = _levelSettings.StartRoom.InstantiateOnZero(_main);
            Debug.Log($"_levelSettings.StartRoom.ExitCount: {_levelSettings.StartRoom.ExitCount}");
            Debug.Log($"start.ExitCount: {start.ExitCount}");
            _availableExits.AddRange(start.ExitPositions);
        }

        private bool PlaceRandomRoom(RoomInfo roomToPlace, Transform entrancePosition) {
            var roomExits = roomToPlace.ExitPositions;
            
            foreach (var exit in roomExits) {
                if (roomToPlace.TryInstantiateSelf(entrancePosition, exit, _main, out var newRoom)) {
                    _availableExits.Remove(entrancePosition);
                    _availableExits.AddRange(newRoom.ExitPositions);
                    return true;
                }
            }

            return false;
        }

        private bool PlaceRandomRoom(RoomInfo exitPosition) {
            var randomExit = _availableExits[_levelGenerator.Next(0, _availableExits.Count)];
            return PlaceRandomRoom(exitPosition, randomExit);
        }

    }
}