using System;
using CHG.Scripts.CoreSystem.AnimationSystem;
using UnityEngine;

namespace CHG.Scripts.Weapon.WeaponSO
{
    [CreateAssetMenu(fileName = "Close weapon data", menuName = "weapon/Close weapon data", order = 0)]
    public class MeleeWeaponSO : AbstractWeaponData
    {
        [Serializable]
        public class ComboStep
        {
            public AnimParamSO Clip;
            public AnimationCurve Curve;
            public float Duration;
            public float DamageMultiplier = 1f;
        }
        public float ComboWindow;
        
        public ComboStep[] ComboSteps;
                
    }
}