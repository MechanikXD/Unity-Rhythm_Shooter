using Core.Game;
using UnityEngine;

namespace Core.Level.Room {
    [RequireComponent(typeof(BoxCollider))]
    public class RoomEnterTrigger : MonoBehaviour {
        [SerializeField] private RoomInfo _attachedRoom;
        
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) 
                GameManager.Instance.EnterBattleState(_attachedRoom);
        }
    }
}