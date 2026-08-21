using System;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.WeaponSystem
{
    public class AmmoCost : AbstractWeaponPart, IReloadable
    {
        [SerializeField] private int maxAmmo = 50;
        [SerializeField] private float reloadTime = 3f;
        
        private int _currentAmmo;
        private float _reloadEndTime;
        
        public int MaxAmmo => maxAmmo;
        public int CurrentAmmo => _currentAmmo;
        public override bool CanFire => !IsReloading && _currentAmmo > 0;
        public bool IsReloading { get; private set; }
        public float ReloadProgress { get; private set; }

        public event Action<int, int> OnAmmoChanged;
        public event Action<float> OnReloadStarted;
        public event Action OnReloadEnded;

        public override void InitPart(Weapon weapon, ModuleOwner owner)
        {
            base.InitPart(weapon, owner);
            
            _currentAmmo = maxAmmo;
            RaiseAmmoChanged();
        }

        public override void UpdatePart()
        {
            if (IsReloading)
            {
                ReloadProgress = 1f - (_reloadEndTime - Time.time) / Mathf.Max(reloadTime, 0.01f);
                if (Time.time >= _reloadEndTime)
                    CompleteReload();
                return;
            }
            
            if (_weapon.IsFiring && _currentAmmo <= 0)
                Reload();
        }

        public override void OnFired()
        {
            _currentAmmo--;
            RaiseAmmoChanged();
        }

        public void Reload()
        {
            if (IsReloading || _currentAmmo == maxAmmo) return;
            
            IsReloading = true; 
            ReloadProgress = 0f;
            _reloadEndTime = Time.time + reloadTime;
            
            OnReloadStarted?.Invoke(reloadTime);
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
            _currentAmmo = maxAmmo;

            RaiseAmmoChanged();
            OnReloadEnded?.Invoke();
        }
        
        private void RaiseAmmoChanged() => OnAmmoChanged?.Invoke(_currentAmmo, maxAmmo);
        
    }
}