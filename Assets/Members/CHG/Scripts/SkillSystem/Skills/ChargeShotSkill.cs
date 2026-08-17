using CHG.Scripts.Agents;
using CHG.Scripts.Agents.StatSystem;
using CHG.Scripts.CombatSystem;
using CHG.Scripts.SkillSystem.SkillDatas;
using CHG.Scripts.WeaponSystem;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG.Scripts.SkillSystem.Skills
{
    public class ChargeShotSkill : ChargeSkill
    {
        [SerializeField] private PoolManagerSO poolManager;

        private ChargeShotDataSO _data;
        private IAimModule _aim;
        private IStatModule _stat;
        private IRenderer _renderer;
        private IPointModule _point;

        protected override float MaxChargeTime => _data != null ? _data.maxChargeTime : base.MaxChargeTime;
        protected override float GraceTime => _data != null ? _data.graceTime : base.GraceTime;

        public override void InitializeSkill(ISkillModule module, SkillDataSO data)
        {
            base.InitializeSkill(module, data);
            
            _data = Data as ChargeShotDataSO;
            _aim = module.Owner.GetModule<IAimModule>();    
            _stat = module.Owner.GetModule<IStatModule>();
            _renderer = module.Owner.GetModule<IRenderer>();
            _point = module.Owner.GetModule<IPointModule>();
            
            Debug.Assert(_data != null, $"Data is not ChargeShotData : {gameObject.name}");
            Debug.Assert(_aim != null, $"aim is null : {gameObject.name}");
            Debug.Assert(_stat != null, $"stat is null : {gameObject.name}");
            Debug.Assert(_renderer != null, $"renderer is null : {gameObject.name}");
            Debug.Assert(_point != null, $"point is null : {gameObject.name}");
        }

        protected override void OnChargeStart(GameObject target)
        {
            _aim.RequestAim(this);
            
            if (_data.ChargeAnim != null)
                _renderer?.PlayClip(_data.ChargeAnim.ParamHash, 0f, 0.05f);
        }

        protected override void Fire(GameObject target, float chargeRatio)
        {
            _aim.ReleaseAim(this);

            Transform point = _point.GetPoint(_data.PointType);
            Vector3 origin = point.position;
            Vector3 toTarget = _aim.AimPoint - origin;
            Vector3 dir = toTarget.sqrMagnitude < 1f ? _aim.AimForward : toTarget.normalized;
            
            float multiplier = _data.DamageMultiplier * Mathf.Lerp(_data.minChargeMultiplier, _data.maxChargeMultiplier, chargeRatio);
            DamageData damage = _stat.CalculateDamage(new DamageSource(_data.DamageType, _data.BaseDamage, multiplier));

            Bullet bullet = poolManager.Pop<Bullet>(_data.BulletItem);
            if (bullet == null)
            {
                Debug.LogError($"{_data.BulletItem?.name} is not found pool");
                return;
            }
            bullet.Launch(origin, dir, damage);
            
            if (_data.FireAnim != null) 
                _renderer?.PlayClip(_data.FireAnim.ParamHash, 0f, 0.02f);

        }
        
        protected override void OnChargeCancel() => _aim.ReleaseAim(this);
    }
}