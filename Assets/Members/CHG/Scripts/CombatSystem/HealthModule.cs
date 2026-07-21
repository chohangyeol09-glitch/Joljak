using System;
using Members.CHG.Scripts.Agents.StatSystem;
using Members.CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace Members.CHG.Scripts.CombatSystem
{
    public class HealthModule : MonoBehaviour, IModule
    {
        public event Action OnDeath;
        
        [SerializeField] private StatSO healthStat;
        [SerializeField] private float baseMaxHealth;
        [SerializeField] private float maxHealth; //나중에 스탯으로 변경한다.
        [SerializeField] private float currentHealth;

        private ModuleOwner _owner;
        private IStatModule _statModule;        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            currentHealth = maxHealth;
        }

        public void AfterInit()
        {
            if (_statModule != null)
            {
                float vital = _statModule.SubscribeStat(healthStat.AssetIndex, HandleVitalChange, healthStat.BaseValue);

                float k = 0.01f;
                currentHealth = maxHealth = baseMaxHealth + 5 * vital / (1 + k * vital);
            }
        }

        private void OnDestroy()
        {
            _statModule?.UnSubscribeStat(healthStat.AssetIndex, HandleVitalChange);
        }

        private void HandleVitalChange(StatSO stat, float currentValue, float prevValue)
        {
            float k = 0.01f;
            float beforeMaxHealth = maxHealth;
            maxHealth = baseMaxHealth + 5 * currentValue;
            float delta = maxHealth - beforeMaxHealth / (1 + k * currentValue);
            currentHealth = Mathf.Clamp(currentHealth + delta, 1, maxHealth);
        }

        public void ApplyDamage(float damageAmount)
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