using Core.Music;
using Player.PlayerWeapons.Abstract;

namespace Player.PlayerWeapons {
    public class DirectionalTestWeapon : WeaponBase {
        public override void WeaponInit() {
            ConductorEvents.BeatMissedEvent += () => Conductor.Instance.DisableNextInteractions(2);
        }

        public override void LeftAction() {
            var currentSongPosition = Conductor.Instance.SongPosition;
            Conductor.Instance.DetermineHitQuality(currentSongPosition);
        }

        public override void RightAction() {
            var currentSongPosition = Conductor.Instance.SongPosition;
            Conductor.Instance.DetermineHitQuality(currentSongPosition);
        }
    }
}