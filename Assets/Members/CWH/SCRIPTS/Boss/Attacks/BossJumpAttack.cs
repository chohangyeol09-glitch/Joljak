using System.Collections;
using Boss.Core;
using DG.Tweening;
using UnityEngine;

namespace Boss.Attacks
{
    public class BossJumpAttack : MonoBehaviour, IBossAttack
    {
        [SerializeField] private string attackId = "Jump";
        [SerializeField] private float range = 6f;
        [SerializeField] private float cooldownSeconds = 5f;
        [SerializeField] private float damage = 30f;
        [SerializeField] private float damageRadius = 3f;
        [SerializeField] private LayerMask damageLayers = ~0;

        [SerializeField] private float jumpHeight = 5f;
        [SerializeField] private float prepDuration = 0.63f;
        [SerializeField] private float ascendDuration = 0.8f;
        [SerializeField] private float descendDuration = 0.25f;
        [SerializeField] private AnimationCurve ascendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve descendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Squash & Stretch (없으면 비워도 됨, 비우면 본체 기준)")]
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Vector3 landingSquash = new(0.25f, -0.3f, 0.25f);
        [SerializeField] private float squashStretchDuration = 0.25f;

        [SerializeField] private GameObject telegraphPrefab;
        [SerializeField] private float telegraphGroundHeight = 0.0001f;
        [SerializeField] private float hoverDuration = 2f;
        [SerializeField] private float blinkLeadTime = 1f;

        [SerializeField] private GameObject landingEffectPrefab;
        [SerializeField] private float landingEffectLifetime = 3f;

        [Header("Animation")]
        [SerializeField] private string ascendTrigger = "JumpAscend";
        [SerializeField] private string landTrigger = "JumpLand";

        private BossAttackCooldown cooldown;
        private IMovable mover;
        private IBossAnimator animator;
        private bool isComplete;

        public string AttackId => attackId;
        public float Range => range;
        public bool IsAttackable => cooldown.IsReady;

        private Transform VisualRoot => visualRoot != null ? visualRoot : transform;

        private void Awake()
        {
            cooldown = new BossAttackCooldown(cooldownSeconds);
            mover = GetComponent<IMovable>();
            animator = GetComponent<IBossAnimator>();
        }

        private void Update()
        {
            cooldown.Tick(Time.deltaTime);
        }

        public void Begin(BossAttackContext context)
        {
            isComplete = false;
            mover.SuspendAutoMovement();
            StartCoroutine(JumpRoutine(context.Self.position, context.Target));
        }

        public BossAttackStatus Tick(BossAttackContext context)
        {
            return isComplete ? BossAttackStatus.Completed : BossAttackStatus.Running;
        }

        private IEnumerator JumpRoutine(Vector3 origin, Transform target)
        {
            // 1단계: 준비 동작 후 제자리에서 위로 솟구침
            animator?.PlayTrigger(ascendTrigger);
            Vector3 hoverPosition = origin + Vector3.up * jumpHeight;
            float riseDuration = Mathf.Max(ascendDuration - prepDuration, 0.0001f);
            float elapsed = 0f;
            while (elapsed < ascendDuration)
            {
                elapsed += Time.deltaTime;

                if (elapsed < prepDuration)
                {
                    // 준비 동작 구간: 실제로 뜨기 전까지는 제자리 유지
                    transform.position = origin;
                }
                else
                {
                    float t = ascendCurve.Evaluate(Mathf.Clamp01((elapsed - prepDuration) / riseDuration));
                    transform.position = Vector3.Lerp(origin, hoverPosition, t);
                }

                yield return null;
            }

            transform.position = hoverPosition;

            // 2단계: 공중에 뜬 채로 대기 - 경고판이 플레이어를 따라다니다가, 마지막 구간엔 고정 + 깜빡임
            GameObject telegraphObject = null;
            AttackTelegraph telegraph = null;
            Vector3 landingPoint = target.position;

            if (telegraphPrefab != null)
            {
                Vector3 groundPosition = GroundPosition(landingPoint);
                telegraphObject = Instantiate(telegraphPrefab, groundPosition, Quaternion.Euler(90f, 0f, 0f));
                telegraphObject.TryGetComponent(out telegraph);
            }

            float followDuration = Mathf.Max(hoverDuration - blinkLeadTime, 0f);
            elapsed = 0f;
            while (elapsed < followDuration)
            {
                elapsed += Time.deltaTime;
                landingPoint = target.position;
                if (telegraphObject != null)
                {
                    telegraphObject.transform.position = GroundPosition(landingPoint);
                }

                yield return null;
            }

            // 착지 지점 확정 (더 이상 플레이어를 따라가지 않음)
            landingPoint = target.position;
            if (telegraphObject != null)
            {
                telegraphObject.transform.position = GroundPosition(landingPoint);
            }

            telegraph?.StartBlinking(blinkLeadTime);

            elapsed = 0f;
            while (elapsed < blinkLeadTime)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (telegraphObject != null)
            {
                Destroy(telegraphObject);
            }

            // 3단계: 확정된 착지 지점으로 가속하며 내리찍기
            animator?.PlayTrigger(landTrigger);
            Vector3 descendStart = transform.position;
            elapsed = 0f;
            while (elapsed < descendDuration)
            {
                elapsed += Time.deltaTime;
                float t = descendCurve.Evaluate(Mathf.Clamp01(elapsed / descendDuration));
                transform.position = Vector3.Lerp(descendStart, landingPoint, t);
                yield return null;
            }

            transform.position = landingPoint;
            VisualRoot.DOPunchScale(landingSquash, squashStretchDuration, 4, 1f);
            ApplyLandingDamage(landingPoint);
            SpawnLandingEffect(landingPoint);
            mover.ResumeAutoMovement(landingPoint);
            cooldown.Start();
            isComplete = true;
        }

        private Vector3 GroundPosition(Vector3 point)
        {
            return new Vector3(point.x, telegraphGroundHeight, point.z);
        }

        private void SpawnLandingEffect(Vector3 position)
        {
            if (landingEffectPrefab == null)
            {
                return;
            }

            var effect = Instantiate(landingEffectPrefab, position, Quaternion.identity);
            Destroy(effect, landingEffectLifetime);
        }

        private void ApplyLandingDamage(Vector3 center)
        {
            var hits = Physics.OverlapSphere(center, damageRadius, damageLayers);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}
