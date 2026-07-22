using Members.CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace Members.CHG.Scripts.Weapon
{
    public class Gun : MonoBehaviour, IWeapon, IReloadable, IModule
    {
        
        
        [Header("Weapon")]
        [field: SerializeField] public WeaponData Data { get; private set; }
        public bool CanFire { get; private set; }
        
        [Header("Reload")]
        [field: SerializeField] public int CurrentAmmo { get; private set; }
        [field: SerializeField] public int MaxAmmo { get; private set; }
        public bool IsReloading { get; private set; }
        [field: SerializeField] public float ReloadTime { get; private set; }

        
        
        public void Initialize(ModuleOwner owner)
        {
            CurrentAmmo = MaxAmmo;   
        }
        
        public void OnStartFire()
        {
            if (!CanFire || IsReloading || CurrentAmmo <= 0) return;
            
        }

        public void OnStopFire()
        {
        }

        public void Reload()
        {
            
        }

        
    }
}