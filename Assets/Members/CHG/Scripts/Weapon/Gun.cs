using System;
using CHG.Scripts.Agents;
using CHG.Scripts.CoreSystem.AnimationSystem;
using CHG.Scripts.CoreSystem.ModuleSystem;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG.Scripts.Weapon
{
    public class Gun : MonoBehaviour, IWeapon, IReloadable, IModule
    {
        [Header("Weapon")] 
        [SerializeField] private WeaponData Data;
        
        public bool CanFire => !IsReloading
                               && CurrentAmmo > 0
                               && !_constraint.Has(ConstraintType.Disarmed)
                               && Time.time >= _nextFireTime;
        
        [SerializeField] private int currentAmmo;

        public bool IsReloading { get; private set; }
        public float ReloadProgress { get; private set; }

        public event Action OnReloadStarted;
        public event Action OnReloadEnded;

        [Header("Fire")] 
        [SerializeField] private Transform muzzle;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO bulletItem;

        [Header("Attack Anim")] 
        [SerializeField] private AnimParamSO attackParam;
        [SerializeField] private AnimParamSO upperIdleParam;
        [SerializeField] private int upperBodyLayer = 1;

        private ModuleOwner _owner;
        private IRenderer _renderer;
        private IAimModule _aim;
        private ConstraintModule _constraint;
        private bool _isFiring;
        private float _nextFireTime;

        public WeaponDisplayInfo Display => Data.Display;
        public int Damage => Data.Damage;
        public float FireRate => Data.FireRate;
        public int MaxAmmo => Data.MaxAmmo;
        public int CurrentAmmo => currentAmmo;
        public float ReloadTime => Data.ReloadTime;
        
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _aim = owner.GetModule<IAimModule>();
            _renderer = owner.GetModule<IRenderer>();
            
            currentAmmo = MaxAmmo;   
            _constraint = owner.GetModule<ConstraintModule>();
            
            Debug.Assert(_constraint != null, $"ConstraintModule is null : {gameObject.name}");
            Debug.Assert(_renderer != null, $"Renderer is null : {gameObject.name}");
            Debug.Assert(_aim != null, $"AimModule is null : {gameObject.name}");
        }
        
        public void StartFire()
        {
            if (!CanFire) return;
            _isFiring = true;    
            _renderer.Animator.SetBool("FIRING", _isFiring);
        }

        public void StopFire()
        {
            _isFiring = false;
            _renderer.Animator.SetBool("FIRING", _isFiring);
            _renderer.PlayClip(upperIdleParam.ParamHash, 0f,0.02f, upperBodyLayer);
        }

        private void Update()
        {
            if (!_isFiring || !CanFire) return;

            Fire();
            _nextFireTime = Time.time + 1f / Data.FireRate;
        }

        private void Fire()
        {
            Vector3 target = _aim.AimPoint;
            if ((target - muzzle.position).sqrMagnitude < 1f)
                target = muzzle.position + _aim.AimForward;
            
            Vector3 dir = (target - muzzle.position).normalized;
            
            Bullet bullet = poolManager.Pop<Bullet>(bulletItem);
            bullet.Launch(muzzle.position, dir, Damage, _owner);
            
            currentAmmo--;
            _renderer.PlayClip(attackParam.ParamHash, 0f,0.02f, upperBodyLayer);
        }

        public void Reload()
        {
            //TODO: 재장전 구현
        }

        public void CancelReload()
        {
            //TODO: 재장전 취소 구현
        }
    }
}