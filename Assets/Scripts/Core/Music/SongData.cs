using System;
using UnityEngine;

namespace Core.Music {
    [Serializable]
    public class SongData {
        [SerializeField] private AudioClip song;
        [SerializeField] private float songBpm;
        [SerializeField] private float startOffset;
        private float _crotchet;
        
        public AudioClip Audio => song;
        public float Bpm => songBpm;
        public float Crotchet {
            get => _crotchet;
            internal set => _crotchet = value;
        }
        public float Offset => startOffset;
    }
}