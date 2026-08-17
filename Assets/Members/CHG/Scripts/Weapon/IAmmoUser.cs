using System;

namespace CHG.Scripts.Weapon
{
    public interface IAmmoUser
    {
        int CurrentAmmo { get; }
        int MaxAmmo { get; }
        event Action<int, int> OnAmmoChanged;
    }
}