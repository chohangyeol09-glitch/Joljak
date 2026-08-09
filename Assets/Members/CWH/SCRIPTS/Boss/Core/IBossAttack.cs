namespace Boss.Core
{
    public interface IBossAttack
    {
        string AttackId { get; }
        float Range { get; }
        bool IsOnCooldown { get; }

        void Begin(BossAttackContext context);
        BossAttackStatus Tick(BossAttackContext context);
    }
}
