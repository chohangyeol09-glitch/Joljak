using CHG.Scripts.Agents;
using CHG.Scripts.CombatSystem;
using CHG.Scripts.SkillSystem.SkillDatas;
using UnityEngine;

namespace CHG.Scripts.SkillSystem.Skills
{
    public class HealSkill : InstantSkill
    {
        private HealSkillDataSO _data;
        private HealthModule _health;
        private IRenderer _renderer;

        public override void InitializeSkill(ISkillModule module, SkillDataSO data)
        {
            base.InitializeSkill(module, data);
            
            _data = Data as HealSkillDataSO;
            _health = module.Owner.GetModule<HealthModule>();
            _renderer = module.Owner.GetModule<IRenderer>();
            
            Debug.Assert(_data != null, $"Data is not HealSkillData : {gameObject.name}");
            Debug.Assert(_health != null, $"health is null : {gameObject.name}");

            if (_health != null)
                _health.OnHealthChanged += HandleHealthChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_health != null)
                _health.OnHealthChanged -= HandleHealthChanged;
        }

        private void HandleHealthChanged(int current, int max)
        {
            RaiseUsableChanged();
        }

        public override bool CanUseSkill(GameObject target = null)
        {
            if (!base.CanUseSkill(target)) return false;

            return _health != null && !_health.IsDead && _health.CurrentHealth < _health.MaxHealth;
        }

        protected override void Cast(GameObject target)
        {
            int amount = _data.HealAmount + Mathf.RoundToInt(_health.MaxHealth * _data.HealPercent);
            _health.Heal(amount);
            
            if (_data.CastAnim != null)
                _renderer?.PlayClip(_data.CastAnim.ParamHash, 0f,0.05f);
            
            EndSkill();
        }
    }
}