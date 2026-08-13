using System;
using CHG.Scripts.Players.InteractionSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.InventorySystem
{
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private int amount = 1;
        [SerializeField] private GameObject highlight;

        private void Awake()
        {
            Debug.Assert(itemData != null, $"itemData is null : {gameObject.name}");
            OnFocusExit();
        }

        public void OnFocusEnter()
        {
            if (highlight != null)
                highlight.SetActive(true);
        }

        public void OnFocusExit()
        {
            if (highlight != null)
                highlight.SetActive(false);
        }

        public void Interact(ModuleOwner interactor)
        {
            IInventoryModule inventory = interactor.GetModule<IInventoryModule>();
            if (inventory == null) return;
            
            inventory.Add(itemData, amount);
            Destroy(gameObject);
        }

        public void Performed() { }
        public void Cancel() { }
    }
}