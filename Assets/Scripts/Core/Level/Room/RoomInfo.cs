using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Level.Room.Enemy;
using Enemy;
using Enemy.Base;
using Interactable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Level.Room {
    /// <summary>  All info needed about each specific room. </summary>
    public class RoomInfo : MonoBehaviour {
        [SerializeField] private bool _isBossBattle;
        // Exits/Entrances that this room has
        [SerializeField] private List<Transform> _exits;
        private List<Transform> _removedExits;
        // Prefab that will block exits when player enters this room
        [SerializeField] private GameObject _exitBlocker;
        private List<GameObject> _exitBlockers;
        // Info about this room shape. Needed to check ability to place this room.
        [SerializeField] private BoxColliderInfo[] _roomColliders;
        // Point where enemy spawn within this room 
        [SerializeField] private Transform[] _enemySpawnPoints;
        // All the info about this room enemies and their spawning logic
        [SerializeField] private EnemyInfo _enemyInfo;
        [SerializeField] private Transform _enemyParent;
        private int _currentEnemyCount;
        private Queue<EnemyBase> _enemyToSpawn;
        [SerializeField] private RoomEnterTrigger[] _roomEnterTriggers;
        [SerializeField] private int _prefabId;
        private Dictionary<int, DamageableBehaviour> _activeEnemies;
        public int PrefabId => _prefabId;   // Identifies this room, shared with other same rooms
        public int RoomId { get; private set; } // Identifies this room in level
        public bool IsBossBattle => _isBossBattle;
        
        public List<Transform> ExitPositions => _exits;
        public int ExitCount => _exits.Count;
        public DamageableBehaviour[] ActiveEnemies => _activeEnemies.Values.ToArray();

        private void Awake() {
            _exitBlockers = new List<GameObject>();
            _enemyToSpawn = new Queue<EnemyBase>();
            _removedExits = new List<Transform>();
            _activeEnemies = new Dictionary<int, DamageableBehaviour>();
            
            foreach (var trigger in _roomEnterTriggers) trigger.SetReference(this);
        }

        /// <summary>
        /// Tries to place this instance of room at given position by specific exit that exists within this room.
        /// </summary>
        /// <param name="otherEntrance"> Position where room will be placed </param>
        /// <param name="thisExit"> Exit within this room that that will be attached to entrance </param>
        /// <param name="parent"> Parent to be attached to </param>
        /// <param name="generatedRoom"> returns room that was generated as RoomInfo </param>
        /// <returns> Whether operation was successful or not </returns>
        public bool TryInstantiateSelf(Transform otherEntrance, Transform thisExit, Transform parent, out RoomInfo generatedRoom) {
            generatedRoom = null;
            // Get rotation room need to be facing
            Quaternion targetRotation = otherEntrance.rotation * Quaternion.Euler(0, 180, 0);
            // difference between two angles
            Quaternion rotationDifference = targetRotation * Quaternion.Inverse(thisExit.rotation);
            // Calculate where the room center should be placed (assuming room center is at 0,0,0)
            Vector3 roomPosition = otherEntrance.position - rotationDifference * thisExit.localPosition;
            
            if (!IsRoomAreaClear(roomPosition, rotationDifference)) return false;
            
            // Instantiate the room with the calculated position and rotation
            var room = Instantiate(gameObject, roomPosition, rotationDifference);
            room.transform.SetParent(parent, true);
            generatedRoom = room.GetComponent<RoomInfo>();
    
            // Remove the used exit
            generatedRoom.RemoveUsedExit(thisExit);
            return true;
        }
        /// <summary>
        /// Instantiates on (0, 0, 0) of given parent. Use to place start room.  
        /// </summary>
        /// <param name="parent"> Parent to be attached to </param>
        /// <returns> Creates instance of this room as RoomInfo </returns>
        public RoomInfo InstantiateOnZero(Transform parent) {
            var start = Instantiate(gameObject, parent, false).GetComponent<RoomInfo>();
            start.name = name;  // To remove -(Clone) suffix
            return start;
        }
        /// <summary> Check if room area is clear before placing </summary>
        private bool IsRoomAreaClear(Vector3 worldPosition, Quaternion worldRotation) {
            return _roomColliders.All(boxCollider =>
                !CheckBoxCollider(boxCollider, worldPosition, worldRotation));
        }
        /// <summary> Check specific area described as BoxColliderInfo for existing objects inside </summary>
        private bool CheckBoxCollider(BoxColliderInfo box, Vector3 roomWorldPosition, Quaternion roomWorldRotation) {
            // Transform local position to world space
            Vector3 colliderWorldPos = roomWorldPosition + roomWorldRotation * box.Center;
        
            return Physics.CheckBox(
                colliderWorldPos,
                box.Size * 0.5f, // CheckBox uses half extents
                roomWorldRotation // Use room's rotation since local transform is identity
            );
        }
        /// <summary>
        /// Remove transform (exit) from this room. Should be used over direct access to array.
        /// </summary>
        /// <param name="exit"> Exit to be removed </param>
        private void RemoveUsedExit(Transform exit) {
            // Direct check for position and rotation, because Transform compares by reference.
            foreach (var thisRoomExit in _exits) {
                if (thisRoomExit.localPosition != exit.localPosition || 
                    thisRoomExit.localRotation != exit.localRotation) continue;
                
                _removedExits.Add(thisRoomExit);
                _exits.Remove(thisRoomExit);
                break;
            }
        }
        /// <summary> Set id of this room (level based) </summary>
        public void SetID(int newId) => RoomId = newId;

        private bool TrySpawnNextEnemy() {
            var nextEnemy = _enemyInfo.GetNextEnemy();

            if (nextEnemy == null) {
                if (_enemyInfo.IsWaveBased && _currentEnemyCount == 0 && _enemyInfo.AdvanceWave()) {
                    foreach (var _ in _enemySpawnPoints) TrySpawnNextEnemy();
                    return true;
                }

                if (_currentEnemyCount == 0) RoomEvents.OnCombatFinished();
                return true;
            }
            
            var randomPoint = _enemySpawnPoints[Random.Range(0, _enemySpawnPoints.Length)];
            var spawnPosition = randomPoint.localPosition;
            if (SpawnPointIsOccupied(randomPoint, nextEnemy.ColliderSize)) {
                foreach (var spawnPoint in _enemySpawnPoints) {
                    if (SpawnPointIsOccupied(spawnPoint, nextEnemy.ColliderSize)) continue;
                    spawnPosition = spawnPoint.localPosition;
                    spawnPosition.y += nextEnemy.ColliderSize.y / 2;
                    
                    _currentEnemyCount++;
                    InstantiateNewEnemy(nextEnemy, spawnPosition, spawnPoint.rotation, _enemyParent);
                    Physics.SyncTransforms();
                    return true;
                }
                
                _enemyToSpawn.Enqueue(nextEnemy);
                StartCoroutine(TrySpawnEnemyLater(1f, nextEnemy.ColliderSize));
                return false;
            }
            
            spawnPosition.y += nextEnemy.ColliderSize.y / 2;
            _currentEnemyCount++;
            InstantiateNewEnemy(nextEnemy, spawnPosition, randomPoint.rotation, _enemyParent);
            Physics.SyncTransforms();
            return true;
        }

        private static bool SpawnPointIsOccupied(Transform point, Vector3 colliderSize) {
            var boxCenter = point.position;
            boxCenter.y += colliderSize.y / 2;
            var adjustedSize = colliderSize / 2f * 1.05f;

            return Physics.CheckBox(boxCenter, adjustedSize, Quaternion.identity);
        }

        private IEnumerator TrySpawnEnemyLater(float delayBetweenChecks, Vector3 colliderSize) {
            if (_enemyToSpawn.Count == 0) yield break;
            if (_enemyInfo.IsBountyBased && _enemyInfo.AllTargetsDefeated) {
                _enemyToSpawn.Clear();
                if (_currentEnemyCount > 0) RoomEvents.OnCombatFinished();
                yield break;
            }

            yield return new WaitForSeconds(delayBetweenChecks);
            
            foreach (var spawnPoint in _enemySpawnPoints) {
                if (SpawnPointIsOccupied(spawnPoint, colliderSize)) continue;

                var newEnemy = _enemyToSpawn.Dequeue();
                _currentEnemyCount++;
                
                var spawnPosition = spawnPoint.localPosition;
                spawnPosition.y += colliderSize.y / 2;
                
                InstantiateNewEnemy(newEnemy, spawnPosition, spawnPoint.rotation, _enemyParent);
                Physics.SyncTransforms();
            }
        }

        public void StartCombat() {
            RoomEvents.OnRoomEntered();
            // Remove all enter triggers
            foreach (var enterTrigger in _roomEnterTriggers) Destroy(enterTrigger.gameObject);
            SetupCombatEvents();

            foreach (var removed in _removedExits) {
                var blocker = Instantiate(_exitBlocker, removed.position, removed.rotation, removed);
                _exitBlockers.Add(blocker);
            }
            // Start Spawning enemies
            foreach (var _ in _enemySpawnPoints) TrySpawnNextEnemy();
        }

        private void SetupCombatEvents() {
            void DecreaseEnemyCount(EnemyBase enemy) {
                _currentEnemyCount--;
                _activeEnemies.Remove(enemy.GetInstanceID());
            }

            void SpawnNextEnemy(EnemyBase _) => TrySpawnNextEnemy();
            
            EnemyEvents.EnemyDefeated += DecreaseEnemyCount;
            EnemyEvents.TargetDefeated += _enemyInfo.TargetDefeated;
            
            if (_enemyInfo.IsBountyBased) EnemyEvents.NormalDefeated += SpawnNextEnemy;
            else EnemyEvents.EnemyDefeated += SpawnNextEnemy;
            
            void Unsubscribe() {
                EnemyEvents.EnemyDefeated -= DecreaseEnemyCount;
                EnemyEvents.TargetDefeated -= _enemyInfo.TargetDefeated;
                
                if (_enemyInfo.IsBountyBased) EnemyEvents.NormalDefeated -= SpawnNextEnemy;
                else EnemyEvents.EnemyDefeated -= SpawnNextEnemy;

                foreach (var blocker in _exitBlockers) Destroy(blocker);

                RoomEvents.CombatFinished -= Unsubscribe;
            }

            RoomEvents.CombatFinished += Unsubscribe;
        }

        private void InstantiateNewEnemy(EnemyBase enemy, Vector3 position, Quaternion rotation,
            Transform parent) {
            var instance = Instantiate(enemy, position, rotation);
            instance.transform.SetParent(parent, false);
            Physics.SyncTransforms();
            _activeEnemies.Add(instance.GetInstanceID(), instance);
        }
    }
}