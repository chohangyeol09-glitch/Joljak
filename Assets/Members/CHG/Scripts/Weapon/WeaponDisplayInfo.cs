using System;
using UnityEngine;

namespace CHG.Scripts.Weapon
{
    [Serializable]
    public class WeaponDisplayInfo
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
    }
}