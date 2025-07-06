using System;
using UnityEngine;

namespace Core.Music {
    [Serializable]
    public class SongData {
        [SerializeField] private AudioClip song;
        [SerializeField] private float songBpm;
        [SerializeField] private float startOffset;
        private float _crotchet;

        private int _beatsPerLoop;

        public int BeatsPerLoop {
            get {
                if (_beatsPerLoop != 0) return _beatsPerLoop;
                
                _beatsPerLoop = (int)((song.length - startOffset) / _crotchet);
                return _beatsPerLoop;
            }
        } 
        
        public AudioClip Audio => song;
        public float Bpm => songBpm;
        public float Crotchet {
            get => _crotchet;
            internal set => _crotchet = value;
        }
        public float Offset => startOffset;
    }
}