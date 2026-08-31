using System.Collections;
using Boss.Core;
using UnityEngine;

namespace Boss.Attacks
{
    public class BossLaserAttack : MonoBehaviour, IBossAttack
    {
        [SerializeField] private string attackId = "Laser";
        [SerializeField] private float range = 15f;
        [SerializeField] private float cooldownSeconds = 8f;

        [Header("Timing")]
        [SerializeField] private float chargeDuration = 3f;
        [SerializeField] private float fireDuration = 1f;

        [Header("Damage")]
        [SerializeField] private float damagePerTick = 5f;
        [SerializeField] private int maxTicks = 20;
        [SerializeField] private float beamRadius = 0.3f;
        [SerializeField] private float beamMaxDistance = 30f;
        [SerializeField] private LayerMask damageLayers = ~0;

        [Header("Visual")]
        [SerializeField] private Transform muzzle;
        [SerializeField] private LaserBeamVisual beamVisual;

        private BossAttackCooldown cooldown;
        private bool isComplete;

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
            isComplete = false;

            Vector3 origin = muzzle != null ? muzzle.position : context.Self.position;
            Vector3 direction = (context.Target.position - origin).normalized;

            StartCoroutine(LaserRoutine(origin, direction));
        }

        public BossAttackStatus Tick(BossAttackContext context)
        {
            return isComplete ? BossAttackStatus.Completed : BossAttackStatus.Running;
        }

        private IEnumerator LaserRoutine(Vector3 origin, Vector3 direction)
        {
            // 차징 단계: 위치/방향은 이미 고정된 상태로 대기 (차징 이펙트는 애니메이션 이벤트가 담당)
            float chargeElapsed = 0f;
            while (chargeElapsed < chargeDuration)
            {
                chargeElapsed += Time.deltaTime;
                yield return null;
            }

            // 발사 단계: 빔 표시 + 최대 maxTicks회 틱 데미지
            Vector3 endPoint = GetBeamEndPoint(origin, direction);
            beamVisual?.ShowBeam(origin, endPoint);

            float tickInterval = fireDuration / maxTicks;
            float fireElapsed = 0f;
            float tickTimer = tickInterval;

            while (fireElapsed < fireDuration)
            {
                fireElapsed += Time.deltaTime;
                tickTimer += Time.deltaTime;

                if (tickTimer >= tickInterval)
                {
                    tickTimer -= tickInterval;
                    ApplyBeamDamage(origin, direction);
                }

                yield return null;
            }

            beamVisual?.HideBeam();

            cooldown.Start();
            isComplete = true;
        }

        private Vector3 GetBeamEndPoint(Vector3 origin, Vector3 direction)
        {
            if (Physics.SphereCast(origin, beamRadius, direction, out var hit, beamMaxDistance, damageLayers))
            {
                return hit.point;
            }

            return origin + direction * beamMaxDistance;
        }

        private void ApplyBeamDamage(Vector3 origin, Vector3 direction)
        {
            if (Physics.SphereCast(origin, beamRadius, direction, out var hit, beamMaxDistance, damageLayers)
                && hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damagePerTick);
            }
        }
    }
}
