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

        private const int MaxRetryAttempts = 100;

        private void Awake() {
            Init();
            if (!TryBuildMainPath()) {
                var mainPathWasGenerated = false;
                while (!mainPathWasGenerated) {
                    mainPathWasGenerated = TryBuildMainPath();
                }
            }
            TryBuildAdditionalRooms();
            // TODO: Block rest of unused exits
        }

        private bool TryBuildMainPath() {
            PlaceStart();

            var mainPath = _levelSettings.GetMainPathRooms();
            var prevRoomExitCount = 1;
            
            foreach (var room in mainPath) {
                if (!TryBuildRoomAtExits(room, _availableExits.Count - prevRoomExitCount, 
                        _availableExits.Count)) {
                    return false;
                }
                prevRoomExitCount = room.ExitCount - 1;
            }
            
            return TryBuildRoomAtExits(_levelSettings.FinalRoom, 
                _availableExits.Count - prevRoomExitCount, _availableExits.Count);
        }

        private bool TryBuildRoomAtExits(RoomInfo room, int minIndex, int maxIndex) {
            var exitIndex = _levelGenerator.Next(minIndex, maxIndex);

            if (TryPlaceRoom(room, _availableExits[exitIndex])) return true;

            // Couldn't place room the first time
            var triedIndexed = new List<int> { exitIndex };
            var availableExits = maxIndex - minIndex;
            var roomWasPlaced = false;
            // Try other indexes
            while (triedIndexed.Count < availableExits) {
                exitIndex = _levelGenerator.Next(minIndex, maxIndex);
                if (triedIndexed.Contains(exitIndex)) continue;

                roomWasPlaced = TryPlaceRoom(room, _availableExits[exitIndex]);
                if (roomWasPlaced) break;
            }

            return roomWasPlaced;
        }

        private bool TryBuildAdditionalRooms() {
            var roomQueue = _levelSettings.GetAdditionalRoomQueue(_availableExits.Count);

            while (roomQueue.Count > 0) {
                var roomToPlace = roomQueue.Dequeue();
                if (TryPlaceRoom(roomToPlace)) continue;

                // Couldn't place this specific room
                var roomChangeAttempts = 1;
                var roomWasPlaced = false;
                // try other rooms
                while (roomChangeAttempts < MaxRetryAttempts) {
                    roomToPlace = _levelSettings.GetRandomRoom();
                    roomWasPlaced = TryPlaceRoom(roomToPlace);
                        
                    if (roomWasPlaced) break;
                    roomChangeAttempts++;
                }

                if (roomWasPlaced) continue;
                // Can't place any room in current maze
                Debug.LogWarning("Couldn't place all additional rooms");
                return false;
            }

            return true;
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
            var start = _levelSettings.StartRoom.InstantiateOnZero(_main);
            _availableExits.AddRange(start.ExitPositions);
        }

        private bool TryPlaceRoom(RoomInfo roomToPlace, Transform entrancePosition) {
            var roomExits = roomToPlace.ExitPositions;
            
            foreach (var exit in roomExits) {
                if (!roomToPlace.TryInstantiateSelf(entrancePosition, exit, _main,
                        out var newRoom)) continue;
                _availableExits.Remove(entrancePosition);
                _availableExits.AddRange(newRoom.ExitPositions);
                return true;
            }

            return false;
        }

        private bool TryPlaceRoom(RoomInfo room) {
            return TryBuildRoomAtExits(room, 0, _availableExits.Count);
        }
    }
}