using System.Collections;
using Boss.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Boss.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class BossHealthBarController : MonoBehaviour
    {
        [SerializeField] private GameObject bossTarget;
        [SerializeField] private string bossDisplayName = "여기에 보스이름";

        [Header("Damage Delay Bar")]
        [SerializeField] private float slowBarDelay = 0.4f;
        [SerializeField] private float slowBarDuration = 0.6f;

        private const string ActivePhaseClass = "phase-chip--active";

        private IDamageable health;
        private IBossPhaseController phaseController;

        private VisualElement realBarFill;
        private VisualElement slowBarFill;
        private Label healthPercentText;
        private Label bossNameText;
        private VisualElement[] phaseChips;

        private float currentSlowRatio = 1f;
        private Coroutine slowBarRoutine;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            realBarFill = root.Q<VisualElement>("RealBarFill");
            slowBarFill = root.Q<VisualElement>("SlowBarFill");
            healthPercentText = root.Q<Label>("HealthPercentText");
            bossNameText = root.Q<Label>("BossNameText");

            phaseChips = new[]
            {
                root.Q<VisualElement>("Phase0"),
                root.Q<VisualElement>("Phase1"),
                root.Q<VisualElement>("Phase2")
            };

            bossNameText.text = bossDisplayName;
        }

        private void OnEnable()
        {
            if (bossTarget == null)
            {
                return;
            }

            health = bossTarget.GetComponent<IDamageable>();
            phaseController = bossTarget.GetComponent<IBossPhaseController>();

            if (health != null)
            {
                health.HealthChanged += OnHealthChanged;
            }

            if (phaseController != null)
            {
                phaseController.PhaseChanged += OnPhaseChanged;
            }
        }

        private void Start()
        {
            // 보스의 Awake가 CurrentHealth를 초기화한 뒤에 읽도록 Start에서 최초 동기화한다.
            if (health != null)
            {
                OnHealthChanged(health.CurrentHealth, health.MaxHealth);
            }

            if (phaseController != null)
            {
                OnPhaseChanged(phaseController.CurrentPhase);
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.HealthChanged -= OnHealthChanged;
            }

            if (phaseController != null)
            {
                phaseController.PhaseChanged -= OnPhaseChanged;
            }
        }

        private void OnHealthChanged(float current, float max)
        {
            float ratio = max > 0f ? Mathf.Clamp01(current / max) : 0f;

            realBarFill.style.width = Length.Percent(ratio * 100f);
            healthPercentText.text = $"{Mathf.RoundToInt(ratio * 100f)}%";

            if (slowBarRoutine != null)
            {
                StopCoroutine(slowBarRoutine);
            }

            slowBarRoutine = StartCoroutine(CatchUpSlowBar(ratio));
        }

        private IEnumerator CatchUpSlowBar(float targetRatio)
        {
            yield return new WaitForSeconds(slowBarDelay);

            float startRatio = currentSlowRatio;
            float elapsed = 0f;

            while (elapsed < slowBarDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / slowBarDuration);
                currentSlowRatio = Mathf.Lerp(startRatio, targetRatio, t);
                slowBarFill.style.width = Length.Percent(currentSlowRatio * 100f);
                yield return null;
            }

            currentSlowRatio = targetRatio;
            slowBarFill.style.width = Length.Percent(currentSlowRatio * 100f);
        }

        private void OnPhaseChanged(int phase)
        {
            for (int i = 0; i < phaseChips.Length; i++)
            {
                phaseChips[i]?.EnableInClassList(ActivePhaseClass, i == phase);
            }
        }
    }
}
