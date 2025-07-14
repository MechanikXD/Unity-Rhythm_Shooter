using System;
using UnityEngine;

namespace Core.Music {
    // TODO: Make this Scriptable object?
    [Serializable]
    public class SongData {
        [SerializeField] private AudioClip song;
        [SerializeField] private float songBpm;
        [SerializeField] private float startOffset;
        private float _crotchet;
        private float _halfCrotchet;

        private int _beatsPerLoop;

        public float HalfCrotchet {
            get {
                if (_halfCrotchet != 0) return _halfCrotchet;

                _halfCrotchet = _crotchet / 2;
                return _halfCrotchet;
            }
        }

        public float Crotchet {
            get {
                if (_crotchet != 0) return _crotchet;

                _crotchet = 60f / songBpm;
                return _crotchet;
            }
        }

        public int BeatsPerLoop {
            get {
                if (_beatsPerLoop != 0) return _beatsPerLoop;
                
                _beatsPerLoop = (int)((song.length - startOffset) / _crotchet);
                return _beatsPerLoop;
            }
        } 
        
        public AudioClip Audio => song;
        public float Bpm => songBpm;
        public float Offset => startOffset;
    }
}