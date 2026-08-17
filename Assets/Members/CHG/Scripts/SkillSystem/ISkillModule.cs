using System;
using CHG.Scripts.SkillSystem.SkillDatas;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.SkillSystem
{
    public interface ISkillModule
    {
        ModuleOwner Owner { get; }
        int SlotCount { get; }

        ISkill GetSkill(int skillIndex);
        void SetSkill(int slotIndex, SkillDataSO data);
        bool TryUseSkill(int slotIndex, GameObject target = null);
        void ReleaseSkill(int slotIndex);

        event Action<int, ISkill> OnSlotChanged;
    }
}