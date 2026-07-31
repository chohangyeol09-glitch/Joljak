using CHG.Scripts.CombatSystem;
using CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;
using UnityEngine.Events;

namespace CHG.Scripts.Agents
{
    public abstract class Agent : ModuleOwner, IDamageable
    {
        public UnityEvent OnHit;
        public UnityEvent OnDeath;

        public HealthModule HealthModule { get; private set; }

        public bool IsDead => HealthModule != null && HealthModule.IsDead;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            HealthModule = GetModule<HealthModule>();

            Debug.Assert(HealthModule != null, $"HealthModule is null : {gameObject.name}");
        }

        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();
            HealthModule.OnDeath += HandleDeath;
        }

        protected virtual void OnDestroy()
        {
            HealthModule.OnDeath -= HandleDeath;
        }

        protected virtual void HandleDeath()
        {
            OnDeath.Invoke();
        }

        protected abstract void HandleHit();

        public void ApplyDamage(DamageData damageData)
        {
            if (IsDead) return;

            HealthModule.ApplyDamage(damageData.DamageAmount);

            HandleHit();
            OnHit.Invoke();
        }
    }
}