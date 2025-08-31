using Enemy.Base;
using Interactable.Damageable;
using Player;
using UnityEngine;

namespace Enemy.Types.SkeletonArcher {
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyArrow : MonoBehaviour {
        private Rigidbody _body;
        private EnemyBase _owner;
        [SerializeField] private float _launchSpeed;
        [SerializeField] private float _timeToLive;

        private float _currentLiveTime;
        
        public void Launch(EnemyBase owner) {
            _owner = owner; 
            _body.AddForce(transform.forward * _launchSpeed, ForceMode.Impulse);
        }

        private void Awake() => Initialize();

        private void Update() {
            UpdateTimeToLive();
            RotateInMotionDirection();
        }

        private void OnCollisionEnter(Collision other) => DamageIfPlayer(other);
        
        private void Initialize() {
            _body = GetComponent<Rigidbody>();
        }
        
        private void UpdateTimeToLive() {
            _currentLiveTime += Time.deltaTime;
            if (_currentLiveTime >= _timeToLive) Destroy(gameObject);
        }

        private void RotateInMotionDirection() {
            transform.forward = _body.linearVelocity.normalized;
        }

        private void DamageIfPlayer(Collision other) {
            if (other.gameObject.TryGetComponent<PlayerController>(out var player)) {
                var info = DamageInfoBuilder.EnemyOnPlayer(_owner);
                player.TakeDamage(info);
            }

            Destroy(gameObject);
        }
    }
}