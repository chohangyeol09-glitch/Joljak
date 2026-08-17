using System;
using System.Collections.Generic;
using CHG.Scripts.InventorySystem;
using CHG.Scripts.Players;
using CHG.Scripts.UI.ViewModels;
using DefaultNamespace;
using DevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace CHG.Scripts.UI
{
    public class InventoryHudModule : MonoBehaviour, IModule
    {
        private const string RootName = "inventory-root";
        private const string ListName = "inventory-list";

        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset itemViewTemplate;
        [SerializeField] private InventoryItemViewModelSO itemViewModelAsset;

        
        private IInventoryModule _inventory;
        private PlayerInputSO _playerInput;

        private VisualElement _root;
        private VisualElement _list;

        private readonly RuntimeViewModels _viewModels = new();
        private readonly Dictionary<ItemData, InventoryItemViewModelSO> _rows = new();

        private bool _isOpen;
        
        public void Initialize(ModuleOwner owner)
        {
            _inventory = owner.GetModule<IInventoryModule>();
            _playerInput = (owner as PlayerController)?.PlayerInput;
            
            Debug.Assert(uiDocument != null, $"uiDocument is null : {gameObject.name}");
            Debug.Assert(itemViewTemplate != null, $"itemViewTemplate is null : {gameObject.name}");
            Debug.Assert(itemViewModelAsset != null, $"ItemViewModelAsset is null : {gameObject.name}");
            Debug.Assert(_inventory != null, $"inventory is null : {gameObject.name}");
            Debug.Assert(_playerInput != null, $"playerInput is null : {gameObject.name}");
        }

        private void Start()
        {
            VisualElement root = uiDocument.rootVisualElement;
            
            _root = root.Q(RootName);
            _list = root.Q(ListName);
            
            Debug.Assert(_root != null, $"{RootName} is null : {gameObject.name}");
            Debug.Assert(_list != null, $"{ListName} is null : {gameObject.name}");

            foreach (ItemData item in _inventory.Items)
                HandleItemChanged(item, _inventory.GetCount(item));

            _inventory.OnItemChanged += HandleItemChanged;
            _playerInput.OnInventoryKeyDown += Toggle;

            SetOpen(false);
        }

        private void OnDestroy()
        {
            if (_inventory != null)
                _inventory.OnItemChanged -= HandleItemChanged;

            if (_playerInput != null)
                _playerInput.OnInventoryKeyDown -= Toggle;
            
            _rows.Clear();
            _viewModels.Dispose();
        }
        
        private void Toggle() => SetOpen(!_isOpen);

        private void SetOpen(bool open)
        {
            _isOpen = open;

            if (_root != null)
                _root.style.display = open ? DisplayStyle.Flex : DisplayStyle.None;

           _playerInput.SetGameplayInputEnabled(!open); 
            
            Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = open;
        }

        private void HandleItemChanged(ItemData item, int count)
        {
            if (item == null) return;

            if (_rows.TryGetValue(item, out InventoryItemViewModelSO vm))
            {
                vm.SetCount(count);
                return;
            }

            vm = _viewModels.Create(itemViewModelAsset);
            vm.SetItem(item.itemName, item.icon, count);
            _rows.Add(item, vm);

            VisualElement entry = itemViewTemplate.Instantiate();

            entry.style.flexShrink = 0;
            entry.dataSource = vm;
            _list.Add(entry);
            
        }
        
    }
}