using System;
using CHG.Scripts.SkillSystem.SkillDatas;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.SkillSystem
{
    public class PlayerSkillModule : MonoBehaviour, IModule, IAfterInitModule, ISkillModule
    {
        public const int Slots = 4;

        [SerializeField] private SkillDataSO[] startingSkills = new SkillDataSO[Slots];

        private readonly AbstractSkill[] _slots = new AbstractSkill[Slots];
        
        public ModuleOwner Owner => _owner;
        public int SlotCount => Slots;
        public AbstractSkill CurrentSkill { get; private set; }

        

        public event Action<int, ISkill> OnSlotChanged;
        
        private ModuleOwner _owner;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
        }

        public void AfterInit()
        {
            for (int i = 0; i < Slots; i++)
            {
                SetSkill(i, i < startingSkills.Length ? startingSkills[i] : null);
            }
        }

        
        public ISkill GetSkill(int skillIndex)
        {
            return IsValidSlot(skillIndex) ? _slots[skillIndex] : null;
        }

        public void SetSkill(int slotIndex, SkillDataSO data)
        {
            if (!IsValidSlot(slotIndex)) return;

            AbstractSkill prev = _slots[slotIndex];
            if (prev != null)
            {
                prev.StopSkill();
                prev.OnSkillEnded -= HandleSkillEnded;
                if (ReferenceEquals(prev, CurrentSkill)) CurrentSkill = null;
                Destroy(prev.gameObject);
            }

            AbstractSkill skill = null;

            if (data != null)
            {
                if (data.SkillPrefab == null)
                {
                    Debug.LogError($"SkillPrefab is null : {data.AssetName}");
                }
                else
                {
                    skill = Instantiate(data.SkillPrefab, transform);
                    skill.InitializeSkill(this, data);
                    skill.OnSkillEnded += HandleSkillEnded;
                }
            }
            
            _slots[slotIndex] = skill;
            OnSlotChanged?.Invoke(slotIndex, skill);
        }


        public bool TryUseSkill(int slotIndex, GameObject target = null)
        {
            if (!IsValidSlot(slotIndex)) return false;
            
            AbstractSkill skill = _slots[slotIndex];
            if (skill == null || !skill.CanUseSkill(target)) return false;
            
            CurrentSkill = skill;
            skill.UseSkill(target);
            return true;
        }

        private void HandleSkillEnded(ISkill skill)
        {
            if (ReferenceEquals(skill, CurrentSkill))
                CurrentSkill = null;
        }
        
        public void ReleaseSkill(int slotIndex)
        {
            if (!IsValidSlot(slotIndex)) return;
            _slots[slotIndex]?.ReleaseSkill();
        }

        private void OnDestroy()
        {
            foreach (AbstractSkill skill in _slots) 
                if (skill != null) skill.OnSkillEnded -= HandleSkillEnded;
        }

        private static bool IsValidSlot(int slotIndex) => slotIndex >= 0 && slotIndex < Slots;
    }
}