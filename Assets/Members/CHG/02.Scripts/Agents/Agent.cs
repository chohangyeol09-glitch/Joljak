using System;
using Members.CHG._02.Scripts.CombatSystem;
using Members.CHG._02.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Members.CHG._02.Scripts.Agents
{
    public abstract class Agent : ModuleOwner
    {
        public bool IsDead { get; protected set; }
        public UnityEvent OnHit;
        public UnityEvent OnDeath;
        
        public HealthModule HealthModule { get; private set; }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            HealthModule = GetModule<HealthModule>();
        }

        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();
            HealthModule.OnDeath += HandleDeath;
            OnHit.AddListener(HandleHit);
        }

        

        protected virtual void OnDestroy()
        {
            HealthModule.OnDeath -= HandleDeath;
        }

        protected virtual void HandleDeath()
        {
            IsDead = true;
            OnDeath.Invoke();
        }

        protected abstract void HandleHit();

        public void ApplyDamage(DamageData damageData)
        {
            if (IsDead) return;

            if (HealthModule != null)
            {
                HealthModule.ApplyDamage(damageData.DamageAmount);
            }
            
            OnHit?.Invoke();
        }
        
    }
}