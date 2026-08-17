using CHG.Scripts.Agents;
using CHG.Scripts.Agents.StatSystem;
using CHG.Scripts.CombatSystem;
using CHG.Scripts.CoreSystem.AnimationSystem;
using DevLib.ModuleSystem;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG.Scripts.WeaponSystem
{
    public class ProjectileDelivery : WeaponDelivery
    {
        
        [Header("Fire")]
        [SerializeField] private Transform muzzle;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO bulletItem;
        
        [Header("Anim")]
        [SerializeField] private AnimParamSO attackParam;
        [SerializeField] private AnimParamSO upperIdleParam;
        [SerializeField] private int upperBodyLayer = 1;
        [SerializeField] private float layerBlendTime = 0.15f;

        private IAimModule _aim;
        private IStatModule _stat;
        private IRenderer _renderer;
        private IWeaponTargeting _targeting;
        private float _targetLayerWeight;

        public override void InitPart(Weapon weapon, ModuleOwner owner)
        {
            base.InitPart(weapon, owner);
            
            _aim = owner.GetModule<IAimModule>();
            _stat = owner.GetModule<IStatModule>();
            _renderer = owner.GetModule<IRenderer>();
            _targeting = weapon.GetPart<IWeaponTargeting>();  
            
            Debug.Assert(_aim != null, $"Aim is null : {gameObject.name}");
            Debug.Assert(_stat != null, $"Stat is null : {gameObject.name}");
            Debug.Assert(_renderer != null, $"Renderer is null : {gameObject.name}");
            
            _renderer.Animator.SetLayerWeight(upperBodyLayer, 0f);
        }

        public override void OnFireStart()
        {
            _targetLayerWeight = 1f;
            _renderer.PlayClip(upperIdleParam.ParamHash, 0f, 0.05f, upperBodyLayer);
        }

        public override void OnFireStop() => _targetLayerWeight = 0f;

        public override void Execute(float damageMultiplier)
        {
            Vector3 target = _aim.AimPoint;
            if ((target - muzzle.position).sqrMagnitude < 1f)
                target = muzzle.position + _aim.AimForward;
            
            Vector3 dir = (target - muzzle.position).normalized;
            
            Bullet bullet = poolManager.Pop<Bullet>(bulletItem);
            DamageData damageData =
                _stat.CalculateDamage(_weapon.Data.ToDamageSource(damageMultiplier));
            bullet.Launch(muzzle.position, dir, damageData);
            
            Transform homingTarget = _targeting?.FindTarget(muzzle.position);
            //Debug.Log();
            bullet.Launch(muzzle.position, dir, damageData, homingTarget);
            
            _renderer.PlayClip(attackParam.ParamHash, 0f, 0.02f, upperBodyLayer);
        }

        public override void UpdatePart()
        {
            Animator animator = _renderer.Animator;
            float current = animator.GetLayerWeight(upperBodyLayer);

            if (Mathf.Approximately(current, _targetLayerWeight)) return;

            float step = Time.deltaTime / Mathf.Max(layerBlendTime, 0.01f);
            animator.SetLayerWeight(upperBodyLayer, Mathf.MoveTowards(current, _targetLayerWeight, step));
        }
    }
}