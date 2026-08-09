using CHG.Scripts.CombatSystem;

namespace CHG.Scripts.SkillSystem.SkillDatas
{
    public abstract class DamageSkillDataSO : SkillDataSO
    {
        public DamageType DamageType;
        public int BaseDamage;
        public float DamageMultiplier = 1f;
        
        public DamageSource ToDamageSource() => new DamageSource(DamageType, BaseDamage, DamageMultiplier);
    }
}