using System.Collections.Generic;
using Interactable.Status;
using UnityEngine;

namespace Interactable.Damageable {
    [RequireComponent(typeof(Rigidbody))]
    public abstract class DamageableBehaviour : MonoBehaviour, IDamageable {
        [SerializeField] protected Rigidbody _body;
        public Vector3 Position => transform.position;
        [SerializeField] protected float _moveSpeed;
        
        [SerializeField] protected int _maxHealth;
        private int _currentMaxHealth;
        public float HealthMultiplier { get; private set; } = 1f;
        public int HealthIncrement { get; private set; }
        public int CurrentHealth { get; protected set; }
        public int MaxHealth => _currentMaxHealth;
        
        [SerializeField] protected float _defaultDamageReduction;
        public float CurrentDamageReduction { get; protected set; }
        private bool _canTakeDamage;
        
        [SerializeField] protected int _damage;
        protected int CurrentDamage;
        public float DamageMultiplier { get; private set; } = 1f;
        public int DamageIncrement { get; private set; }
        
        // TODO: Make actual status class
        public HashSet<StatusBase> CurrentStatuses { get; private set; }
        [SerializeField] protected int _staggerThreshold;
        private int _currentsStagger;

        protected virtual void Awake() {
            CurrentStatuses = new HashSet<StatusBase>();

            _currentsStagger = _staggerThreshold;
            CurrentHealth = _maxHealth;
            CurrentDamage = _damage;
            CurrentDamageReduction = _defaultDamageReduction;
            _canTakeDamage = true;
            
            RecalculateCurrentDamage();
            RecalculateMaxHealth();
        }

        public virtual void ApplyStatus(StatusBase status) {
            if (CurrentStatuses.Add(status)) {
                status.ApplyStatus(this);
            }
            else {
                status.RepeatedApply();
            }
        }

        public virtual void RemoveStatus(StatusBase status) {
            if (!CurrentStatuses.Contains(status)) return;

            status.RemoveStatus();
            CurrentStatuses.Remove(status);
        }

        public virtual void ClearStatuses() {
            foreach (var status in CurrentStatuses) {
                status.RemoveStatus();
            }
            CurrentStatuses.Clear();
        }

        public virtual void Parried(int value) {
            _currentsStagger -= value;
            if (_currentsStagger <= 0) {
                _currentsStagger = 0;
                EnterParriedState();
            }
        }

        protected abstract void EnterParriedState();

        public virtual void TakeDamage(DamageInfo damageInfo) {
            if (!_canTakeDamage) return;

            CurrentHealth -= (int)(damageInfo.DamageValue -
                                   damageInfo.DamageValue * CurrentDamageReduction);
            if (CurrentHealth <= 0) {
                CurrentHealth = 0;
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

        public void SetMaxHealth(int newValue, bool adjustCurrentHealth = true) {
            if (newValue <= 0) return;

            var oldMaxHealth = _currentMaxHealth;
            _currentMaxHealth = newValue;
            
            if (CurrentHealth > _currentMaxHealth) {
                CurrentHealth = _currentMaxHealth;
            }
            else if (adjustCurrentHealth) {
                var relativeHealth = CurrentHealth / oldMaxHealth;
                CurrentHealth = _currentMaxHealth * relativeHealth;
            }
        }

        public void ChangeDamageMultiplier(float newValue) {
            if (newValue < 0) return;
            DamageMultiplier = newValue;
            RecalculateCurrentDamage();
        }
        
        public void ChangeDamageIncrement(int newValue) {
            if (newValue < 0) return;
            DamageIncrement = newValue;
            RecalculateCurrentDamage();
        }

        public void RecalculateCurrentDamage() {
            CurrentDamage = (int)((_damage + DamageIncrement) * DamageMultiplier);
            if (CurrentDamage <= 0) CurrentDamage = 1;
        }
        
        public void ChangeHealthMultiplier(float newValue) {
            if (newValue < 0) return;
            HealthMultiplier = newValue;
            RecalculateMaxHealth();
        }
        
        public void ChangeHealthIncrement(int newValue) {
            if (newValue < 0) return;
            HealthIncrement = newValue;
            RecalculateMaxHealth();
        }
        
        public void RecalculateMaxHealth() {
            var newValue = (int)((_maxHealth + HealthIncrement) * HealthMultiplier);
            if (newValue <= 0) newValue = 1;
            SetMaxHealth(newValue);
        }

        public abstract void Die();
    }
}