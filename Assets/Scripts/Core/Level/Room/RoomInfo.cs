using System.Collections.Generic;
using UnityEngine;

namespace Core.Level.Room {
    public class RoomInfo : MonoBehaviour {
        [SerializeField] private List<Transform> exits;
        [SerializeField] private Transform[] enemySpawnPoints;
        [SerializeField] private GameObject roomColliders;

        public List<Transform> ExitPositions => exits;

        public RoomInfo InstantiateSelf(Transform otherEntrance, Transform thisExit, Transform parent) {
            // Get rotation room need to be facing
            Quaternion targetRotation = otherEntrance.rotation * Quaternion.Euler(0, 180, 0);
            // difference between two angles
            Quaternion rotationDifference = targetRotation * Quaternion.Inverse(thisExit.rotation);
            // Calculate where the room center should be placed (assuming room center is at 0,0,0)
            Vector3 roomPosition = otherEntrance.position - rotationDifference * thisExit.localPosition;
    
            // Instantiate the room with the calculated position and rotation
            var room = Instantiate(gameObject, roomPosition, rotationDifference);
            room.transform.SetParent(parent, true);
            var createdRoomInfo = room.GetComponent<RoomInfo>();
    
            // Remove the used exit
            createdRoomInfo.exits.Remove(thisExit);
            return createdRoomInfo;

            // TODO: Check if other exits alligned/got blocked
        }
        
        public RoomInfo InstantiateOnZero(Transform parent) {
            return Instantiate(gameObject, parent, false).GetComponent<RoomInfo>();
        }
        
        public bool CanBePlacedAt(Transform otherEntrance, Transform thisExit, Transform parent) {
            /*var rotation = -thisExit.rotation.eulerAngles;
            var position = thisExit.position;
            if (rotation != Vector3.zero) {
                position = RotatePointAroundPivot(position, Vector3.zero, rotation);
            }
            
            var checkCollider = Instantiate(roomColliders, otherEntrance.position - position, Quaternion.Euler(rotation));
            checkCollider.transform.SetParent(parent, false);
            // TODO: wrong was of detecting collision
            var hasContacts = roomColliders.GetComponents<BoxCollider>().Any(c => c.providesContacts);
            Destroy(checkCollider);
            return !hasContacts;*/
            return true;
        }
        
        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }
    }
}