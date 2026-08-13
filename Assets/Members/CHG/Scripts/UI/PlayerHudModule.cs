using System.Collections.Generic;
using CHG.Scripts.CombatSystem;
using CHG.Scripts.SkillSystem;
using DevLib.ModuleSystem;
using CHG.Scripts.UI.ViewModels;
using CHG.Scripts.Weapon;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace CHG.Scripts.UI
{
    public class PlayerHudModule : MonoBehaviour, IModule
    {
        private const string HealthPanelName = "health-hud";
        private const string AmmoPanelName = "ammo-hud";
        private const string SkillPanelName = "skill-hud";
        private const string ReloadFillName = "ammo-reload-fill";

        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private HealthViewModelSO healthViewModel;
        [SerializeField] private AmmoViewModelSO ammoViewModelAsset;
        [SerializeField] private SkillSlotViewModelSO skillSlotViewModelAsset;

        private readonly RuntimeViewModels _viewModels = new();

        private HealthModule _health;
        private IReloadable _reloadable;
        private ISkillModule _skillModule;

        private SkillHudBinder _skillBinder;

        private HealthViewModelSO _healthVm;
        private AmmoViewModelSO _ammoVm;

        private VisualElement _reloadFill;
        private IValueAnimation _reloadAnimation;

        public void Initialize(ModuleOwner owner)
        {
            _health = owner.GetModule<HealthModule>();
            _reloadable = owner.GetModule<IWeapon>() as IReloadable;
            _skillModule = owner.GetModule<ISkillModule>();

            Debug.Assert(uiDocument != null, $"uiDocument is null : {gameObject.name}");
            Debug.Assert(_health != null, $"health is null : {gameObject.name}");
            Debug.Assert(healthViewModel != null, $"healthViewModel is null : {gameObject.name}");
        }

        private void Start()
        {
            VisualElement root = uiDocument.rootVisualElement;

            BindHealth(root);
            BindAmmo(root);
            BindSkills(root);
        }

        private void OnDestroy()
        {
            _skillBinder?.Dispose();
            _skillBinder = null;

            _reloadAnimation?.Stop();
            _reloadAnimation = null;

            if (_health != null)
                _health.OnHealthChanged -= HandleHealthChanged;

            if (_reloadable != null)
            {
                _reloadable.OnAmmoChanged -= HandleAmmoChanged;
                _reloadable.OnReloadStarted -= HandleReloadStarted;
                _reloadable.OnReloadEnded -= HandleReloadEnded;
            }

            _viewModels.Dispose();
        }

        private void BindHealth(VisualElement root)
        {
            _healthVm = _viewModels.Create(healthViewModel);
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

            _ammoVm = _viewModels.Create(ammoViewModelAsset);
            BindPanel(root, AmmoPanelName, _ammoVm);

            _reloadFill = root.Q(ReloadFillName);
            Debug.Assert(_reloadFill != null, $"{ReloadFillName} 을 찾지 못했습니다 : {gameObject.name}");

            _reloadable.OnAmmoChanged += HandleAmmoChanged;
            _reloadable.OnReloadStarted += HandleReloadStarted;
            _reloadable.OnReloadEnded += HandleReloadEnded;

            HandleAmmoChanged(_reloadable.CurrentAmmo, _reloadable.MaxAmmo);
        }

        private void BindSkills(VisualElement root)
        {
            if (_skillModule == null)              
            {
                HidePanel(root, SkillPanelName);
                return;
            }

            if (skillSlotViewModelAsset == null)
            {
                Debug.LogError($"skillSlotViewModel 에셋이 없습니다 : {gameObject.name}");
                HidePanel(root, SkillPanelName);
                return;
            }

            _skillBinder = new SkillHudBinder(_skillModule, root, skillSlotViewModelAsset);
        }

        private void HandleHealthChanged(int current, int max) => _healthVm.SetValue(current, max);
        private void HandleAmmoChanged(int current, int max) => _ammoVm.SetValue(current, max);

        private void HandleReloadStarted(float duration)
        {
            _ammoVm.SetReloading(true); 

            if (_reloadFill == null) return;

            _reloadAnimation?.Stop();

            _reloadAnimation = _reloadFill.experimental.animation.Start(
                    0f, 1f,
                    Mathf.Max(1, Mathf.RoundToInt(duration * 1000f)),
                    (element, value) => element.style.width = Length.Percent(value * 100f))
                .Ease(Easing.Linear)
                .KeepAlive();

        }

        private void HandleReloadEnded()
        {
            _ammoVm.SetReloading(false);

            if (_reloadFill == null) return;

            _reloadAnimation?.Stop();
            _reloadAnimation = null;

            _reloadFill.style.width = Length.Percent(0f);
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
