using System;
using System.Collections.Generic;
using Core.Level.Room;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Level.Generator {
    /// <summary>
    /// Settings that will be used to create new levels.
    /// Defines everything necessary for level generator.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "Scriptable Objects/LevelSettings")]
    public class LevelSettings : ScriptableObject {
        [Header("Room pool")]
        [Tooltip("Room where player will start this level")]
        [SerializeField] private RoomInfo _startRoom;
        [Tooltip("Pool of level that can generate")]
        [SerializeField] private RoomInfo[] _rooms;
        [Tooltip("Very last room of this level")]
        [SerializeField] private RoomInfo _finalRoom;
        [Tooltip("Prefab that will block unused exits")]
        [SerializeField] private GameObject _pathBlocker;

        [Header("Level Structure")]
        [SerializeField] private int _roomsBetweenStartAndEnd;
        [SerializeField] private int _additionalRooms;
        [Tooltip("Chance for each room from rooms array to appear. \n" +
                 "each room's chance ranges from last cell value to current\n" +
                 "First room chance starts from 0.")]
        [SerializeField] private float[] _probabilityRange;

        /// <summary> Room where player will start this level </summary>
        public RoomInfo StartRoom => _startRoom;
        /// <summary> Very last room of this level </summary>
        public RoomInfo FinalRoom => _finalRoom;
        /// <summary> Prefab that will block unused exits </summary>
        public GameObject PathBlocker => _pathBlocker;
        /// <summary>
        /// Returns random room from rooms pool.
        /// If random range was defined it will use this range over random.
        /// </summary>
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
        /// <summary>
        /// Creates an array of rooms that will be placed between start and end.
        /// Each room will have at least 2 exits and at least one will have more than 2.
        /// </summary>
        /// <returns> Array of rooms that should be placed between start and end rooms </returns>
        /// <exception cref="TimeoutException"> Appears when max generation attempts was reached </exception>
        public RoomInfo[] GetMainPathRooms(int maxGenerationAttempts=999) {
            var hasBranchOut = false;
            var path = new RoomInfo[_roomsBetweenStartAndEnd]; 
            for (var i = 0; i < _roomsBetweenStartAndEnd; i++) {
                for (var iterCount = 0; iterCount < maxGenerationAttempts; iterCount++) {
                    var randomRoom = GetRandomRoom();
                    // In case path consists only of 2-exit rooms
                    if (i + 1 == _roomsBetweenStartAndEnd - 1 && !hasBranchOut) {
                        if (randomRoom.ExitCount > 2) {
                            path[i] = randomRoom;
                            break;
                        }
                    }
                    else {
                        if (randomRoom.ExitCount > 1) {
                            path[i] = randomRoom;

                            if (randomRoom.ExitCount > 2) hasBranchOut = true;
                            break;
                        }
                    }
                    // Too many attempts
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
        /// <summary>
        /// Returns queue of random rooms that supposed to will level.
        /// Specific order of rooms can prevent blocking all existing exits, still this issue may occur.
        /// </summary>
        /// <param name="existingExitsCount"> Current existing exit in level </param>
        /// <returns> Queue of rooms that should be to fill the level </returns>
        /// <exception cref="TimeoutException"> Appears when max generation attempts was reached </exception>
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