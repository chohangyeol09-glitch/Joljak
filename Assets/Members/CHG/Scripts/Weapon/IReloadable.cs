using System;

namespace CHG.Scripts.Weapon
{
    public interface IReloadable
    {
        bool IsReloading { get; }
        float ReloadProgress { get; }
        void Reload();
        void CancelReload();
        event Action OnReloadStarted;
        event Action OnReloadEnded;
    }
}