using System;
using System.Collections.Generic;
using CHG.Scripts.CombatSystem;
using CHG.Scripts.CoreSystem.ModuleSystem;
using CHG.Scripts.UI.ViewModels;
using CHG.Scripts.Weapon;
using UnityEngine;
using UnityEngine.UIElements;

namespace CHG.Scripts.UI
{
    public class PlayerHudModule : MonoBehaviour, IModule
    {
        private const string HealthPanelName = "health-hud";
        private const string AmmoPanelName = "ammo-hud";
        
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private HealthViewModelSO healthViewModel;
        [SerializeField] private AmmoViewModelSO ammoViewModelAsset;
        
        private readonly List<ScriptableObject> _runtimeViewModels = new();

        private HealthModule _health;
        private IReloadable _reloadable;
        
        private HealthViewModelSO _healthVm;
        private AmmoViewModelSO _ammoVm;
        
        public void Initialize(ModuleOwner owner)
        {
            _health = owner.GetModule<HealthModule>();    
            _reloadable = owner.GetModule<IWeapon>() as IReloadable;

            Debug.Assert(uiDocument != null, $"uiDocument is null : {gameObject.name}");
            Debug.Assert(_health != null, $"health is null : {gameObject.name}");
            Debug.Assert(healthViewModel != null, $"healthViewModel is null : {gameObject.name}");
        }

        private void Start()
        {
            VisualElement root = uiDocument.rootVisualElement;

            BindHealth(root);
            BindAmmo(root);
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.OnHealthChanged -= HandleHealthChanged;

            if (_reloadable != null)
            {
                _reloadable.OnAmmoChanged -= HandleAmmoChanged;
                _reloadable.OnReloadStarted -= HandleReloadStarted;
                _reloadable.OnReloadEnded -= HandleReloadEnded;
            }
            
            foreach (ScriptableObject vm in _runtimeViewModels)
                Destroy(vm);
            
            _runtimeViewModels.Clear();
        }

        private void BindHealth(VisualElement root)
        {
            _healthVm = Register(Instantiate(healthViewModel));
            BindPanel(root, HealthPanelName, _healthVm);

            _health.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(_health.CurrentHealth, _health.MaxHealth);
        }

        private void BindAmmo(VisualElement root)
        {
            if (_reloadable == null)
            {
                HidePanel(root, AmmoPanelName);
                return;
            }

            if (ammoViewModelAsset == null)
            {
                Debug.LogError($"ammoViewModel 에셋이 없습니다 : {gameObject.name}");
                HidePanel(root, AmmoPanelName);
                return;
            }
            
            _ammoVm = Register(Instantiate(ammoViewModelAsset));
            BindPanel(root, AmmoPanelName, _ammoVm);

            _reloadable.OnAmmoChanged += HandleAmmoChanged;
            _reloadable.OnReloadStarted += HandleReloadStarted;
            _reloadable.OnReloadEnded += HandleReloadEnded;
            
            HandleAmmoChanged(_reloadable.CurrentAmmo, _reloadable.MaxAmmo);
        }
        
        private void HandleHealthChanged(int current, int max) => _healthVm.SetValue(current, max);
        private void HandleAmmoChanged(int current, int max) => _ammoVm.SetValue(current, max);
        private void HandleReloadEnded() => _ammoVm.SetReloading(false);
        private void HandleReloadStarted(float dutation) => _ammoVm.SetReloading(true);

        private T Register<T>(T viewModel) where T : ScriptableObject
        {
            _runtimeViewModels.Add(viewModel);
            return viewModel;
        }

        private static void BindPanel(VisualElement root, string panelName, ScriptableObject viewModel)
        {
            VisualElement panel = root.Q(panelName);
            if (panel == null)
            {
                Debug.LogError($"panel is null : {panelName}");
                return;
            }

            panel.dataSource = viewModel;
        }

        private static void HidePanel(VisualElement root, string panelName)
        {
            VisualElement panel = root.Q(panelName);
            if (panel != null)
                panel.style.display = DisplayStyle.None;
        }
    }
}