using System;
using System.Collections.Generic;
using Core.Level.Room;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Level.Generator {
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "Scriptable Objects/LevelSettings")]
    public class LevelSettings : ScriptableObject {
        [Header("Room pool")]
        [SerializeField] private RoomInfo _startRoom;
        [SerializeField] private RoomInfo[] _rooms;
        [SerializeField] private RoomInfo _finalRoom;
        [SerializeField] private GameObject _pathBlocker;

        [Header("Level Structure")]
        [SerializeField] private int _roomsBetweenStartAndEnd;
        [SerializeField] private int _additionalRooms;

        [SerializeField] private float[] _probabilityRange;

        public RoomInfo StartRoom => _startRoom;
        public RoomInfo FinalRoom => _finalRoom;
        public GameObject PathBlocker => _pathBlocker;
        
        public RoomInfo GetRandomRoom() {
            if (_probabilityRange.Length > 0 && _probabilityRange.Length == _rooms.Length) {
                return GetRoomFromRange(Random.value);
            }
            else {
                return GetTrueRandomRoom();
            }
        }

        private RoomInfo GetTrueRandomRoom() => _rooms[Random.Range(0, _rooms.Length)];

        private RoomInfo GetRoomFromRange(float valueInRange) {
            for (var i = 0; i < _rooms.Length; i++) {
                if (valueInRange <= _probabilityRange[i]) {
                    return _rooms[i];
                }
            }

            return GetTrueRandomRoom();
        }

        public RoomInfo[] GetMainPathRooms() {
            var path = new RoomInfo[_roomsBetweenStartAndEnd]; 
            for (var i = 0; i < _roomsBetweenStartAndEnd; i++) {
                for (var iterCount = 0; iterCount < 9999; iterCount++) {
                    var randomRoom = GetRandomRoom();
                    if (randomRoom.ExitCount > 1) {
                        path[i] = randomRoom;
                        break;
                    }
                    if (iterCount + 1 == 9999) {
                        throw new TimeoutException(
                            "Too many iterations taken to select room in main path. " +
                            "\nMake sure that at least one room has 2 exits " +
                            "and it's probability greater than 0");
                    }
                }
            }

            return path;
        }

        public Queue<RoomInfo> GetAdditionalRoomQueue(int existingExitsCount) {
            var rooms = new Queue<RoomInfo>(); 
            for (var i = 0; i < _additionalRooms; i++) {
                if (existingExitsCount > 1 || rooms.Count + 1 == _additionalRooms) {
                    rooms.Enqueue(GetRandomRoom());
                    existingExitsCount -= 1;
                }
                else {
                    for (var iterCount = 0; iterCount < 9999; iterCount++) {
                        var randomRoom = GetRandomRoom();
                        if (randomRoom.ExitCount > 1) {
                            rooms.Enqueue(randomRoom);
                            break;
                        }

                        if (iterCount + 1 == 9999) {
                            throw new TimeoutException(
                                "Too many iterations taken to select room in main path. " +
                                "\nMake sure that at least one room has 2 exits " +
                                "and it's probability greater than 0");
                        }
                    }
                }
            }

            return rooms;
        }
    }
}