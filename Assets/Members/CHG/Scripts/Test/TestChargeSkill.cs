using CHG.Scripts.SkillSystem;
using UnityEngine;

namespace CHG.Scripts.Test
{
    public class TestChargeSkill : ChargeSkill
    {
        protected override void OnChargeStart(GameObject target)
        {
            Debug.Log($"{Data.AssetName} 차징 시작");
        }

        protected override void Fire(GameObject target, float chargeRatio)
        {
            Debug.Log($"{Data.AssetName} 시전, 차징 {chargeRatio:P0}");
        }

        protected override void OnChargeCancel()
        {
            Debug.Log($"{Data.AssetName} 차징 취소");
        }
    }
}