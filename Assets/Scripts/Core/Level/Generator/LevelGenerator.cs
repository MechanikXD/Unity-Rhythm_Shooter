using System.Collections.Generic;
using Core.Level.Room;
using UnityEngine;
using Random = System.Random;

namespace Core.Level.Generator {
    public class LevelGenerator : MonoBehaviour {
        private Transform _main;
        [SerializeField] private Transform origin;
        [Space]
        [SerializeField] private RoomInfo startRoom;
        [SerializeField] private RoomInfo[] rooms;
        [SerializeField] private RoomInfo endRoom;

        private List<Transform> _availableExits;
        private Random _seedGenerator;
        private Random _levelGenerator;
        private int _seed;

        private void Awake() {
            Init();
            PlaceStart();
            PlaceRandomRoom(rooms[1], _availableExits[0]);
            PlaceRandomRoom(rooms[0], _availableExits[1]);
            PlaceRandomRoom(rooms[0], _availableExits[1]);
            PlaceRandomRoom(rooms[0], _availableExits[1]);
        }

        public void Init() {
            _seedGenerator = new Random();
            _seed = _seedGenerator.Next();
            _levelGenerator = new Random(_seed);
            _availableExits = new List<Transform>();
        }

        public void PlaceStart() {
            var level = new GameObject("Level") {
                transform = {
                    position = origin.position
                }
            };

            _main = level.transform;
            var start = startRoom.InstantiateOnZero(_main);
            _availableExits.AddRange(start.ExitPositions);
        }

        public void PlaceRandomRoom(RoomInfo roomToPlace, Transform entrancePosition) {
            var roomExits = roomToPlace.ExitPositions;
            
            foreach (var exit in roomExits) {
                //if (!roomToPlace.CanBePlacedAt(entrancePosition, exit, _main)) continue;
                var generatedRoom = roomToPlace.InstantiateSelf(entrancePosition, exit, _main);
                _availableExits.Remove(entrancePosition);
                _availableExits.AddRange(generatedRoom.ExitPositions);
                break;
            }
        }

        public void PlaceRandomRoom(Transform exitPosition) {
            var randomRoom = rooms[_levelGenerator.Next(0, rooms.Length)];
            PlaceRandomRoom(randomRoom, exitPosition);
        }

        public void PlaceRandomRoom() {
            var randomExit = _availableExits[_levelGenerator.Next(0, _availableExits.Count)];
            PlaceRandomRoom(randomExit);
        }
    }
}