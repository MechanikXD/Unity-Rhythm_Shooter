using System;
using UnityEngine;

namespace Core.Level.Room {
    /// <summary>
    /// Class that stores only size of some BoxCollider, or just box area
    /// </summary>
    [Serializable] public class BoxColliderInfo {
        [SerializeField] private Vector3 _center;
        [SerializeField] private Vector3 _size;

        public Vector3 Center => _center;
        public Vector3 Size => _size;
    }
}