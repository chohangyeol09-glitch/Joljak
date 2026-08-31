using System;
using System.Collections;
using Boss.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Boss.Attacks
{
    enum PillarState
    {
        Dormant,
        Rising,
        Active,
        Destroyed
    }

    public class Pillar : MonoBehaviour, IDamageable
    {
        [Header("이펙트")] [SerializeField] private Transform bossTransform;
        [SerializeField] private BeamLineVisual laser;
        [SerializeField] private ParticleSystem[] particles;
        [SerializeField] private Transform beamAnchor;

        [Header("기둥")] [SerializeField] private float maximumHeight = 5f; // 높이 (주의! 현재 위치에서 추가하는것임 절대값 아님)
        [SerializeField] private float prepDuration = 0.63f; // Rising시 잠시 대기 시간 
        [SerializeField] private float ascendDuration = 0.8f; // 상승되는데 소모되는 시간
        [SerializeField] private float descendDuration = 0.25f; // 하강하는데 소모되는 시간
        [SerializeField] private AnimationCurve ascendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private Collider col; // 콜라이더
        [SerializeField] private NavMeshObstacle navObstacle; // NavMeshObstacle

        [SerializeField] private float jitterAmount = 0.05f; // 떨림의 크기
        [SerializeField] private float jitterFrequency = 25f; // 떨림 속도(빠를수록 잘게 떰)

        [SerializeField] private float maxHealth = 500f; //최대 체력

        [Header("보스 회복")] [SerializeField] private float healAmount = 20f; // 한 번에 회복시키는 양
        [SerializeField] private float healInterval = 3f; // 회복 주기(초)

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;
        public bool IsAvailable => _state == PillarState.Dormant;

        public event Action<float, float> HealthChanged;
        public event Action Died;

        private Vector3 _setPosition;
        private PillarState _state;
        private Coroutine _activeRoutine;
        private Outline _outline;
        private IHealable _bossHealable;
        private float _healElapsed;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            col = GetComponent<Collider>();
            navObstacle = GetComponent<NavMeshObstacle>();
            _outline = GetComponent<Outline>();
            _bossHealable = bossTransform.GetComponentInParent<IHealable>();
            _setPosition = transform.position;
        }

        private void Start()
        {
            _state = PillarState.Dormant;
            col.enabled = false;
            navObstacle.enabled = false;
            _outline.enabled = false;
            foreach (var particle in particles)
            {
                particle.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (_state == PillarState.Active)
            {
                laser?.UpdateBeam(bossTransform.position, beamAnchor.position);

                _healElapsed += Time.deltaTime;
                if (_healElapsed >= healInterval)
                {
                    _healElapsed -= healInterval;
                    _bossHealable?.Heal(healAmount);
                }
            }
        }

        // 상승하는 함수
        [ContextMenu("Activate")]
        public void Activate()
        {
            if (_state != PillarState.Dormant) return;
            _activeRoutine = StartCoroutine(Rising(() =>
            {
                col.enabled = true;
                navObstacle.enabled = true;
            }));
        }

        public void Deactivate()
        {
            if (_activeRoutine != null) StopCoroutine(_activeRoutine);

            StartCoroutine(Descent(() =>
            {
                col.enabled = false;
                navObstacle.enabled = false;
            }));
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (IsDead)
            {
                Died?.Invoke();
                Deactivate();
            }
        }

        private IEnumerator Rising(Action onComplete)
        {
            _state = PillarState.Rising;
            float elapsed = 0f;
            float riseDuration = Mathf.Max(ascendDuration - prepDuration, 0.0001f);
            Vector3 hoverPosition = _setPosition + Vector3.up * maximumHeight;

            while (elapsed < ascendDuration)
            {
                elapsed += Time.deltaTime;

                if (elapsed < prepDuration)
                {
                    transform.position = _setPosition;
                }
                else
                {
                    float t = ascendCurve.Evaluate(Mathf.Clamp01((elapsed - prepDuration) / riseDuration));
                    Vector3 basePos = Vector3.Lerp(_setPosition, hoverPosition, t);

                    float noiseX = (Mathf.PerlinNoise(Time.time * jitterFrequency, 0f) - 0.5f);
                    float noiseZ = (Mathf.PerlinNoise(0f, Time.time * jitterFrequency) - 0.5f);
                    Vector3 jitter = new Vector3(noiseX, 0f, noiseZ) * jitterAmount;

                    transform.position = basePos + jitter;
                }

                yield return null;
            }

            _state = PillarState.Active;
            _healElapsed = 0f;
            foreach (var particle in particles)
            {
                particle.gameObject.SetActive(true);
            }
            laser?.ShowBeam(bossTransform.position, beamAnchor.position);
            _outline.enabled = true;
            onComplete?.Invoke();
            yield return null;
        }

        private IEnumerator Descent(Action onComplete)
        {
            _state = PillarState.Destroyed;
            foreach (var particle in particles)
            {
                particle.gameObject.SetActive(false);
            }
            laser?.HideBeam();
            _outline.enabled = false;
            float elapsed = 0f;
            Vector3 hoverPosition = _setPosition + Vector3.up * maximumHeight;

            while (elapsed < descendDuration)
            {
                elapsed += Time.deltaTime;

                float t = ascendCurve.Evaluate(Mathf.Clamp01(elapsed / descendDuration));
                Vector3 basePos = Vector3.Lerp(hoverPosition, _setPosition, t);

                float noiseX = (Mathf.PerlinNoise(Time.time * jitterFrequency, 0f) - 0.5f);
                float noiseZ = (Mathf.PerlinNoise(0f, Time.time * jitterFrequency) - 0.5f);
                Vector3 jitter = new Vector3(noiseX, 0f, noiseZ) * jitterAmount;

                transform.position = basePos + jitter;

                yield return null;
            }

            _state = PillarState.Dormant;
            CurrentHealth = maxHealth;
            onComplete?.Invoke();
            yield return null;
        }

        #region Debuging

        [ContextMenu("Take Damage 500")]
        private void DebuggingDamage()
        {
            TakeDamage(500f);
        }

        #endregion
    }
}