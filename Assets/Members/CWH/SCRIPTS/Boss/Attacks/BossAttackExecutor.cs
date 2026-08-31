using Boss.Core;
using UnityEngine;

namespace Boss.Attacks
{
    public class BossAttackExecutor : MonoBehaviour, IBossAttacker
    {
        private IBossAttack[] attacks;
        private IBossAttack currentAttack;
        private IBossAnimator animator;

        public bool IsAttacking => currentAttack != null;

        private void Awake()
        {
            attacks = GetComponents<IBossAttack>();
            animator = GetComponent<IBossAnimator>();
        }

        public bool TryBeginAttack(string attackId, BossAttackContext context)
        {
            var attack = FindAttack(attackId);
            if (attack == null || !attack.IsAttackable)
            {
                return false;
            }

            currentAttack = attack;
            currentAttack.Begin(context);
            animator?.PlayAttack(attackId);
            return true;
        }

        public BossAttackStatus TickCurrentAttack(BossAttackContext context)
        {
            if (currentAttack == null)
            {
                return BossAttackStatus.Failed;
            }

            var status = currentAttack.Tick(context);
            if (status != BossAttackStatus.Running)
            {
                currentAttack = null;
            }

            return status;
        }

        private IBossAttack FindAttack(string attackId)
        {
            foreach (var attack in attacks)
            {
                if (attack.AttackId == attackId)
                {
                    return attack;
                }
            }

            return null;
        }
    }
}
