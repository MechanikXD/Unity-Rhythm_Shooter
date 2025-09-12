using System;
using System.Collections.Generic;
using Core.Behaviour.BehaviourInjection;
using Core.Game.Audio;
using Interactable.Status;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactable.Damageable {
    public abstract class DamageableBehaviour : MonoBehaviour, IDamageable {
        [Header("Damageable Base:")]
        // ---------- General ----------
        [SerializeField] protected Rigidbody _body;
        public Vector3 Position => transform.position;
        
        // ---------- Movement ----------
        [SerializeField] protected float _moveSpeed;
        public float MoveSpeedMultiplier { get; private set; } = 1f;
        public float CurrentSpeed { get; private set; }
        
        // ---------- Health & Damage Red. ----------
        [SerializeField] protected int _maxHealth;
        private int _currentMaxHealth;
        public float HealthMultiplier { get; private set; } = 1f;
        public int HealthIncrement { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth => _currentMaxHealth;
        public BehaviourInjection<DamageInfo, int> DamageProcessor { get; private set; }

        public event Action<int> MaxHealthChanged;
        public event Action<int> CurrentHealthChanged; 

        [SerializeField] protected float _defaultDamageReduction;
        public float CurrentDamageReduction { get; private set; }
        private bool _canTakeDamage;
        
        // [SerializeField] protected int _staggerThreshold;
        // private int _currentsStagger;
        
        // ---------- Damage & Statuses ----------
        [SerializeField] protected int _damage;
        public int CurrentDamage { get; private set; }
        public float DamageMultiplier { get; private set; } = 1f;
        public int DamageIncrement { get; private set; }
        public Dictionary<StatusEffect, StatusBase> CurrentStatuses { get; private set; }

        // ---------- Sounds ----------
        protected const float DefaultSoundReach = 10f;
        [SerializeField] protected Vector2 _audioPitchChange;
        
        [SerializeField] protected AudioClip[] _stepSounds;
        [SerializeField] protected AudioClip[] _hurtSounds;
        
        protected virtual void Awake() {
            Initialize();
            UpdateCurrentDamage();
            UpdateMaxHealth();
        }
        
        protected void OnDestroy() {
            ClearStatuses();
        }

        #region Status Manipulations

        public virtual void ApplyStatus(StatusEffect status) {
            if (!HasStatus(status)) {
                var newStatusBase = new StatusBase(this, status);
                CurrentStatuses.Add(status, newStatusBase);
                newStatusBase.ApplyStatus();
            }
            else {
                var thisStatusBase = CurrentStatuses[status];
                
                if (thisStatusBase.IsActive) thisStatusBase.RepeatedApply();
                else thisStatusBase.ApplyStatus();
            }
        }

        public virtual void RemoveStatus(StatusEffect status) {
            if (!HasStatus(status)) return;

            CurrentStatuses[status].RemoveStatus();
        }

        public virtual bool HasStatus(StatusEffect statusBase) {
            return CurrentStatuses.ContainsKey(statusBase);
        }

        public virtual void ClearStatuses() {
            foreach (var status in CurrentStatuses.Values) {
                status.RemoveStatus();
            }
            CurrentStatuses.Clear();
        }

        #endregion

        // WIP; Will be implemented in future
        /*
        public virtual void Parried(int value) {
            _currentsStagger -= value;
            if (_currentsStagger <= 0) {
                _currentsStagger = 0;
                // EnterParriedState();
            }
        }

        protected abstract void EnterParriedState();
        */

        private int DefaultDamageProcessor(DamageInfo damageInfo) {
            return (int)(damageInfo.DamageValue - damageInfo.DamageValue * CurrentDamageReduction);
        }

        public virtual void TakeDamage(DamageInfo damageInfo) {
            if (!_canTakeDamage) return;

            CurrentHealth = Mathf.Clamp(CurrentHealth - DamageProcessor.Perform(damageInfo), 0,
                _currentMaxHealth);
            CurrentHealthChanged?.Invoke(CurrentHealth);
            
            if (CurrentHealth <= 0) {
                CurrentHealth = 0;
                _canTakeDamage = false;
                Die();
            }
            else if (damageInfo.Force > 0) {
                // TODO: Test that, may be in wrong order
                _body.AddForce((damageInfo.HitPosition - damageInfo.SourcePosition).normalized *
                              damageInfo.Force);
            }
        }

        public void SetDamageAbility(bool canBeDamaged) => _canTakeDamage = canBeDamaged;

        public void SetDamageReduction(float value) => CurrentDamageReduction = value;

        #region Health Manipulations

        public void SetMaxHealth(int newValue, bool adjustCurrentHealth = true) {
            if (newValue <= 0) return;

            var oldMaxHealth = _currentMaxHealth;
            _currentMaxHealth = newValue;
            MaxHealthChanged?.Invoke(_currentMaxHealth);
            
            if (CurrentHealth > _currentMaxHealth) {
                CurrentHealth = _currentMaxHealth;
                CurrentHealthChanged?.Invoke(CurrentHealth);
            }
            else if (adjustCurrentHealth) {
                var relativeHealth = oldMaxHealth != 0 ? CurrentHealth / oldMaxHealth : 1;
                CurrentHealth = _currentMaxHealth * relativeHealth;
                CurrentHealthChanged?.Invoke(CurrentHealth);
            }
        }
        
        public void SetHealthMultiplier(float newValue) {
            if (newValue < 0) return;
            HealthMultiplier = newValue;
            UpdateMaxHealth();
        }
        
        public void SetHealthIncrement(int newValue) {
            if (newValue < 0) return;
            HealthIncrement = newValue;
            UpdateMaxHealth();
        }
        
        public void UpdateMaxHealth() {
            var newValue = (int)((_maxHealth + HealthIncrement) * HealthMultiplier);
            if (newValue <= 0) newValue = 1;
            SetMaxHealth(newValue);
        }

        #endregion

        #region Move Speed Manipulations

        public void SetMoveSpeed(float newValue) {
            if (newValue < 0) newValue = 0;
            
            CurrentSpeed = newValue * MoveSpeedMultiplier;
        }
        
        public void SetMoveSpeedMultiplier(float newValue) {
            if (newValue < 0) newValue = 0;
            
            MoveSpeedMultiplier = newValue;
            UpdateCurrentSpeed();
        }

        public void UpdateCurrentSpeed() {
            CurrentSpeed = _moveSpeed * MoveSpeedMultiplier;
        }

        #endregion

        #region Damage Manipulations

        public void SetDamageMultiplier(float newValue) {
            if (newValue < 0) return;
            DamageMultiplier = newValue;
            UpdateCurrentDamage();
        }
        
        public void SetDamageIncrement(int newValue) {
            if (newValue < 0) return;
            DamageIncrement = newValue;
            UpdateCurrentDamage();
        }

        public void UpdateCurrentDamage() {
            CurrentDamage = (int)((_damage + DamageIncrement) * DamageMultiplier);
            if (CurrentDamage <= 0) CurrentDamage = 1;
        }

        #endregion

        public void PlayRandomSound(AudioClip[] sounds) =>
            PlayRandomSound(sounds, DefaultSoundReach);
        
        public void PlayRandomSound(AudioClip[] sounds, float reach) {
            var randomSound = sounds[Random.Range(0, sounds.Length)];
            var randomPitch = Random.Range(_audioPitchChange.x, _audioPitchChange.y);
            AudioManager.Instance.PlaySound(randomSound, Position, reach, randomPitch);
        }

        public abstract void Die();
        
        private void Initialize() {
            CurrentStatuses = new Dictionary<StatusEffect, StatusBase>();

            // _currentsStagger = _staggerThreshold;
            DamageProcessor = new BehaviourInjection<DamageInfo, int>(DefaultDamageProcessor);
            CurrentHealth = _maxHealth;
            CurrentDamage = _damage;
            CurrentDamageReduction = _defaultDamageReduction;
            CurrentSpeed = _moveSpeed;
            _canTakeDamage = true;
        }
    }
}