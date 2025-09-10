using System;
using System.Collections.Generic;
using Core.Behaviour.SingletonBehaviour;
using Player;
using UI.Managers;
using UI.Views.Pause;
using UnityEngine;

namespace UI {
    public class UIManager : SingletonBase<UIManager> {
        private Dictionary<Type, CanvasView> _uiCanvases;
        private Dictionary<Type, CanvasView> _hudCanvases;

        [SerializeField] private CanvasView[] _sceneUiCanvases;
        [SerializeField] private CanvasView[] _sceneHudCanvases;

        public static event Action PauseStateEntered;
        public static event Action PauseStateExited;

        private Stack<CanvasView> _uiStack;
        private bool _isPause;
        private Action _unsubscribeAction;

        public CrosshairView Crosshair => GetHUDCanvas<CrosshairView>();

        private void OnEnable() {
            void HandlePausePress() {
                if (_isPause) ExitLastCanvas();
                else EnterCanvas<PauseView>();
            }
            
            PlayerEvents.PausePressed += HandlePausePress;

            _unsubscribeAction = () => {
                PlayerEvents.PausePressed -= HandlePausePress;
            };
        }

        protected override void Awake() {
            ToSingleton(false);
            SortCanvases();
            Initialize();
        }
        
        private void Start() => DisableUICanvases();

        private void OnDisable() => _unsubscribeAction();

        private void EnterPauseState() {
            _isPause = true;
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            PauseStateEntered?.Invoke();
        }

        private void ExitPauseState() {
            _isPause = false;
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            PauseStateExited?.Invoke();
        }

        public void EnterCanvas<T>() where T : CanvasView {
            if (!_isPause) EnterPauseState();
            
            if (_uiStack.Count > 0) _uiStack.Peek().ExitCanvas();
            var canvas = GetCanvas<T>();
            _uiStack.Push(canvas);
            canvas.EnterCanvas();
        }

        public void ExitLastCanvas() {
            if (_uiStack.Count > 0) _uiStack.Pop().ExitCanvas();

            if (_uiStack.Count == 0) ExitPauseState();
            else _uiStack.Peek().EnterCanvas();
        }

        public T GetUICanvas<T>() where T : CanvasView => (T)_uiCanvases[typeof(T)];
        public T GetHUDCanvas<T>() where T : CanvasView => (T)_hudCanvases[typeof(T)];

        private T GetCanvas<T>() where T : CanvasView {
            if (_uiCanvases.TryGetValue(typeof(T), out var uiCanvas)) {
                return (T)uiCanvas;
            }
            
            if (_hudCanvases.TryGetValue(typeof(T), out var hudCanvas)) {
                return (T)hudCanvas;
            }
            // No canvas found
            return null;
        }

        private void SortCanvases() {
            _hudCanvases = new Dictionary<Type, CanvasView>();
            foreach (var hudCanvas in _sceneHudCanvases) {
                _hudCanvases.Add(hudCanvas.GetType(), hudCanvas);
            }

            _uiCanvases = new Dictionary<Type, CanvasView>();
            foreach (var uiCanvas in _sceneUiCanvases) {
                _uiCanvases.Add(uiCanvas.GetType(), uiCanvas);
            }
        }
        
        private void Initialize() {
            _isPause = false;
            _uiStack = new Stack<CanvasView>();
        }
        
        private void DisableUICanvases() {
            foreach (var uiCanvas in _uiCanvases.Values) {
                // Safe exit from canvas (disables only canvas, not gameObject)
                if (!uiCanvas.gameObject.activeInHierarchy) {
                    uiCanvas.gameObject.SetActive(true);
                }
                uiCanvas.ExitCanvas();
            }
        }
    }
}