using System;
using UnityEngine;

namespace CHG.Scripts.UI.ViewModels
{
    [CreateAssetMenu(fileName = "Ammo view model", menuName = "UI/Ammo view", order = 0)]
    public class AmmoViewModelSO : AbstractCountViewModelSO
    {
        public bool isReloading;
        
        public void SetReloading(bool reloading) => isReloading = reloading;
    }
}