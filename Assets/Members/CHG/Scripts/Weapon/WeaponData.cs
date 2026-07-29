using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG.Scripts.Weapon
{
    [CreateAssetMenu(fileName = "Weapon data", menuName = "Weapon/Weapon data", order = 0)]
    public class WeaponData : ScriptableObject
    {
        
        [field: SerializeField] public int Damage { get; private set; }
        [Tooltip("초당 발사 수")]
        [field: SerializeField] public float FireRate { get; private set; }
        [field: SerializeField] public float ReloadTime { get; private set; }
        [field: SerializeField] public int MaxAmmo { get; private set; }
        [field: SerializeField] public PoolItemSO Bullet { get; private set; }
        
        [Header("Display")]
        [field: SerializeField] public WeaponDisplayInfo Display { get; private set; }
    }
}