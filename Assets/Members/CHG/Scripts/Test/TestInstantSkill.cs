using CHG.Scripts.SkillSystem;
using UnityEngine;

namespace CHG.Scripts.Test
{
    public class TestInstantSkill : InstantSkill
    {
        [SerializeField] private float delayTime = 0.4f;
        
        protected override void Cast(GameObject target)
        {
            Debug.Log($"{Data.AssetName} 시전 시작, 쿨타임: {Data.Cooldown}");
            Invoke(nameof(FinishCast), delayTime);
        }

        private void FinishCast()
        {
            Debug.Log($"{Data.AssetName} 시전");
            EndSkill();
        }

        protected override void Cancel()
        {
            CancelInvoke(nameof(FinishCast));
        }
    }
}