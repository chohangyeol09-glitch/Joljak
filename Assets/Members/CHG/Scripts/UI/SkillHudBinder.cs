using System;
using CHG.Scripts.SkillSystem;
using CHG.Scripts.UI.ViewModels;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace CHG.Scripts.UI
{
    public sealed class SkillHudBinder : IDisposable
    {
        private const string SlotNamePrefix = "skill-slot-";
        private const string CooldownName = "skill-cooldown";
        private const string CooldownLabelName = "skill-cooldown-label";
        private const string ChargeFullClass = "skill-slot__charge--full";
        private const string ChargeName = "skill-charge";

        private sealed class Slot
        {
            public SkillSlotViewModelSO Vm;
            public VisualElement Cooldown;
            public Label CooldownLabel;
            public IValueAnimation Animation;

            public ISkill Skill;
            public Action<float> CooldownHandler;
            public Action UsableHandler;
            
            public VisualElement Charge;
            public ChargeSkill Charging;      
            public float ChargeShown = -1f;
            public bool FullShown;
        }

        private readonly ISkillModule _skillModule;
        private readonly Slot[] _slots;
        private readonly IVisualElementScheduledItem _chargeTick;

        public SkillHudBinder(ISkillModule skillModule, VisualElement root, SkillSlotViewModelSO viewModelAsset)
        {
            _skillModule = skillModule;
            _slots = new Slot[skillModule.SlotCount];

            for (int i = 0; i < _slots.Length; i++)
            {
                VisualElement slotElement = root.Q(SlotNamePrefix + i);
                if (slotElement == null)
                {
                    Debug.LogError($"스킬 슬롯을 찾지 못했습니다 : {SlotNamePrefix + i}");
                    continue;
                }

                SkillSlotViewModelSO vm = UnityEngine.Object.Instantiate(viewModelAsset);
                vm.keyLabel = (i + 1).ToString();

                slotElement.dataSource = vm;

                _slots[i] = new Slot
                {
                    Vm = vm,
                    Cooldown = slotElement.Q(CooldownName),
                    CooldownLabel = slotElement.Q<Label>(CooldownLabelName),
                    Charge = slotElement.Q<VisualElement>(ChargeName)
                };

                BindSkill(i, skillModule.GetSkill(i));
            }

            _skillModule.OnSlotChanged += HandleSlotChanged;
            _chargeTick = root.schedule.Execute(TickCharge).Every(0);
        }

        private void TickCharge()
        {
            foreach (Slot slot in _slots)
            {
                if (slot?.Charge == null) continue;

                bool charging = slot.Charging != null && slot.Charging.IsUsing;
                float ratio = charging ? slot.Charging.ChargeRatio : 0f;
                bool full = charging && slot.Charging.IsFullyCharged;

                if (!Mathf.Approximately(ratio, slot.ChargeShown))
                {
                    slot.ChargeShown = ratio;
                    slot.Charge.style.height = Length.Percent(ratio * 100f);
                }

                if (full != slot.FullShown)
                {
                    slot.FullShown = full;
                    slot.Charge.EnableInClassList(ChargeFullClass, full);
                }
                
            }
        }

        public void Dispose()
        {
            _skillModule.OnSlotChanged -= HandleSlotChanged;
            _chargeTick?.Pause();

            foreach (Slot slot in _slots)
            {
                if (slot == null) continue;

                UnsubscribeSkill(slot);

                slot.Animation?.Stop();
                slot.Animation = null;

                if (slot.Vm != null)
                    UnityEngine.Object.Destroy(slot.Vm); 
                
            }
        }

        private void HandleSlotChanged(int slotIndex, ISkill skill) => BindSkill(slotIndex, skill);

        private void BindSkill(int slotIndex, ISkill skill)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length) return;

            Slot slot = _slots[slotIndex];
            if (slot == null) return;

            UnsubscribeSkill(slot);

            slot.Animation?.Stop();
            slot.Animation = null;
            ResetCooldownVisual(slot);

            slot.Skill = skill;

            if (skill == null)                 
            {
                slot.Vm.ClearSkill();
                return;
            }

            slot.Vm.SetSkill(skill.Data.AssetName, skill.Data.Icon);
            slot.Vm.SetUsable(skill.CanUseSkill());

            slot.CooldownHandler = duration => StartCooldownAnimation(slot, duration);
            slot.UsableHandler = () => slot.Vm.SetUsable(slot.Skill.CanUseSkill());
            
            slot.Charging = skill as ChargeSkill;

            skill.OnCooldownStarted += slot.CooldownHandler;
            skill.OnUsableChanged += slot.UsableHandler;
        }

        private static void UnsubscribeSkill(Slot slot)
        {
            if (slot.Skill == null) return;

            if (slot.CooldownHandler != null) slot.Skill.OnCooldownStarted -= slot.CooldownHandler;
            if (slot.UsableHandler != null) slot.Skill.OnUsableChanged -= slot.UsableHandler;

            slot.CooldownHandler = null;
            slot.UsableHandler = null;
            slot.Charging = null;
            slot.Skill = null;
        }

        private static void StartCooldownAnimation(Slot slot, float duration)
        {
            if (slot.Cooldown == null) return;

            slot.Animation?.Stop();

            slot.Animation = slot.Cooldown.experimental.animation.Start(
                    1f, 0f,
                    Mathf.Max(1, Mathf.RoundToInt(duration * 1000f)),
                    (element, value) =>
                    {
                        element.style.height = Length.Percent(value * 100f);

                        if (slot.CooldownLabel == null) return;

                        int remaining = Mathf.CeilToInt(value * duration);
                        slot.CooldownLabel.text = remaining > 0 ? remaining.ToString() : string.Empty;
                    })
                .Ease(Easing.Linear)
                .OnCompleted(() => ResetCooldownVisual(slot))
                .KeepAlive();
        }

        private static void ResetCooldownVisual(Slot slot)
        {
            if (slot.Cooldown != null)
                slot.Cooldown.style.height = Length.Percent(0f);

            if (slot.CooldownLabel != null)
                slot.CooldownLabel.text = string.Empty;
        }
    }
}
