using System;
using System.Collections.Generic;
using Player;
using UI.Managers;
using UI.Views.OfferingSelection;
using UI.Views.Pause;
using UI.Views.Score;
using UnityEngine;

namespace UI {
    public class UIManager : MonoBehaviour {
        public static UIManager Instance;
        [Header("HUD")]
        [SerializeField] private CrosshairBeat _crosshair;
        [SerializeField] private ScoreView _score;

        [Header("Game UI")]
        [SerializeField] private PauseView _pause;
        [SerializeField] private OfferingView _offeringSelection;

        public static event Action PauseStateEntered;
        public static event Action PauseStateExited;

        private Stack<CanvasView> _uiStack;
        private bool _isPause;
        private Action _unsubscribeAction;

        public CrosshairBeat Crosshair => _crosshair;

        private void OnEnable() {
            void HandlePausePress() {
                if (_isPause) {
                    ExitLastCanvas();
                }
                else {
                    EnterCanvas(_pause);
                }
            }
            
            PlayerEvents.PausePressed += HandlePausePress;

            _unsubscribeAction = () => {
                PlayerEvents.PausePressed -= HandlePausePress;
            };
        }

        private void Awake() {
            ToSingleton();
            _isPause = false;
            _uiStack = new Stack<CanvasView>();
        }

        private void Start() {
            _pause.ExitCanvas();
            _offeringSelection.ExitCanvas();
        }

        private void OnDisable() => _unsubscribeAction();
        
        private void ToSingleton() {
            if (Instance != null) {
                Destroy(this);
                return;
            }
            
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }

        public void EnterOfferingSelection() => EnterCanvas(_offeringSelection);

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

        private void EnterCanvas(CanvasView canvas) {
            if (!_isPause) EnterPauseState();
            
            if (_uiStack.Count > 0) _uiStack.Peek().ExitCanvas();
            _uiStack.Push(canvas);
            canvas.EnterCanvas();
        }

        public void ExitLastCanvas() {
            if (_uiStack.Count > 0) _uiStack.Pop().ExitCanvas();

            if (_uiStack.Count == 0) ExitPauseState();
            else _uiStack.Peek().EnterCanvas();
        }
    }
}