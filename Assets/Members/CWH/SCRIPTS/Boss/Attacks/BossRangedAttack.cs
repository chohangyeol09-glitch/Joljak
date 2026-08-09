using Boss.Core;
using UnityEngine;

namespace Boss.Attacks
{
    public class BossRangedAttack : MonoBehaviour, IBossAttack
    {
        [SerializeField] private string attackId = "Ranged";
        [SerializeField] private float range = 10f;
        [SerializeField] private float cooldownSeconds = 3f;
        [SerializeField] private GameObject telegraphPrefab;
        [SerializeField] private float telegraphGroundHeight = 0.0001f;
        [SerializeField] private float blinkLeadTime = 1f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform muzzle;

        private BossAttackCooldown cooldown;
        private Vector3 impactPoint;
        private bool impactResolved;

        public string AttackId => attackId;
        public float Range => range;
        public bool IsOnCooldown => !cooldown.IsReady;

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
            impactPoint = context.Target.position;
            impactResolved = false;

            AttackTelegraph telegraph = null;
            if (telegraphPrefab != null)
            {
                var groundPosition = new Vector3(impactPoint.x, telegraphGroundHeight, impactPoint.z);
                var telegraphObject = Instantiate(telegraphPrefab, groundPosition, Quaternion.Euler(90f, 0f, 0f));
                telegraphObject.TryGetComponent(out telegraph);
            }

            var origin = muzzle != null ? muzzle.position : transform.position;
            var launchDirection = muzzle != null ? muzzle.forward : transform.forward;
            var projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);

            if (projectile.TryGetComponent<Missile>(out var missile))
            {
                missile.Launch(impactPoint, launchDirection, telegraph, blinkLeadTime, () => impactResolved = true);
            }
            else
            {
                impactResolved = true;
            }
        }

        public BossAttackStatus Tick(BossAttackContext context)
        {
            if (!impactResolved)
            {
                return BossAttackStatus.Running;
            }

            cooldown.Start();
            return BossAttackStatus.Completed;
        }
    }
}
