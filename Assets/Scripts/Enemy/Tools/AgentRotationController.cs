using System;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Tools {
    public class AgentRotationController : MonoBehaviour {
        [SerializeField] private NavMeshAgent _agent;

        private Action _rotationUpdater;

        private void Awake() => SetDefaultMode();

        public void SetDefaultMode() {
            _agent.updateRotation = true;

            _rotationUpdater = () => { };
        }

        public void LookAt(Vector3 point) {
            _agent.updateRotation = false;
            
            transform.LookAt(point);

            _rotationUpdater = () => { };
        } 

        public void SetObservationPoint(Transform point, float angularSpeed=45) {
            _agent.updateRotation = false;
            
            _rotationUpdater = () => {
                // Calculate direction to target (flatten Y component)
                Vector3 direction = point.position - transform.position;
                direction.y = 0; // Constraint to Y-axis only

                if (direction != Vector3.zero) {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                        angularSpeed * Time.deltaTime);
                }
            };
        }

        public void SetLocalDirection(Vector3 direction) {
            _agent.updateRotation = false;

            transform.localRotation = Quaternion.LookRotation(ToHorizontalPosition(direction));

            _rotationUpdater = () => { };
        }

        public void SetGlobalDirection(Vector3 direction) {
            _agent.updateRotation = false;

            transform.rotation = Quaternion.LookRotation(ToHorizontalPosition(direction));

            _rotationUpdater = () => { };
        }

        private void Update() => _rotationUpdater();

        private Vector3 ToHorizontalPosition(Vector3 position) => 
            new Vector3(position.x, 0, position.z);
    }
}