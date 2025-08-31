using UnityEngine;

namespace Interactable.Damageable {
    public class DamageInfo {
        public IDamageable Source { get; private set; }
        public IDamageable Target { get; private set; }
        
        public Vector3 SourcePosition { get; private set; }
        public Vector3 HitPosition { get; private set; }
        
        public int DamageValue { get; private set; }
        public float Force { get; private set; }
        
        public DamageInfo(IDamageable source, IDamageable target, int value, Vector3 sourcePosition,
            Vector3 hitPosition, float force=0f) {
            Source = source;
            Target = target;
            DamageValue = value;
            SourcePosition = sourcePosition;
            HitPosition = hitPosition;
            Force = force;
        }
    }
}