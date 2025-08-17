using UnityEngine;

namespace Interactable {
    public class DamageInfo {
        public IDamageable Source { get; private set; }
        public IDamageable[] Targets { get; private set; }
        
        public Vector3 SourcePosition { get; private set; }
        public Vector3 HitPosition { get; private set; }
        
        public int DamageValue { get; private set; }
        public float Force { get; private set; }

        public DamageInfo(IDamageable source, IDamageable[] targets, int value, Vector3 sourcePosition,
            Vector3 hitPosition, float force=0f) {
            Source = source;
            Targets = targets;
            DamageValue = value;
            SourcePosition = sourcePosition;
            HitPosition = hitPosition;
            Force = force;
        }
        
        public DamageInfo(IDamageable source, IDamageable target, int value, Vector3 sourcePosition,
            Vector3 hitPosition, float force=0f) {
            Source = source;
            Targets = new []{ target };
            DamageValue = value;
            SourcePosition = sourcePosition;
            HitPosition = hitPosition;
            Force = force;
        }
    }
}