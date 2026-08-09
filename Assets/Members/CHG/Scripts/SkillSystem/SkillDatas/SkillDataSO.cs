using CHG.Scripts.Agents;
using CHG.Scripts.CombatSystem;
using DevLib.DatabaseSystem.Runtime;
using UnityEngine;

namespace CHG.Scripts.SkillSystem.SkillDatas
{
    public abstract class SkillDataSO : IndexedAsset
    {
        public AbstractSkill SkillPrefab;
        public PointType PointType = PointType.Muzzle;
        public Sprite Icon;
        public float Cooldown;
        
    }
}