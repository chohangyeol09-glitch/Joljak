using CHG.Scripts.CoreSystem.AnimationSystem;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG.Scripts.SkillSystem.SkillDatas
{
    [CreateAssetMenu(fileName = "Charge shot data", menuName = "Agent/Skill/Charge", order = 0)]
    public class ChargeShotDataSO : DamageSkillDataSO
    {
        public float maxChargeTime = 1.5f;
        public float graceTime = 0.5f;
        public float maxChargeMultiplier = 2f;
        public float minChargeMultiplier = 1f;

        public PoolItemSO BulletItem;

        public AnimParamSO ChargeAnim;
        public AnimParamSO FireAnim;
    }
}