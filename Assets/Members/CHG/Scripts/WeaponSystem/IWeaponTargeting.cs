using UnityEngine;

namespace CHG.Scripts.WeaponSystem
{
    public interface IWeaponTargeting
    {
        Transform FindTarget(Vector3 origin);
    }
}