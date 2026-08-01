using System.Collections.Generic;
using CHG.Scripts.CombatSystem;
using CHG.Scripts.CoreSystem.ModuleSystem;
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
        private const string ReloadFillName = "ammo-reload-fill";

        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private HealthViewModelSO healthViewModel;
        [SerializeField] private AmmoViewModelSO ammoViewModelAsset;

        private readonly List<ScriptableObject> _runtimeViewModels = new();

        private HealthModule _health;
        private IReloadable _reloadable;

        private HealthViewModelSO _healthVm;
        private AmmoViewModelSO _ammoVm;

        // 재장전 게이지. 표시 여부는 isReloading 바인딩이 처리하고,
        // 채워지는 애니메이션만 여기서 돌린다.
        private VisualElement _reloadFill;
        private IValueAnimation _reloadAnimation;

        public void Initialize(ModuleOwner owner)
        {
            _health = owner.GetModule<HealthModule>();
            _reloadable = owner.GetModule<IWeapon>() as IReloadable;

            Debug.Assert(uiDocument != null, $"uiDocument is null : {gameObject.name}");
            Debug.Assert(_health != null, $"health is null : {gameObject.name}");
            Debug.Assert(healthViewModel != null, $"healthViewModel is null : {gameObject.name}");
        }

        // UIDocument.rootVisualElement 는 Awake 시점에 아직 null 이므로 Start 에서 구성한다.
        private void Start()
        {
            VisualElement root = uiDocument.rootVisualElement;

            BindHealth(root);
            BindAmmo(root);
        }

        private void OnDestroy()
        {
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

            _reloadFill = root.Q(ReloadFillName);
            Debug.Assert(_reloadFill != null, $"{ReloadFillName} 을 찾지 못했습니다 : {gameObject.name}");

            _reloadable.OnAmmoChanged += HandleAmmoChanged;
            _reloadable.OnReloadStarted += HandleReloadStarted;
            _reloadable.OnReloadEnded += HandleReloadEnded;

            HandleAmmoChanged(_reloadable.CurrentAmmo, _reloadable.MaxAmmo);
        }

        private void HandleHealthChanged(int current, int max) => _healthVm.SetValue(current, max);
        private void HandleAmmoChanged(int current, int max) => _ammoVm.SetValue(current, max);

        private void HandleReloadStarted(float duration)
        {
            _ammoVm.SetReloading(true); // 게이지 표시 여부는 바인딩이 처리한다

            if (_reloadFill == null) return;

            _reloadAnimation?.Stop(); // 이전 애니메이션이 남아 있으면 정리

            // Ease 를 지정하지 않으면 기본값이 Easing.OutQuad 라 끝부분에서 감속한다.
            // 재장전 게이지는 남은 시간을 그대로 보여줘야 하므로 등속(Linear)으로 고정한다.
            _reloadAnimation = _reloadFill.experimental.animation.Start(
                    0f, 1f,
                    Mathf.Max(1, Mathf.RoundToInt(duration * 1000f)),
                    (element, value) => element.style.width = Length.Percent(value * 100f))
                .Ease(Easing.Linear);
        }

        // 완료와 취소 모두 여기로 온다. 어느 쪽이든 게이지는 멈추고 0 으로 돌아간다.
        private void HandleReloadEnded()
        {
            _ammoVm.SetReloading(false);

            if (_reloadFill == null) return;

            _reloadAnimation?.Stop();
            _reloadAnimation = null;

            _reloadFill.style.width = Length.Percent(0f);
        }

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
