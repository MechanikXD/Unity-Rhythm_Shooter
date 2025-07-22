using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Level.Room {
    public class RoomInfo : MonoBehaviour {
        // Exits/Entrances that this room has
        [SerializeField] private List<Transform> _exits;
        // Info about this room shape. Needed to check ability to place this room.
        [SerializeField] private BoxColliderInfo[] _roomColliders;
        // Point where enemy spawn within this room 
        [SerializeField] private Transform[] _enemySpawnPoints;
        // Also there should be room entrance colliders,
        // room ID (unique to each created room), room prefab ID (unique to room prefabs) 
        
        public List<Transform> ExitPositions => _exits;
        public int ExitCount => _exits.Count;

        public bool TryInstantiateSelf(Transform otherEntrance, Transform thisExit, Transform parent, out RoomInfo generatedRoom) {
            generatedRoom = null;
            // Get rotation room need to be facing
            Quaternion targetRotation = otherEntrance.rotation * Quaternion.Euler(0, 180, 0);
            // difference between two angles
            Quaternion rotationDifference = targetRotation * Quaternion.Inverse(thisExit.rotation);
            // Calculate where the room center should be placed (assuming room center is at 0,0,0)
            Vector3 roomPosition = otherEntrance.position - rotationDifference * thisExit.localPosition;
            
            if (!IsRoomAreaClear(roomPosition, rotationDifference)) {
                Debug.Log("Cannot place room - area is occupied!");
                return false;
            }
            
            // Instantiate the room with the calculated position and rotation
            var room = Instantiate(gameObject, roomPosition, rotationDifference);
            room.transform.SetParent(parent, true);
            generatedRoom = room.GetComponent<RoomInfo>();
    
            // Remove the used exit
            generatedRoom.RemoveUsedExit(thisExit);
            // TODO: Check for other exits overlapping/blocking
            return true;
        }
        
        public RoomInfo InstantiateOnZero(Transform parent) {
            return Instantiate(gameObject, parent, false).GetComponent<RoomInfo>();
        }
        
        // Check if room area is clear before placing
        private bool IsRoomAreaClear(Vector3 worldPosition, Quaternion worldRotation) {
            return _roomColliders.All(boxCollider =>
                !CheckBoxCollider(boxCollider, worldPosition, worldRotation));
        }

        private bool CheckBoxCollider(BoxColliderInfo box, Vector3 roomWorldPosition, Quaternion roomWorldRotation) {
            // Transform local position to world space
            Vector3 colliderWorldPos = roomWorldPosition + roomWorldRotation * box.Center;
        
            return Physics.CheckBox(
                colliderWorldPos,
                box.Size * 0.5f, // CheckBox uses half extents
                roomWorldRotation // Use room's rotation since local transform is identity
            );
        }

        private void RemoveUsedExit(Transform exit) {
            // Direct check for position and rotation, because Transform compares by reference.
            foreach (var thisRoomExit in _exits) {
                if (thisRoomExit.localPosition != exit.localPosition || 
                    thisRoomExit.localRotation != exit.localRotation) continue;

                _exits.Remove(thisRoomExit);
                break;
            }
        }
    }
}