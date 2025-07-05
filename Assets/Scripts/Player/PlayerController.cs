using System.Collections;
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
        [SerializeField] private float mouseSensitivity = 50f;
        [SerializeField] private Vector2 cameraPitchBounds = new Vector2(-80f, 80f);
        private float _cameraRotation; // Camera rotation along X axis only
        private float _bodyRotation; // Rotation of player along Y axis
        
        [Header("Movement Settings")]
        [SerializeField] public float gravityMultiplier = 0.85f;
        private Vector2 _moveDirection; // Store data from OnMove method
        [SerializeField] private float moveSpeed = 10f;
        
        public float MoveSpeed => moveSpeed;
        
        // TODO: Add minimum jump duration
        [Header("Jump Settings")]
        [SerializeField] private float coyoteTime = 0.2f;
        [SerializeField] private float verticalJumpSpeed = 5f;
        [SerializeField] private Vector2 jumpDurationBounds = new Vector2(0.15f, 0.35f);
        [SerializeField] private float airborneMoveSpeed = 5f;
        [SerializeField] private float forwardJumpSpeedMultiplier = 3f;
        
        public float CurrentAirborneTime { get; internal set; }

        public float CoyoteTime => coyoteTime;
        public float VerticalJumpSpeed => verticalJumpSpeed;
        public Vector2 JumpDurationBounds => jumpDurationBounds;
        public float AirborneMoveSpeed => airborneMoveSpeed;
        public float ForwardJumpSpeedMultiplier => forwardJumpSpeedMultiplier;
        
        [Header("Dash Settings")]
        [SerializeField] private float dashCooldownTime = 1.2f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashSpeed = 25f;
        public bool DashInCooldown { get; private set; }
        private float _currentDashCooldown;
        
        public float DashDuration => dashDuration;
        public float DashSpeed => dashSpeed;
        
        #endregion

        public bool IsMoving => MoveKey.IsPressed();
        public bool IsJumping => JumpKey.IsPressed();
        public bool IsGrounded => controller.isGrounded;
        
        public Vector3 CameraForward => attachedCamera.forward;
        
        public Vector3 GetCameraRelativeVector(float vectorMultiplier) {
            Vector3 playerRelativeVector = (_moveDirection.y * CameraForward +
                                            _moveDirection.x * attachedCamera.right) *
                                           vectorMultiplier;
            playerRelativeVector.y = 0;
            return playerRelativeVector;
        }

        private IEnumerator RestoreDashLater() {
            yield return new WaitForSeconds(dashCooldownTime);
            yield return new WaitUntil(() => IsGrounded);
            DashInCooldown = false;
        }

        public void StartDashCooldown() {
            DashInCooldown = true;
            StartCoroutine(RestoreDashLater());
        }

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

        private void Update() => _stateMachine.CurrentState.FrameUpdate();

        private void FixedUpdate() => _stateMachine.CurrentState.FixedUpdate();

        public void OnMove(InputValue currentMoveDirection) => _moveDirection = currentMoveDirection.Get<Vector2>();

        public void OnLook(InputValue lookDirection) {
            // Get mouse position
            var look = lookDirection.Get<Vector2>() * (mouseSensitivity * Time.deltaTime);
            // Rotate player instead of camera
            _bodyRotation += look.x;
            transform.rotation = Quaternion.Euler(0f, _bodyRotation, 0f);
            // Camera rotate here
            _cameraRotation -= look.y;
            _cameraRotation = Mathf.Clamp(_cameraRotation, cameraPitchBounds.x, cameraPitchBounds.y);
            attachedCamera.localRotation = Quaternion.Euler(_cameraRotation, 0f, 0f);
        }

        #endregion
    }
}