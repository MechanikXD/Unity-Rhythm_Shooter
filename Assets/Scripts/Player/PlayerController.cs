using Core.StateMachine.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour {
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform attachedCamera;
        [SerializeField] private PlayerInput playerInput;
        private StateMachine _stateMachine;

        public States States { get; private set; }
        public CharacterController Controller => controller;
        
        public InputAction MoveKey { get; private set; }
        public InputAction JumpKey { get; private set; }
        public InputAction DashKey { get; private set; }

        #region Player Settings

        [Header("Mouse Settings")]
        [SerializeField] private float mouseSensitivity = 30f;
        private Vector2 _lookDirection; // Store data from OnLook method
        private float _cameraRotation; // Camera rotation along X axis only
        private float _bodyRotation; // Rotation of player along Y axis
        
        [Header("Movement Settings")]
        [SerializeField] public float gravityMultiplier = 1.2f;
        private Vector2 _moveDirection; // Store data from OnMove method
        [SerializeField] private float moveSpeed = 8f;
        
        public float MoveSpeed => moveSpeed;
        
        // TODO: Add minimum jump duration
        [Header("Jump Settings")]
        [SerializeField] private float coyoteTime = 0.2f;
        [SerializeField] private float verticalJumpSpeed = 10f;
        [SerializeField] private float jumpMaxDuration = 0.2f;
        [SerializeField] private float airborneMoveSpeed = 4f;
        [SerializeField] private float forwardJumpSpeedMultiplier = 3f;
        private float _currentAirborneTime;
        
        public float VerticalJumpSpeed => verticalJumpSpeed;
        public float JumpMaxDuration => jumpMaxDuration;
        public float AirborneMoveSpeed => airborneMoveSpeed;
        public float ForwardJumpSpeedMultiplier => forwardJumpSpeedMultiplier;
        
        [Header("Dash Settings")]
        [SerializeField] private float dashCooldownTime = 1.2f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashSpeed = 20f;
        public bool dashInCooldown;
        private float _currentDashCooldown;
        
        public float DashDuration => dashDuration;
        public float DashSpeed => dashSpeed;
        
        #endregion

        public bool IsMoving => MoveKey.IsPressed();
        public bool IsJumping => JumpKey.IsPressed();
        public bool IsGrounded => controller.isGrounded;
        
        public Vector3 CameraForward => attachedCamera.forward;
        
        public Vector3 GetCameraRelativeVector(float vectorMultiplier) {
            Vector3 playerRelativeVector = (_moveDirection.y * attachedCamera.forward +
                                            _moveDirection.x * attachedCamera.right) *
                                           vectorMultiplier;
            playerRelativeVector.y = 0;
            return playerRelativeVector;
        }

        public bool CanJump() => IsGrounded || _currentAirborneTime < coyoteTime;

        #region Event Functions
        
        private void Awake() {
            _stateMachine = new StateMachine();
            States = new States(_stateMachine, this);
            _stateMachine.Initialize(States.IdleState);
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            MoveKey = playerInput.actions["Move"];
            JumpKey = playerInput.actions["Jump"];
            DashKey = playerInput.actions["Dash"];
        }

        private void Update() {
            _stateMachine.CurrentState.FrameUpdate();
            // TODO: Make it with invoke
            if (dashInCooldown) {
                _currentDashCooldown += Time.deltaTime;

                // Refresh only on ground
                if (_currentDashCooldown >= dashCooldownTime && IsGrounded) {
                    dashInCooldown = false;
                    _currentDashCooldown = 0f;
                }
            }
            
            if (IsGrounded) {
                _currentAirborneTime = 0;
            }
            else {
                _currentAirborneTime += Time.deltaTime;
            }
            
            // Get mouse position
            var lookDirection = _lookDirection * (mouseSensitivity * Time.deltaTime);

            // Rotate player instead of camera
            _bodyRotation += lookDirection.x;
            transform.rotation = Quaternion.Euler(0f, _bodyRotation, 0f);

            // Camera rotate here
            _cameraRotation -= lookDirection.y;
            _cameraRotation = Mathf.Clamp(_cameraRotation, -70f, 70f);
            attachedCamera.localRotation = Quaternion.Euler(_cameraRotation, 0f, 0f);
        }

        private void FixedUpdate() => _stateMachine.CurrentState.FixedUpdate();

        public void OnMove(InputValue currentMoveDirection) => this._moveDirection = currentMoveDirection.Get<Vector2>();

        public void OnLook(InputValue lookDirection) => _lookDirection = lookDirection.Get<Vector2>();
        
        #endregion
    }
}