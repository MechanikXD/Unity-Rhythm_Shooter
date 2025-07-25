using System;
using UnityEngine;

namespace Core.Level.Room.Enemy {
    [Serializable]
    public class Wave<T> {
        [SerializeField] private T[] _array;

        public int Count => _array.Length;
        public T this[int index] => _array[index];
    }
}