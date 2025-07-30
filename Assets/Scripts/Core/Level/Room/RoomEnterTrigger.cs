using Core.Game;
using UnityEngine;

namespace Core.Level.Room {
    [RequireComponent(typeof(BoxCollider))]
    public class RoomEnterTrigger : MonoBehaviour {
        private RoomInfo _attachedRoom;

        public void SetReference(RoomInfo reference) => _attachedRoom = reference;

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) 
                GameManager.Instance.EnterBattleState(_attachedRoom);
        }
    }
}