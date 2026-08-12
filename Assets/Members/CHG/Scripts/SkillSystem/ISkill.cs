using System;
using CHG.Scripts.SkillSystem.SkillDatas;
using UnityEngine;

namespace CHG.Scripts.SkillSystem
{
    public interface ISkill
    {
        SkillDataSO Data { get; }

        bool IsUsing { get; }
        float RemainingCooldown { get; }    
        float NormalizedCooldown { get; }   
        bool IsOnCooldown { get; }
        bool IsBlocked { get; }

        event Action<float> OnCooldownStarted;

        event Action OnUsableChanged;

        event Action OnSkillStarted;
        event Action<ISkill> OnSkillEnded;

        void InitializeSkill(ISkillModule module, SkillDataSO data = null);
        bool CanUseSkill(GameObject target = null);
        void UseSkill(GameObject target = null);
        void ReleaseSkill();
        void StopSkill();
    }
}