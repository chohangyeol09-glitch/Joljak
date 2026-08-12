using CHG.Scripts.CoreSystem.AnimationSystem;
using UnityEngine;

namespace CHG.Scripts.SkillSystem.SkillDatas
{
    [CreateAssetMenu(fileName = "Heal data", menuName = "Agent/Skill/Heal")]
    public class HealSkillDataSO : SkillDataSO
    {
        public int HealAmount;

        [Range(0f, 1f)] public float HealPercent;

        public AnimParamSO CastAnim;
    }
}