using System;
using CHG.Scripts.Agents.StatSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.CombatSystem
{
    public class HealthModule : MonoBehaviour, IModule, IAfterInitModule
    {
        private const float VitalScale = 5f;
        private const float VitalSoftCap = 0.01f;

        public event Action<int, int> OnHealthChanged; // current, max
        public event Action OnDeath;

        [SerializeField] private StatSO healthStat;
        [SerializeField] private int baseMaxHealth;
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;

        private ModuleOwner _owner;
        private IStatModule _statModule;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDead => currentHealth <= 0;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            currentHealth = maxHealth;
            _statModule = owner.GetModule<IStatModule>();
            
            Debug.Assert(_statModule != null, $"statModule is null : {gameObject.name}");
        }

        public void AfterInit()
        {
            if (_statModule == null) return;

            float vital = _statModule.SubscribeStat(healthStat.AssetIndex, HandleVitalChange, healthStat.BaseValue);    
            
            currentHealth = maxHealth = Mathf.Max(1, CalculateMaxHealth(vital));
            
            RaiseHealthChange();
        }

        private void OnDestroy()
        {
            _statModule?.UnSubscribeStat(healthStat.AssetIndex, HandleVitalChange);
        }

        private void HandleVitalChange(StatSO stat, float currentValue, float prevValue)
        {
            int prevMaxHealth = maxHealth;
            maxHealth = CalculateMaxHealth(currentValue);

            if (!IsDead)
            {
                int delta = maxHealth - prevMaxHealth;
                currentHealth = Mathf.Clamp(currentHealth + delta, 1, maxHealth);
            }

            RaiseHealthChange();
        }

        private int CalculateMaxHealth(float vital)
            => baseMaxHealth + Mathf.RoundToInt(VitalScale * vital / (1f + VitalSoftCap * vital));

        public void ApplyDamage(int damageAmount)
        {
            if (IsDead) return;

            currentHealth = Mathf.Max(0, currentHealth - damageAmount);

            RaiseHealthChange();

            if (IsDead)
                OnDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0) return;
            
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            RaiseHealthChange();
        }
        
        private void RaiseHealthChange() => OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}