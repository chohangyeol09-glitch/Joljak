namespace Boss.Core
{
    public interface IBossAttack
    {
        string AttackId { get; }
        float Range { get; }
        bool IsAttackable { get; }

        void Begin(BossAttackContext context);
        BossAttackStatus Tick(BossAttackContext context);
    }
}
