using System;
using Core.Music;
using Player.PlayerWeapons.Base;

namespace Player {
    public static class PlayerActionEvents {
        public static void OnPlayerLeftAction(float songPosition, WeaponBase currentWeapon) {
            OnLeftAttempted(songPosition);
            var beatType = Conductor.Instance.DetermineHitQuality(songPosition);
            
            if (beatType == BeatHitType.Disabled) return;
            Conductor.Instance.SetInteractedThisBeat();

            switch (beatType) {
                case BeatHitType.Perfect:
                    currentWeapon.LeftPerfectAction();
                    OnPerfectPerformed();
                    OnLeftPerfectPerformed();
                    break;
                case BeatHitType.Good:
                    currentWeapon.LeftGoodAction();
                    OnGoodPerformed();
                    OnLeftGoodPerformed();
                    break;
                case BeatHitType.Miss:
                    currentWeapon.LeftMissedAction();
                    OnMissPerformed();
                    OnLeftMissPerformed();
                    break;
            }
        }
        
        public static void OnPlayerRightAction(float songPosition, WeaponBase currentWeapon) {
            OnRightAttempted(songPosition);
            var beatType = Conductor.Instance.DetermineHitQuality(songPosition);
            
            if (beatType == BeatHitType.Disabled) return;
            Conductor.Instance.SetInteractedThisBeat();

            switch (beatType) {
                case BeatHitType.Perfect:
                    currentWeapon.RightPerfectAction();
                    OnPerfectPerformed();
                    OnRightPerfectPerformed();
                    break;
                case BeatHitType.Good:
                    currentWeapon.RightGoodAction();
                    OnGoodPerformed();
                    OnRightGoodPerformed();
                    break;
                case BeatHitType.Miss:
                    currentWeapon.RightMissedAction();
                    OnMissPerformed();
                    OnRightMissPerformed();
                    break;
            }
        }
        
        public static void OnPlayerBothAction(float songPosition, WeaponBase currentWeapon) {
            OnBothAttempted(songPosition);
            var beatType = Conductor.Instance.DetermineHitQuality(songPosition);
            
            if (beatType == BeatHitType.Disabled) return;
            Conductor.Instance.SetInteractedThisBeat();

            switch (beatType) {
                case BeatHitType.Perfect:
                    currentWeapon.BothPerfectAction();
                    OnPerfectPerformed();
                    OnBothPerfectPerformed();
                    break;
                case BeatHitType.Good:
                    currentWeapon.BothGoodAction();
                    OnGoodPerformed();
                    OnBothGoodPerformed();
                    break;
                case BeatHitType.Miss:
                    currentWeapon.BothMissedAction();
                    OnMissPerformed();
                    OnBothMissPerformed();
                    break;
            }
        }

        public static void OnPlayerDashed(BeatHitType hitType) {
            switch (hitType) {
                case BeatHitType.Perfect:
                    OnPerfectPerformed();
                    break;
                case BeatHitType.Good:
                    OnGoodPerformed();
                    break;
                case BeatHitType.Miss:
                    OnMissPerformed();
                    break;
                case BeatHitType.Disabled:
                default:
                    throw new ArgumentOutOfRangeException(nameof(hitType), hitType,
                        "OnPlayerDashed was called when actions were disabled");
            }
        }

        #region Attemped Actions

        public static event Action<float> LeftAttempted;
        public static event Action<float> RightAttempted;
        public static event Action<float> BothAttempted;
        
        private static void OnLeftAttempted(float actionTimeInSongPosition) =>
            LeftAttempted?.Invoke(actionTimeInSongPosition);
        private static void OnRightAttempted(float actionTimeInSongPosition) =>
            RightAttempted?.Invoke(actionTimeInSongPosition);
        private static void OnBothAttempted(float actionTimeInSongPosition) =>
            BothAttempted?.Invoke(actionTimeInSongPosition);

        #endregion

        #region General Actions By Type

        public static event Action PerfectPerformed;
        public static event Action GoodPerformed;
        public static event Action MissPerformed;
        
        private static void OnPerfectPerformed() => PerfectPerformed?.Invoke();
        private static void OnGoodPerformed() => GoodPerformed?.Invoke();
        private static void OnMissPerformed() => MissPerformed?.Invoke();

        #endregion

        #region Left Action

        public static event Action LeftPerfectPerformed;
        public static event Action LeftGoodPerformed;
        public static event Action LeftMissPerformed;

        private static void OnLeftPerfectPerformed() => LeftPerfectPerformed?.Invoke();
        private static void OnLeftGoodPerformed() => LeftGoodPerformed?.Invoke();
        private static void OnLeftMissPerformed() => LeftMissPerformed?.Invoke();

        #endregion

        #region Right Action

        public static event Action RightPerfectPerformed;
        public static event Action RightGoodPerformed;
        public static event Action RightMissPerformed;
        
        private static void OnRightPerfectPerformed() => RightPerfectPerformed?.Invoke();
        private static void OnRightGoodPerformed() => RightGoodPerformed?.Invoke();
        private static void OnRightMissPerformed() => RightMissPerformed?.Invoke();

        #endregion

        #region Both Actions

        public static event Action BothPerfectPerformed;
        public static event Action BothGoodPerformed;
        public static event Action BothMissPerformed;

        private static void OnBothPerfectPerformed() => BothPerfectPerformed?.Invoke();
        private static void OnBothGoodPerformed() => BothGoodPerformed?.Invoke();
        private static void OnBothMissPerformed() => BothMissPerformed?.Invoke();

        #endregion
    }
}