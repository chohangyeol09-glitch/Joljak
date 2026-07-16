using System;
using System.Reflection;
using Members.CHG._02.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace Members.CHG._02.Scripts.CombatSystem
{
    public class HealthModule : MonoBehaviour, IModule
    {
        public event Action OnDeath;
        
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;

        private ModuleOwner _owner;
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
        }

        public void ApplyDamage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                OnDeath?.Invoke();
            }
        }
    }
}