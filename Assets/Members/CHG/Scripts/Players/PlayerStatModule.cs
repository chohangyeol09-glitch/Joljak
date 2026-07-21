using Members.CHG.Scripts.Agents.StatSystem;
using Members.CHG.Scripts.CombatSystem;
using Members.CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace Player
{
    public class PlayerStatModule : AbstractAgentStatModule
    {
        [Header("Damage related stat list")] 
        [SerializeField] private StatSO strStat;
        [SerializeField] private StatSO intStat;
        [SerializeField] private StatSO criStat;
        [SerializeField] private StatSO cDmgStat;

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);

            if (!TryGetStat(strStat.AssetIndex, out strStat))
                Debug.LogError($"플레이어에 힘 스탯이 없습니다.");
            if (!TryGetStat(strStat.AssetIndex, out intStat))
                Debug.LogError($"플레이어에 지능 스탯이 없습니다.");
            if (!TryGetStat(strStat.AssetIndex, out criStat))
                Debug.LogError($"플레이어에 크리티컬 스탯이 없습니다.");
            if (!TryGetStat(strStat.AssetIndex, out cDmgStat))
                Debug.LogError($"플레이어에 크리티컬 증뎀 스탯이 없습니다.");
        }

        public override DamageData CalculateDamage(SkillDataSO skillData)
        {
            float damagee = skillData.damageType switch
            {
                SkillDamageType.Physical => (strStat.Value + skillData.baseDamage) * skillData.damageMultiplier,
                SkillDamageType.Magical => (intStat.Value + skillData.baseDamage) * skillData.damageMultiplier,
                _ => skillData.baseDamage
            };
            
            bool isCritical = Random.value < criStat.Value;

            if (isCritical)
            {
                damagee *= cDmgStat.Value;
            }

            return new DamageData(damagee, Vector3.zero,Vector3.zero, _owner, isCritical);
        }
    }
}