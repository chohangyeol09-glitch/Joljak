using System;
using System.Collections.Generic;
using UnityEngine;

namespace CHG.Scripts.InventorySystem
{
    public interface IInventoryModule
    {
        IReadOnlyList<ItemData> Items { get; }

        event Action<ItemData, int> OnItemChanged;
        
        int GetCount(ItemData item);
        void Add(ItemData item, int amount = 1);
        
        
    }
}
