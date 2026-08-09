namespace CHG.Scripts.CombatSystem
{
    public enum DamageType
    {
        Physical,
        Magical
    }
    
    public class DamageSource
    {
        public readonly DamageType DamageType;
        public readonly int BaseDamage;
        public readonly float Multiplier;

        public DamageSource(DamageType damageType, int baseDamage, float multiplier = 1f)
        {
            DamageType = damageType;
            BaseDamage = baseDamage;
            Multiplier = multiplier;
        }
    }
}