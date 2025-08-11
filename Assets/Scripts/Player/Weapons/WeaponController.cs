using System;
using Core.Music;
using Player.Weapons.Base;
using UnityEngine;

namespace Player.Weapons {
    [Serializable]
    public class WeaponController {
        private WeaponBase _currentWeapon;
        [SerializeField] private float _maxInputBufferTime = 0.1f;
        private bool _leftActionBuffered;
        private bool _rightActionBuffered;
        private float _currentBufferTime;
        private bool _doBufferCounting;

        public void Initialize(WeaponBase startWeapon) {
            _currentWeapon = startWeapon;
            _currentWeapon.OnWeaponSelected();
        }

        public void ChangeWeapon(WeaponBase newWeapon) {
            _currentWeapon.OnWeaponDeselected();
            _currentWeapon = newWeapon;
            _currentWeapon.OnWeaponSelected();
        }

        public void WeaponUpdate() {
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
        
        public void LeftAction() {
            if (_rightActionBuffered && _currentWeapon.CanDoDoubleAction) {
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

        public void RightAction() {
            if (_leftActionBuffered && _currentWeapon.CanDoDoubleAction) {
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

        public void Reload() {
            if (_currentWeapon.IsReloading && _currentWeapon.CanFastReload) {
                var beatHitInfo = Conductor.Instance.GetBeatHitInfo();
                
                if (beatHitInfo.HitType is BeatHitType.Perfect or BeatHitType.Good) {
                    _currentWeapon.FastReload();
                }
                else {
                    _currentWeapon.SlowReload();
                }
            }
            else if (!_currentWeapon.IsReloading) {
                _currentWeapon.StartReload();
            }
            Conductor.Instance.SetInteractedThisBeat();
        }
    }
}