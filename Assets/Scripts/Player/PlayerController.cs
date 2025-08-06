using System.Collections;
using Core.Behaviour.FiniteStateMachine;
using Core.Music;
using Core.Music.Songs.Scriptable_Objects;
using Player.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform _attachedCamera;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Revolvers _currentWeapon = new Revolvers();
        private StateMachine _stateMachine;

        [Header("MOVE SOMEWHERE ELSE")]
        [SerializeField] private SongData _songData;

        [SerializeField] private AudioSource _songSource;

        private bool _leftActionBuffered;
        private bool _rightActionBuffered;
        private float _currentBufferTime;
        private bool _doBufferCounting;
        [SerializeField] private float _maxInputBufferTime = 0.1f;

        public PlayerStates States { get; private set; }
        public CharacterController Controller => _controller;
        public Vector3 Position => transform.position;

        public InputAction MoveKey { get; private set; }
        public InputAction JumpKey { get; private set; }
        public InputAction DashKey { get; private set; }

        #region Player Settings

        [Header("Mouse Settings")]
        [SerializeField] private float _mouseSensitivity = 50f;
        [SerializeField] private Vector2 _cameraPitchBounds = new Vector2(-80f, 80f);
        private float _cameraRotation; // Camera rotation along X axis only
        private float _bodyRotation; // Rotation of player along Y axis

        [Header("Movement Settings")]
        [SerializeField] public float _gravityMultiplier = 0.85f;
        private Vector2 _moveDirection; // Store data from OnMove method
        [SerializeField] private float _moveSpeed = 10f;

        public float MoveSpeed => _moveSpeed;

        [Header("Jump Settings")]
        [SerializeField] private float _coyoteTime = 0.2f;
        [SerializeField] private float _verticalJumpSpeed = 5f;
        [SerializeField] private Vector2 _jumpDurationBounds = new Vector2(0.15f, 0.35f);
        [SerializeField] private float _airborneMoveSpeed = 10f;

        public float CurrentAirborneTime { get; internal set; }

        public float CoyoteTime => _coyoteTime;
        public float VerticalJumpSpeed => _verticalJumpSpeed;
        public Vector2 JumpDurationBounds => _jumpDurationBounds;
        public float AirborneMoveSpeed => _airborneMoveSpeed;

        [Header("Dash Settings")]
        [SerializeField] private float _dashCooldownTime = 1.2f;
        [SerializeField] private float _dashDuration = 0.2f;
        [SerializeField] private float _dashSpeed = 25f;
        public bool DashInCooldown { get; private set; }
        private float _currentDashCooldown;

        public float DashDuration => _dashDuration;
        public float DashSpeed => _dashSpeed;

        #endregion

        public bool IsMoving => MoveKey.IsPressed();
        public bool IsJumping => JumpKey.IsPressed();
        public bool IsGrounded => _controller.isGrounded;

        public Vector3 CameraForward => _attachedCamera.forward;

        public Vector3 GetCameraRelativeVector(float vectorMultiplier) {
            Vector3 playerRelativeVector = (_moveDirection.y * CameraForward +
                                            _moveDirection.x * _attachedCamera.right) *
                                           vectorMultiplier;
            playerRelativeVector.y = 0;
            return playerRelativeVector;
        }

        private IEnumerator RestoreDashLater() {
            yield return new WaitForSeconds(_dashCooldownTime);
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
            States = new PlayerStates(_stateMachine, this);
            _stateMachine.Initialize(States.IdleState);
            _currentWeapon.OnWeaponSelected();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            MoveKey = _playerInput.actions["Move"];
            JumpKey = _playerInput.actions["Jump"];
            DashKey = _playerInput.actions["Dash"];
        }

        private void Start() {
            Conductor.Instance.Initialize(_songData, _songSource);
        }

        private void Update() {
            _stateMachine.CurrentState.FrameUpdate();
            _currentWeapon.WeaponUpdate();

            if (_doBufferCounting) {
                _currentBufferTime += Time.deltaTime;
                if (_currentBufferTime <= _maxInputBufferTime) return;
                var songPositionWhenHit = Conductor.Instance.SongPosition - _currentBufferTime;

                // ReSharper disable Unity.PerformanceCriticalCodeInvocation
                if (_leftActionBuffered)
                    PlayerActionEvents.OnPlayerLeftAction(songPositionWhenHit, _currentWeapon);
                if (_rightActionBuffered)
                    PlayerActionEvents.OnPlayerRightAction(songPositionWhenHit, _currentWeapon);

                // ReSharper restore Unity.PerformanceCriticalCodeInvocation

                _doBufferCounting = false;
                _currentBufferTime = 0f;
                _leftActionBuffered = false;
                _rightActionBuffered = false;
            }
        }

        private void FixedUpdate() => _stateMachine.CurrentState.FixedUpdate();

        public void OnMove(InputValue currentMoveDirection) =>
            _moveDirection = currentMoveDirection.Get<Vector2>();

        public void OnLook(InputValue lookDirection) {
            // Get mouse position
            var look = lookDirection.Get<Vector2>() * (_mouseSensitivity * Time.deltaTime);

            // Rotate player instead of camera
            _bodyRotation += look.x;
            transform.rotation = Quaternion.Euler(0f, _bodyRotation, 0f);

            // Camera rotate here
            _cameraRotation -= look.y;
            _cameraRotation =
                Mathf.Clamp(_cameraRotation, _cameraPitchBounds.x, _cameraPitchBounds.y);
            _attachedCamera.localRotation = Quaternion.Euler(_cameraRotation, 0f, 0f);
        }

        public void OnLeftAction() {
            if (_rightActionBuffered) {
                var currentSongPosition = Conductor.Instance.SongPosition;
                PlayerActionEvents.OnPlayerBothAction(currentSongPosition, _currentWeapon);
                _rightActionBuffered = false;
                _doBufferCounting = false;
                _currentBufferTime = 0f;
            }
            else {
                _leftActionBuffered = true;
                _doBufferCounting = true;
                _currentBufferTime = 0f;
            }
        }

        public void OnRightAction() {
            if (_leftActionBuffered) {
                var currentSongPosition = Conductor.Instance.SongPosition;
                PlayerActionEvents.OnPlayerBothAction(currentSongPosition, _currentWeapon);
                _leftActionBuffered = false;
                _doBufferCounting = false;
                _currentBufferTime = 0f;
            }
            else {
                _rightActionBuffered = true;
                _doBufferCounting = true;
                _currentBufferTime = 0f;
            }
        }
        
        public void OnReload() {}

        #endregion
    }
}