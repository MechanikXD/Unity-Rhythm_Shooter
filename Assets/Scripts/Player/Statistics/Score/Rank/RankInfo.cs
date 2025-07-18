using System;

namespace Player.Statistics.Score.Rank {
    [Serializable]
    public class RankInfo { 
        public RankLetter Letter { get; private set; }
        public float ScoreMultiplayer { get; private set; }
        
        public float PassiveDecrement { get; private set; }
        public float MissDecrement { get; private set; }
        public float GoodIncrement { get; private set; }
        public float PerfectIncrement { get; private set; }

        public RankInfo(RankLetter letter, float multiplayer, float passiveDecr, float missDecr,
            float goodIncr, float perfectIncr) {
            Letter = letter;
            ScoreMultiplayer = multiplayer;
            PassiveDecrement = passiveDecr;
            MissDecrement = missDecr;
            GoodIncrement = goodIncr;
            PerfectIncrement = perfectIncr;
        }
    }
}