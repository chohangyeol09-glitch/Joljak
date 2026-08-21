using System;
using System.Collections.Generic;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.InventorySystem
{
    public class InventoryModule : MonoBehaviour, IModule, IInventoryModule
    {
        private readonly List<ItemData> _order = new();
        private readonly Dictionary<ItemData, int> _counts = new();

        public IReadOnlyList<ItemData> Items => _order;
        public event Action<ItemData, int> OnItemChanged;
        
        public void Initialize(ModuleOwner owner) { }
        
        public int GetCount(ItemData item) => item != null && _counts.TryGetValue(item, out int count) ? count : 0;

        public void Add(ItemData item, int amount = 1)
        {
            if (item == null || amount <= 0) return;

            if (_counts.TryGetValue(item, out int current))
            {
                _counts[item] = current + amount;
            }
            else
            {
                _counts.Add(item, amount);
                _order.Add(item);
            }
            
            OnItemChanged?.Invoke(item, _counts[item]);
        }
    }
}