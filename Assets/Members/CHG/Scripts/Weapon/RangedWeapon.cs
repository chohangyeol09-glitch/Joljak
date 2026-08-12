using System;
using CHG.Scripts.Agents;
using CHG.Scripts.Agents.StatSystem;
using CHG.Scripts.CombatSystem;
using CHG.Scripts.CoreSystem.AnimationSystem;
using CHG.Scripts.Weapon.WeaponSO;
using DevLib.ModuleSystem;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG.Scripts.Weapon
{
    public class RangedWeapon : AbstractWeapon, IReloadable
    {
        [Header("Fire")]
        [SerializeField] private Transform muzzle;
        [SerializeField] private PoolManagerSO poolManager;

        [Header("Attack Anim")]
        [SerializeField] private AnimParamSO attackParam;
        [SerializeField] private AnimParamSO upperIdleParam;
        [SerializeField] private int upperBodyLayer = 1;

        private RangedWeaponSO _data;
        private IAimModule _aim;
        private IStatModule _statModule;
        private int _currentAmmo;
        private float _reloadEndTime;

        public bool IsReloading { get; private set; }
        public float ReloadProgress { get; private set; }

        public event Action<int, int> OnAmmoChanged;
        public event Action<float> OnReloadStarted;
        public event Action OnReloadEnded;

        public int MaxAmmo => _data.MaxAmmo;
        public int CurrentAmmo => _currentAmmo;
        public float ReloadTime => _data.ReloadTime;

        public override bool CanFire => base.CanFire && !IsReloading && _currentAmmo > 0;

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);

            _data = data as RangedWeaponSO;
            _aim = owner.GetModule<IAimModule>();
            _statModule = owner.GetModule<IStatModule>();

            Debug.Assert(_data != null, $"RangedWeaponData is null : {gameObject.name}");
            Debug.Assert(_aim != null, $"AimModule is null : {gameObject.name}");
            Debug.Assert(_statModule != null, $"StatModule is null : {gameObject.name}");

            _currentAmmo = MaxAmmo;
            RaiseAmmoChanged();
        }

        public override void StartFire()
        {
            base.StartFire();
            _renderer.Animator.SetBool("FIRING", true);
        }

        public override void StopFire()
        {
            base.StopFire();
            _renderer.Animator.SetBool("FIRING", false);
            _renderer.PlayClip(upperIdleParam.ParamHash, 0f,0.02f, upperBodyLayer);
        }

        protected override void UpdateWeapon()
        {
            if (IsReloading)
            {
                ReloadProgress = 1f - (_reloadEndTime - Time.time) / ReloadTime;
                if (Time.time >= _reloadEndTime)
                    CompleteReload();
                return;
            }

            if (_isFiring && _currentAmmo <= 0)
                Reload();
        }

        protected override void Fire()
        {
            Vector3 target = _aim.AimPoint;
            if ((target - muzzle.position).sqrMagnitude < 1f)
                target = muzzle.position + _aim.AimForward;

            Vector3 dir = (target - muzzle.position).normalized;

            Bullet bullet = poolManager.Pop<Bullet>(_data.Bullet);
            DamageData damageData = _statModule.CalculateDamage(_data.ToDamageSource());
            bullet.Launch(muzzle.position, dir, damageData);

            _currentAmmo--;
            RaiseAmmoChanged();
            _renderer.PlayClip(attackParam.ParamHash, 0f,0.02f, upperBodyLayer);
        }

        public void Reload()
        {
            if (IsReloading || _currentAmmo == MaxAmmo) return;

            IsReloading = true;
            ReloadProgress = 0f;
            _reloadEndTime = Time.time + ReloadTime;

            OnReloadStarted?.Invoke(ReloadTime);
        }

        public void CancelReload()
        {
            if (!IsReloading) return;

            IsReloading = false;
            ReloadProgress = 0f;

            OnReloadEnded?.Invoke();
        }

        private void CompleteReload()
        {
            IsReloading = false;
            ReloadProgress = 0f;
            _currentAmmo = MaxAmmo;

            RaiseAmmoChanged();
            OnReloadEnded?.Invoke();
        }

        private void RaiseAmmoChanged() => OnAmmoChanged?.Invoke(_currentAmmo, MaxAmmo);
    }
}
