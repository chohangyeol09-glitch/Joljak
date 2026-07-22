using UnityEngine;

namespace Members.CHG.Scripts.Weapon
{
    [CreateAssetMenu(fileName = "Weapon data", menuName = "Weapon/Weapon data", order = 0)]
    public class WeaponData : ScriptableObject
    {
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float FireRate { get; private set; }
        
    }
}