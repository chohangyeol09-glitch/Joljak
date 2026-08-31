using Boss.Core;
using UnityEngine;

namespace Boss.Attacks
{
    public class BossMeleeAttack : MonoBehaviour, IBossAttack
    {
        [SerializeField] private string attackId = "Melee";
        [SerializeField] private float range = 2.5f;
        [SerializeField] private float damage = 25f;
        [SerializeField] private float windupSeconds = 0.5f;
        [SerializeField] private float cooldownSeconds = 2f;

        private float windupTimer;
        private BossAttackCooldown cooldown;

        public string AttackId => attackId;
        public float Range => range;
        public bool IsAttackable => cooldown.IsReady;

        private void Awake()
        {
            cooldown = new BossAttackCooldown(cooldownSeconds);
        }

        private void Update()
        {
            cooldown.Tick(Time.deltaTime);
        }

        public void Begin(BossAttackContext context)
        {
            windupTimer = windupSeconds;
        }

        public BossAttackStatus Tick(BossAttackContext context)
        {
            windupTimer -= Time.deltaTime;
            if (windupTimer > 0f)
            {
                return BossAttackStatus.Running;
            }

            var target = context.Target != null ? context.Target.GetComponent<IDamageable>() : null;
            target?.TakeDamage(damage);

            cooldown.Start();
            return BossAttackStatus.Completed;
        }
    }
}
