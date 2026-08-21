using CHG.Scripts.CombatSystem;
using UnityEngine;

namespace CHG.Scripts.WeaponSystem.WeaponSO
{
    [CreateAssetMenu(fileName = "Weapon data", menuName = "Weapon/Weapon data")]
    public class WeaponDataSO : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; private set; }    
        [field: SerializeField] public DamageType DamageType { get; private set; }
        [field: Tooltip("초당 공격 수")]
        [field: SerializeField] public float AttackRate { get; private set; }
        
        [Header("Display")]
        [field: SerializeField] public WeaponDisplayInfo Display { get; private set; }
        
        public DamageSource ToDamageSource(float multiplier = 1f) => new DamageSource(DamageType, Damage, multiplier);
    }
}