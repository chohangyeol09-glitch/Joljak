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
        [field: SerializeField] public WeaponData Data { get; private set; }
        public bool CanFire => !IsReloading
                               && CurrentAmmo > 0
                               && !_constraint.Has(ConstraintType.Disarmed);
        
        [Header("Reload")]
        [field: SerializeField] public int CurrentAmmo { get; private set; }
        [field: SerializeField] public int MaxAmmo { get; private set; }
        public bool IsReloading { get; private set; }
        [field: SerializeField] public float ReloadTime { get; private set; }

        [Header("Fire")] 
        [SerializeField] private Transform muzzle;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO bulletItem;

        [Header("Attack Anim")] 
        [SerializeField] private AnimParamSO attackParam;
        [SerializeField] private AnimParamSO upperIdleParam;
        [SerializeField] private int upperBodyLayer = 1;
        
        private IRenderer _renderer;
        private IAimModule _aim;
        private ConstraintModule _constraint;
        private bool _isFiring;
        private float _nextFireTime;
        
        public void Initialize(ModuleOwner owner)
        {
            _aim = owner.GetModule<IAimModule>();
            _renderer = owner.GetModule<IRenderer>();
            
            MaxAmmo = Data.MaxAmmo;
            ReloadTime = Data.ReloadTime;
            CurrentAmmo = MaxAmmo;   
            _constraint = owner.GetModule<ConstraintModule>();
            
            Debug.Assert(_constraint != null, $"ConstraintModule is null : {gameObject.name}");
            Debug.Assert(_renderer != null, $"Renderer is null : {gameObject.name}");
            Debug.Assert(_aim != null, $"AimModule is null : {gameObject.name}");
        }
        
        public void OnStartFire()
        {
            _isFiring = true;    
            _renderer.Animator.SetBool("FIRING", _isFiring);
        }

        public void OnStopFire()
        {
            _isFiring = false;
            _renderer.Animator.SetBool("FIRING", _isFiring);
            _renderer.PlayClip(upperIdleParam.ParamHash, 0f,0.02f, upperBodyLayer);
        }

        private void Update()
        {
            if (!_isFiring || Time.time < _nextFireTime || !CanFire) return;

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
            bullet.Launch(muzzle.position, dir);
            
            CurrentAmmo--;
            _renderer.PlayClip(attackParam.ParamHash, 0f,0.02f, upperBodyLayer);
        }

        public void Reload()
        {
            
        }
    }
}