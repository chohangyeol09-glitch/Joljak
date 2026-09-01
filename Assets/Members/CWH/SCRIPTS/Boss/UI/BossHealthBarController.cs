using System.Collections;
using Boss.Core;
using Boss.Phase;
using UnityEngine;
using UnityEngine.UIElements;

namespace Boss.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class BossHealthBarController : MonoBehaviour
    {
        [SerializeField] private GameObject bossTarget;
        [SerializeField] private string bossDisplayName = "여기에 보스이름";
        [SerializeField] private BossStatsSO stats;

        [Header("Damage Delay Bar")]
        [SerializeField] private float slowBarDelay = 0.4f;
        [SerializeField] private float slowBarDuration = 0.6f;

        [Header("Crack Shatter Punch")]
        [SerializeField] private float shatterPunchScale = 1.2f;
        [SerializeField] private float shatterBurstUndershoot = 0.85f;
        [SerializeField] private float shatterGrowDuration = 0.12f;
        [SerializeField] private float shatterBurstDuration = 0.08f;
        [SerializeField] private float shatterSettleDuration = 0.15f;
        [SerializeField] private float shatterFlashFadeDuration = 0.3f;
        [SerializeField] private float shatterJitterAmount = 3f;

        [Header("Chain Wrap Lock")]
        [SerializeField] private float wrapStartDelay = 0.45f;
        [SerializeField] private int wrapLineCount = 6;
        [SerializeField] private float wrapLineWidthPercent = 34f;
        [SerializeField] private float wrapHorizontalOvershoot = 6f;
        [SerializeField] private float wrapLineFadeDuration = 0.12f;
        [SerializeField] private float wrapLineStagger = 0.13f;

        private const string ActivePhaseClass = "phase-chip--active";
        private const int SegmentCount = 3;

        private IDamageable health;
        private IBossPhaseController phaseController;

        private VisualElement[] realFills;
        private VisualElement[] slowFills;
        private VisualElement[] flashes;
        private VisualElement[][] wrapLines;
        private VisualElement[] segments;
        private Label healthPercentText;
        private Label bossNameText;
        private VisualElement[] phaseChips;

        private float[] currentSlowRatio;
        private Coroutine[] slowBarRoutines;
        private float[] segmentBoundaries;
        private int previousPhase = -1;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            realFills = new VisualElement[SegmentCount];
            slowFills = new VisualElement[SegmentCount];
            flashes = new VisualElement[SegmentCount];
            wrapLines = new VisualElement[SegmentCount][];
            segments = new VisualElement[SegmentCount];
            currentSlowRatio = new float[SegmentCount];
            slowBarRoutines = new Coroutine[SegmentCount];

            for (int i = 0; i < SegmentCount; i++)
            {
                segments[i] = root.Q<VisualElement>($"PhaseSegment{i}");
                realFills[i] = root.Q<VisualElement>($"RealFill{i}");
                slowFills[i] = root.Q<VisualElement>($"SlowFill{i}");
                flashes[i] = root.Q<VisualElement>($"Flash{i}");
                currentSlowRatio[i] = 1f;
                wrapLines[i] = CreateWrapLines(segments[i]);
            }

            healthPercentText = root.Q<Label>("HealthPercentText");
            bossNameText = root.Q<Label>("BossNameText");

            phaseChips = new[]
            {
                root.Q<VisualElement>("Phase0"),
                root.Q<VisualElement>("Phase1"),
                root.Q<VisualElement>("Phase2")
            };

            bossNameText.text = bossDisplayName;
            segmentBoundaries = BuildSegmentBoundaries();
        }

        private VisualElement[] CreateWrapLines(VisualElement segment)
        {
            var lines = new VisualElement[wrapLineCount];

            // 첫 띠는 -overshoot%에서, 마지막 띠는 (100+overshoot-폭)%에서 시작해서
            // (100+overshoot)%에 닿도록 균등 배분한다. 양 끝을 세그먼트 경계보다 살짝 더 밀어내서,
            // 회전 때문에 모서리가 못 미치는 것까지 overflow:hidden으로 깔끔하게 잘라낸다.
            float firstStart = -wrapHorizontalOvershoot;
            float lastStart = (100f + wrapHorizontalOvershoot) - wrapLineWidthPercent;
            float step = wrapLineCount > 1 ? (lastStart - firstStart) / (wrapLineCount - 1) : 0f;

            for (int i = 0; i < wrapLineCount; i++)
            {
                var line = new VisualElement();
                line.AddToClassList("wrap-line");

                line.style.left = Length.Percent(firstStart + step * i);
                line.style.width = Length.Percent(wrapLineWidthPercent);

                // 양 끝(첫/마지막) 띠만 회전 없이 수직으로 세워서, 모서리를 빈틈없이 딱 맞게 채운다.
                bool isEdgeLine = i == 0 || i == wrapLineCount - 1;
                if (isEdgeLine)
                {
                    line.style.rotate = new StyleRotate(new Rotate(0f));
                }

                segment.Add(line);
                lines[i] = line;
            }

            return lines;
        }

        private float[] BuildSegmentBoundaries()
        {
            // boundaries[i]는 세그먼트 i가 채워지기 시작하는 상한 체력비율, boundaries[i+1]은 하한
            var thresholds = stats != null ? stats.PhaseHealthThresholds : null;
            var boundaries = new float[SegmentCount + 1];
            boundaries[0] = 1f;

            for (int i = 0; i < SegmentCount - 1; i++)
            {
                boundaries[i + 1] = thresholds != null && i < thresholds.Count ? thresholds[i] : 0f;
            }

            boundaries[SegmentCount] = 0f;
            return boundaries;
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
                previousPhase = phaseController.CurrentPhase;
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
            float overallRatio = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            healthPercentText.text = $"{Mathf.RoundToInt(overallRatio * 100f)}%";

            for (int i = 0; i < SegmentCount; i++)
            {
                // 오른쪽(높은 인덱스) 세그먼트부터 깎이도록, 체력 구간을 역순으로 배정한다.
                int boundaryIndex = SegmentCount - 1 - i;
                float segmentMax = segmentBoundaries[boundaryIndex];
                float segmentMin = segmentBoundaries[boundaryIndex + 1];
                float range = segmentMax - segmentMin;
                float localRatio = range > 0f ? Mathf.Clamp01((overallRatio - segmentMin) / range) : 0f;

                realFills[i].style.width = Length.Percent(localRatio * 100f);

                if (slowBarRoutines[i] != null)
                {
                    StopCoroutine(slowBarRoutines[i]);
                }

                slowBarRoutines[i] = StartCoroutine(CatchUpSlowBar(i, localRatio));
            }
        }

        private IEnumerator CatchUpSlowBar(int index, float targetRatio)
        {
            yield return new WaitForSeconds(slowBarDelay);

            float startRatio = currentSlowRatio[index];
            float elapsed = 0f;

            while (elapsed < slowBarDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / slowBarDuration);
                currentSlowRatio[index] = Mathf.Lerp(startRatio, targetRatio, t);
                slowFills[index].style.width = Length.Percent(currentSlowRatio[index] * 100f);
                yield return null;
            }

            currentSlowRatio[index] = targetRatio;
            slowFills[index].style.width = Length.Percent(currentSlowRatio[index] * 100f);
        }

        private void OnPhaseChanged(int phase)
        {
            for (int i = 0; i < phaseChips.Length; i++)
            {
                phaseChips[i]?.EnableInClassList(ActivePhaseClass, i == phase);
            }

            // 페이즈가 진행(악화)됐을 때만, 방금 다 깨진 구역에 펀치 연출을 재생한다.
            // 세그먼트가 역순으로 배정되므로(오른쪽부터 깎임), 깨지는 구역도 오른쪽부터.
            int justShatteredIndex = SegmentCount - phase;
            if (phase > previousPhase && justShatteredIndex >= 0 && justShatteredIndex < SegmentCount)
            {
                StartCoroutine(ShatterPunch(justShatteredIndex));
            }

            previousPhase = phase;
        }

        private IEnumerator ShatterPunch(int index)
        {
            var segment = segments[index];
            var flash = flashes[index];

            // 1) 부풀어 오르는 단계 (천천히 커짐 + 흔들리기 시작 - 압력이 쌓이는 느낌)
            yield return LerpScaleWithJitter(segment, 1f, shatterPunchScale, shatterGrowDuration, shatterJitterAmount * 0.4f);

            // 2) 터지는 순간: 하얀 플래시 즉시 점등 + 원래 크기보다 작게 훅 줄어듦 + 흔들림 최대
            flash.style.opacity = 1f;
            yield return LerpScaleWithJitter(segment, shatterPunchScale, shatterBurstUndershoot, shatterBurstDuration, shatterJitterAmount);

            // 3) 정착: 흔들림 멈추고 원래 크기로, 플래시는 서서히 페이드아웃
            segment.style.translate = new StyleTranslate(new Translate(0, 0));
            StartCoroutine(FadeOut(flash, shatterFlashFadeDuration));
            yield return LerpScale(segment, shatterBurstUndershoot, 1f, shatterSettleDuration);

            segment.style.scale = new StyleScale(new Scale(Vector3.one));

            // 4) 잠금: 터진 직후 바로 감기지 않고 잠깐 텀을 두었다가, 붕대를 감듯 넓은 띠들이
            //    한쪽부터 순서대로 돌아 들어와 자리 잡는다.
            //    한 번 잠기면(페이즈는 되돌아가지 않으므로) 계속 남아있어야 해서 다시 지우지 않는다.
            yield return new WaitForSeconds(wrapStartDelay);
            yield return WrapChain(index);
        }

        private IEnumerator WrapChain(int index)
        {
            var lines = wrapLines[index];

            // 오른쪽(마지막 인덱스)부터 순서대로 나타나게 한다.
            // 회전 스윙 없이 이미 정해진 자리에서 짧게 페이드만 하기 때문에, 나타난 부분은
            // 항상 겹침 계산이 정확한 최종 위치라서 애니메이션 도중에도 빈틈이 안 생긴다.
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                StartCoroutine(FadeIn(lines[i], wrapLineFadeDuration));
                yield return new WaitForSeconds(wrapLineStagger);
            }
        }

        private IEnumerator FadeIn(VisualElement element, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                element.style.opacity = t;
                yield return null;
            }

            element.style.opacity = 1f;
        }

        private IEnumerator LerpScale(VisualElement element, float from, float to, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float scale = Mathf.Lerp(from, to, t);
                element.style.scale = new StyleScale(new Scale(new Vector3(scale, scale, 1f)));
                yield return null;
            }
        }

        private IEnumerator LerpScaleWithJitter(VisualElement element, float from, float to, float duration, float jitterAmount)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float scale = Mathf.Lerp(from, to, t);
                element.style.scale = new StyleScale(new Scale(new Vector3(scale, scale, 1f)));

                float jitterX = Random.Range(-jitterAmount, jitterAmount);
                float jitterY = Random.Range(-jitterAmount, jitterAmount);
                element.style.translate = new StyleTranslate(new Translate(jitterX, jitterY));

                yield return null;
            }
        }

        private IEnumerator FadeOut(VisualElement element, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                element.style.opacity = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }

            element.style.opacity = 0f;
        }
    }
}
