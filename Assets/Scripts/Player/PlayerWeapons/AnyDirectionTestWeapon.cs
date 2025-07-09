using Core.Music;
using Player.PlayerWeapons.Abstract;
using UnityEngine;

namespace Player.PlayerWeapons {
    public class AnyDirectionTestWeapon : WeaponBase {
        private bool _leftActionBuffered;
        private bool _rightActionBuffered;
        private float _currentBufferTime;
        private bool _doBufferCounting;
        private const float MaxInputBufferTime = 0.1f;

        public override void LeftAction() {
            var currentSongPosition = Conductor.Instance.SongPosition;
            if (_rightActionBuffered) {
                PlayerEvents.OnBothActions(currentSongPosition);
                Conductor.Instance.DetermineHitQuality(currentSongPosition);
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

        public override void RightAction() {
            var currentSongPosition = Conductor.Instance.SongPosition;
            if (_leftActionBuffered) {
                PlayerEvents.OnBothActions(currentSongPosition);
                Conductor.Instance.DetermineHitQuality(currentSongPosition);
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

        public override void WeaponUpdate() {
            if (!_doBufferCounting) return;

            _currentBufferTime += Time.deltaTime;
            if (_currentBufferTime <= MaxInputBufferTime) return;
                
            // ReSharper disable Unity.PerformanceCriticalCodeInvocation
            var songPositionWhenHit = Conductor.Instance.SongPosition - _currentBufferTime;
            if (_leftActionBuffered) {
                PlayerEvents.OnLeftAction(songPositionWhenHit);
                Conductor.Instance.DetermineHitQuality(songPositionWhenHit);
            }

            if (_rightActionBuffered) {
                PlayerEvents.OnRightAction(songPositionWhenHit);
                Conductor.Instance.DetermineHitQuality(songPositionWhenHit);
            }
            // ReSharper restore Unity.PerformanceCriticalCodeInvocation
                
            _doBufferCounting = false;
            _currentBufferTime = 0f;
            _leftActionBuffered = false;
            _rightActionBuffered = false;
        }
    }
}