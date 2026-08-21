using UnityEngine;

namespace CHG.Scripts.UI.ViewModels
{
    [CreateAssetMenu(fileName = "Inventory item view model", menuName = "UI/Inventory item view", order = 0)]
    public class InventoryItemViewModelSO : ScriptableObject
    {
        public string labelName;
        public Sprite icon;
        public int count;

        public void SetItem(string itemName, Sprite itemIcon, int itemCount)
        {
            labelName = itemName;
            icon = itemIcon;
            count = itemCount;
        }
        
        public void SetCount(int itemCount) => count = itemCount;

    }
}