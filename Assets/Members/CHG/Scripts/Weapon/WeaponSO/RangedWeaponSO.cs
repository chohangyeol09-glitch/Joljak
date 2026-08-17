using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG.Scripts.Weapon.WeaponSO
{
    [CreateAssetMenu(fileName = "Gun data", menuName = "weapon/Gun data", order = 0)]
    public class RangedWeaponSO : AbstractWeaponData
    {
        [field: SerializeField] public float ReloadTime { get; private set; }
        [field: SerializeField] public int MaxAmmo { get; private set; }
        [field: SerializeField] public PoolItemSO Bullet { get; private set; }
    }
}