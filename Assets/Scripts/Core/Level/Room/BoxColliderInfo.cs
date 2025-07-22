using System;
using UnityEngine;

namespace Core.Level.Room {
    [Serializable]
    public class BoxColliderInfo {
        [SerializeField] private Vector3 center;
        [SerializeField] private Vector3 size;

        public Vector3 Center => center;
        public Vector3 Size => size;
    }
}