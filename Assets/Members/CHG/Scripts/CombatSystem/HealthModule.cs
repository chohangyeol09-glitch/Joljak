using System;
using CHG.Scripts.Agents.StatSystem;
using CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.CombatSystem
{
    public class HealthModule : MonoBehaviour, IModule, IAfterInitModule
    {
        private const float VitalScale = 5f;
        private const float VitalSoftCap = 0.01f;

        public event Action OnDeath;

        [SerializeField] private StatSO healthStat;
        [SerializeField] private int baseMaxHealth;
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;

        private ModuleOwner _owner;
        private IStatModule _statModule;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

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
            currentHealth = maxHealth = CalculateMaxHealth(vital);
        }

        private void OnDestroy()
        {
            _statModule?.UnSubscribeStat(healthStat.AssetIndex, HandleVitalChange);
        }

        private void HandleVitalChange(StatSO stat, float currentValue, float prevValue)
        {
            int prevMaxHealth = maxHealth;
            maxHealth = CalculateMaxHealth(currentValue);

            int delta = maxHealth - prevMaxHealth;
            currentHealth = Mathf.Clamp(currentHealth + delta, 1, maxHealth);
        }

        private int CalculateMaxHealth(float vital)
            => baseMaxHealth + Mathf.RoundToInt(VitalScale * vital / (1f + VitalSoftCap * vital));

        public void ApplyDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDeath?.Invoke();
            }
        }
    }
}