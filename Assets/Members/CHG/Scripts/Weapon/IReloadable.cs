using System;

namespace CHG.Scripts.Weapon
{
    public interface IReloadable
    {
        int MaxAmmo { get; }
        int CurrentAmmo { get; }
        bool IsReloading { get; }
        float ReloadProgress { get; }
        
        void Reload();
        void CancelReload();
        
        event Action<int, int> OnAmmoChanged; //current, max
        event Action<float> OnReloadStarted;
        event Action OnReloadEnded;
    }
}