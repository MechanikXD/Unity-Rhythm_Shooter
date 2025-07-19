using System.Collections.Generic;

namespace Player.Statistics.Score.Rank {
    public class RankManager {
        private readonly List<RankInfo> _ranks;
        private int _currentRankIndex;

        public RankInfo CurrentRank => _ranks[_currentRankIndex];
        
        public RankInfo DRank => _ranks[0];
        public RankInfo CRank => _ranks[1];
        public RankInfo BRank => _ranks[2];
        public RankInfo ARank => _ranks[3];
        public RankInfo SRank => _ranks[4];

        public RankManager() {
            var sRank = new RankInfo(RankLetter.S, 2f, -0.5f, -0.3f, 0.3f, 0.4f);
            var aRank = new RankInfo(RankLetter.A, 1.5f, -0.4f, -0.25f, 0.2f, 0.35f);
            var bRank = new RankInfo(RankLetter.B, 1f, -0.25f, -0.15f, 0.15f, 0.25f);
            var cRank = new RankInfo(RankLetter.C, 0.8f, -0.15f, -0.1f, 0.15f, 0.2f);
            var dRank = new RankInfo(RankLetter.D, 0.6f, -0.1f, -0.1f, 0.2f, 0.3f);

            _ranks = new() { dRank, cRank, bRank, aRank, sRank };
            _currentRankIndex = 0;
        }

        public bool CanPromote() => _currentRankIndex < _ranks.Count - 1;
        public bool CanDemote() => _currentRankIndex > 0;

        public void PromoteRank() => _currentRankIndex += 1;
        public void DemoteRank() => _currentRankIndex -= 1;
    }
}