namespace Boss.Core
{
    public interface IBossAttacker
    {
        bool IsAttacking { get; }

        bool TryBeginAttack(string attackId, BossAttackContext context);
        BossAttackStatus TickCurrentAttack(BossAttackContext context);
    }
}
