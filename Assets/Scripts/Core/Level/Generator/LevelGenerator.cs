using System;
using System.Collections.Generic;
using Core.Level.Room;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Core.Level.Generator {
    /// <summary>
    /// Class that procedurally generates levels from given settings. 
    /// </summary>
    public static class LevelGenerator {
        private static Transform _main; // game object where all prefabs will be instantiated  
        private static Transform _origin; // Where level will be created
        private static LevelSettings _levelSettings;

        private static List<Transform> _availableExits;
        private static Random _seedGenerator;
        private static Random _levelGenerator;
        private static int _seed;

        private const int MaxRetryAttempts = 100;
        /// <summary>
        /// Creates new level using given settings
        /// </summary>
        /// <param name="settings"> Scriptable object that defines everything necessary for level creation </param>
        public static void CreateNewLevel(LevelSettings settings) {
            // TRY TO BREAK NOW, MF'ER
            try {
                Initialize(settings);
                // Place rooms between start and finish
                if (!TryPlaceMainPath()) {
                    var mainPathWasGenerated = false;
                    while (!mainPathWasGenerated) {
                        DestroyAllExceptStart();
                        mainPathWasGenerated = TryPlaceMainPath();
                    }
                }
                // Fill the level side rooms
                if (!TryPlaceAdditionalRooms()) {
                    // Probably something went wrong with level structure so some of the room couldn't generate
                    // Simple solution: exit and try again.
                    throw new ApplicationException("Couldn't generate all additional rooms");
                }
                BlockExistingExits();
            }
            catch (Exception) {
                DestroyAll();
                CreateNewLevel(settings);
            }
        }

        private static void Initialize(LevelSettings settings) {
            var level = new GameObject("Level") {
                transform = {
                    position = _origin.position
                }
            };

            _levelSettings = settings;
            _main = level.transform;
            _seedGenerator ??= new Random();
            _seed = _seedGenerator.Next();
            _levelGenerator = new Random(_seed);
            _availableExits = new List<Transform>();
        }
        
        private static bool TryPlaceMainPath() {
            PlaceStart();

            var mainPath = _levelSettings.GetMainPathRooms();
            // Keep amount of exits from last room, so path is always generated forward, not sideways.
            var prevRoomExitCount = 1;
            
            foreach (var room in mainPath) {
                if (!TryPlaceRoomAtExits(room, _availableExits.Count - prevRoomExitCount, 
                        _availableExits.Count)) {
                    return false;
                }
                prevRoomExitCount = room.ExitCount - 1;
            }
            
            return TryPlaceRoomAtExits(_levelSettings.FinalRoom, 
                _availableExits.Count - prevRoomExitCount, _availableExits.Count);
        }
        
        private static bool TryPlaceAdditionalRooms() {
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
        /// <summary>
        /// Seals all unused exits in level. Called after everything else was generated.
        /// </summary>
        private static void BlockExistingExits() {
            foreach (var exit in _availableExits) {
                Object.Instantiate(_levelSettings.PathBlocker, exit);
            }
            _availableExits.Clear();
        }
        /// <summary>
        /// Destroys all rooms except start one, used to try generating level again.
        /// </summary>
        private static void DestroyAllExceptStart() {
            _availableExits.Clear();
            foreach (GameObject child in _main) {
                if (child.name == _levelSettings.StartRoom.name) {
                    _availableExits.AddRange(child.GetComponent<RoomInfo>().ExitPositions);
                    continue;
                }
                
                Object.Destroy(child);
            }
        }
        /// <summary>
        /// Destroys everything that was generated
        /// </summary>
        private static void DestroyAll() {
            Object.Destroy(GameObject.Find("Level"));
            _availableExits.Clear();
        }
        /// <summary>
        /// Places start room. It's unique, since it should be just instantiated, no other logic...
        /// </summary>
        private static void PlaceStart() {
            var start = _levelSettings.StartRoom.InstantiateOnZero(_main);
            _availableExits.AddRange(start.ExitPositions);
        }
        /// <summary>
        /// Tries to place rooms at given range of indexes.
        /// Intended to try place room near ANY of other rooms exits. 
        /// </summary>
        /// <param name="room"> Room to be placed </param>
        /// <param name="minIndex"> min inclusive index of exit </param>
        /// <param name="maxIndex"> max exclusive index of exit </param>
        /// <returns> Whether operation was successful or not </returns>
        private static bool TryPlaceRoomAtExits(RoomInfo room, int minIndex, int maxIndex) {
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
        /// <summary>
        /// Tries to place room at specific position that should be located within _availableExits array.  
        /// </summary>
        /// <param name="room"> Room to be place at given position </param>
        /// <param name="position"> Position where to place new room </param>
        /// <returns> Whether operation was successful or not </returns>
        private static bool TryPlaceRoom(RoomInfo room, Transform position) {
            var roomExits = room.ExitPositions;
            
            foreach (var exit in roomExits) {
                if (!room.TryInstantiateSelf(position, exit, _main,
                        out var newRoom)) continue;
                _availableExits.Remove(position);
                _availableExits.AddRange(newRoom.ExitPositions);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Tries to place room anywhere within the level.
        /// </summary>
        /// <param name="room"> Room to be placed </param>
        /// <returns> Whether operation was successful or not </returns>
        private static bool TryPlaceRoom(RoomInfo room) {
            return TryPlaceRoomAtExits(room, 0, _availableExits.Count);
        }
    }
}