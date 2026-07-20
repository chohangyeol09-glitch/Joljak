using Members.CHG.Scripts.CombatSystem;

namespace Members.CHG.Scripts.Agents.StatSystem
{
    public interface IStatModule
    {
        StatSO GetStat(int statIndex);
        bool TryGetStat(int statIndex, out StatSO outStat);
        void AddModifier(int statIndex, object key, float modifier);
        void RemoveModifier(int statIndex, object key);
        float SubscribeStat(int statIndex, StatSO.ValueChangeHandler handler, float defaultValue);
        void UnSubscribeStat(int statIndex, StatSO.ValueChangeHandler handler);
        DamageData CalculateDamage(SkillDataSO skillData);
    }
}