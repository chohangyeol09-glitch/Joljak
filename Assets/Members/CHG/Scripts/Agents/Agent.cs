using Members.CHG.Scripts.CombatSystem;
using Members.CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine.Events;

namespace Members.CHG.Scripts.Agents
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
            OnHit.RemoveListener(HandleHit);
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