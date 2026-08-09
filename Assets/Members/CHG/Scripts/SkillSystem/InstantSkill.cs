using UnityEngine;

namespace CHG.Scripts.SkillSystem
{
    public abstract class InstantSkill : AbstractSkill
    {
        protected sealed override void Execute(GameObject target)
        {
            StartCooldown();
            Cast(target);
        }

        protected abstract void Cast(GameObject target);
    }
}