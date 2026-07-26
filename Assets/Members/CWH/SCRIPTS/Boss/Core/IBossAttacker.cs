namespace Boss.Core
{
    public interface IBossAttacker
    {
        bool TryBeginAttack(string attackId, BossAttackContext context);
        BossAttackStatus TickCurrentAttack(BossAttackContext context);
    }
}
