using System;
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
            var sRank = new RankInfo(RankLetter.S, 2f, -0.5f, -0.4f, 0.03f, 0.05f);
            var aRank = new RankInfo(RankLetter.A, 1.5f, -0.4f, -0.34f, 0.04f, 0.07f);
            var bRank = new RankInfo(RankLetter.B, 1f, -0.25f, -0.25f, 0.1f, 0.15f);
            var cRank = new RankInfo(RankLetter.C, 0.8f, -0.15f, -0.17f, 0.13f, 0.17f);
            var dRank = new RankInfo(RankLetter.D, 0.6f, -0.1f, -0.1f, 0.2f, 0.3f);

            _ranks = new() { dRank, cRank, bRank, aRank, sRank };
            _currentRankIndex = 0;
        }

        public bool CanPromote() => _currentRankIndex < _ranks.Count - 1;
        public bool CanDemote() => _currentRankIndex > 0;

        public void PromoteRank() {
            if (CanPromote()) throw new ArgumentOutOfRangeException(nameof(_currentRankIndex),
                    "No rank promotion is allowed past S rank");
            _currentRankIndex += 1;
        }

        public void DemoteRank() {
            if (CanDemote()) throw new ArgumentOutOfRangeException(nameof(_currentRankIndex),
                    "No rank Demotion is allowed past D rank");
            _currentRankIndex -= 1;
        }
    }
}