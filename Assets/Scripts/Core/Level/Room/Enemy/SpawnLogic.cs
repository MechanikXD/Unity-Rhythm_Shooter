namespace Core.Level.Room.Enemy {
    public enum SpawnLogic {
        Waves, // Pure wave based
        Random, // Random amount of enemies
        RandomWaves, // Several Waves of random enemies
        Bounty, // One or several targets to defeat
        BountyWithOdds  // One or several targets to defeat with junk enemies  
    }
}