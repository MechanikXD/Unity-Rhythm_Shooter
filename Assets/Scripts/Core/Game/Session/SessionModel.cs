namespace Core.Game.Session {
    public static class SessionModel {
        public class ValueModifier {
            public float FlatModifier { get; set; }
            public float PercentModifier { get; set; } = 1f;

            public float GetModifiedValue(float baseValue) {
                return (baseValue + FlatModifier) * PercentModifier;
            }
        }
        
        public static ValueModifier PlayerHealthModifier;
        public static ValueModifier PlayerDamageModifier;
        public static ValueModifier PlayerDamageReduction;
        
        public static ValueModifier EnemyHealthModifier;
        public static ValueModifier EnemyDamageModifier;
        public static ValueModifier EnemyDamageReduction;
    }
}