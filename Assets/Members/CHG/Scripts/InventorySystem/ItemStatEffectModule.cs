using System;
using CHG.Scripts.Agents.StatSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.InventorySystem
{
    public class ItemStatEffectModule : MonoBehaviour, IModule, IAfterInitModule
    {
        private IInventoryModule _inventory;
        private IStatModule _statModule;
        
        public void Initialize(ModuleOwner owner)
        {
            _inventory = owner.GetModule<IInventoryModule>();
            _statModule = owner.GetModule<IStatModule>();
            
            Debug.Assert(_inventory != null, $"inventory is null : {gameObject.name}");
            Debug.Assert(_statModule != null, $"statModule is null : {gameObject.name}");
        }

        public void AfterInit()
        {
            foreach (ItemData item in _inventory.Items)
                HandleItemChanged(item, _inventory.GetCount(item));

            _inventory.OnItemChanged += HandleItemChanged;
        }

        private void OnDestroy()
        {
            if (_inventory != null)
                _inventory.OnItemChanged -= HandleItemChanged;
        }

        private void HandleItemChanged(ItemData item, int count)
        {
            if (item == null || item.statEffects == null) return;

            foreach (ItemStatEffect effect in item.statEffects)
            {
                if (effect.Stat == null) continue;
                
                if (count <= 0)
                    _statModule.RemoveModifier(effect.Stat.AssetIndex, item);
                else
                    _statModule.AddModifier(effect.Stat.AssetIndex, item, effect.Value * count);
            }
        }
    }
}