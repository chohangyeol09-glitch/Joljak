using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ChestGrade
{
    Common,
    Rare,
    Equipment
}

[Serializable]
public class ChestDropSlot
{
    [Header("등급 설정")]
    public ChestGrade grade;

    [Tooltip("이 등급에서 떨어질 상자 데이터")]
    public ChestData chestData;

    [Tooltip("상자가 떨어지는 시간 간격입니다.")]
    [Min(1f)]
    public float intervalSeconds = 60f;

    [Tooltip("UI에 표시할 상자 등급 이미지")]
    public Sprite gradeIcon;

    [Header("UI 연결")]
    public Image gradeImage;

    public TMP_Text remainingTimeText;

    public Scrollbar timeScrollbar;

    [Header("낙하 연출")]
    [Tooltip("해당 등급 UI 한 줄 전체를 연결합니다.")]
    public RectTransform uiRoot;

    [Tooltip("상자 낙하 순간 UI가 커지는 크기")]
    [Range(1f, 2f)]
    public float dropEffectScale = 1.18f;

    [Tooltip("UI가 커지는 시간")]
    [Min(0.01f)]
    public float scaleUpDuration = 0.15f;

    [Tooltip("DROP! 상태 유지 시간")]
    [Min(0f)]
    public float holdDuration = 0.25f;

    [Tooltip("UI가 원래 크기로 돌아오는 시간")]
    [Min(0.01f)]
    public float scaleDownDuration = 0.2f;

    [HideInInspector]
    public float remainingTime;

    [HideInInspector]
    public float retryRemainingTime;

    [HideInInspector]
    public bool isPlayingDropEffect;
}

public class ChestDropManager : MonoBehaviour
{
    [Header("상자 낙하 관리자")]
    [SerializeField]
    private ChestDirector chestDirector;

    [Header("등급별 타이머 3개")]
    [SerializeField]
    private ChestDropSlot[] dropSlots =
        new ChestDropSlot[3];

    [Header("시작 설정")]
    [Tooltip("켜면 게임 시작 직후 각 등급의 상자가 바로 떨어집니다.")]
    [SerializeField]
    private bool dropImmediatelyOnStart;

    [Header("생성 실패 재시도")]
    [Tooltip("낙하 가능한 위치를 못 찾았을 때 다시 시도하는 시간입니다.")]
    [SerializeField]
    private float retryDelay = 1f;

    private void Start()
    {
        if (chestDirector == null)
        {
            chestDirector =
                FindFirstObjectByType<ChestDirector>();
        }

        InitializeSlots();
    }

    private void Update()
    {
        for (int i = 0; i < dropSlots.Length; i++)
        {
            UpdateSlot(dropSlots[i]);
        }
    }

    private void InitializeSlots()
    {
        foreach (ChestDropSlot slot in dropSlots)
        {
            if (slot == null)
                continue;

            slot.remainingTime =
                dropImmediatelyOnStart
                    ? 0f
                    : slot.intervalSeconds;

            slot.retryRemainingTime = 0f;

            slot.isPlayingDropEffect = false;

            // 등급 이미지 설정
            if (slot.gradeImage != null)
            {
                slot.gradeImage.sprite =
                    slot.gradeIcon;

                slot.gradeImage.enabled =
                    slot.gradeIcon != null;
            }

            // Scrollbar 초기 설정
            if (slot.timeScrollbar != null)
            {
                slot.timeScrollbar.interactable = false;

                slot.timeScrollbar.value = 0f;

                slot.timeScrollbar.size = 1f;
            }

            UpdateSlotUI(slot);
        }
    }

    private void UpdateSlot(
        ChestDropSlot slot)
    {
        if (slot == null)
            return;

        // DROP! 연출 중에는
        // 타이머 및 UI 업데이트 중지
        if (slot.isPlayingDropEffect)
            return;

        if (slot.chestData == null ||
            slot.intervalSeconds <= 0f)
        {
            SetInvalidUI(slot);
            return;
        }

        // 아직 시간이 남아있는 경우
        if (slot.remainingTime > 0f)
        {
            slot.remainingTime -=
                Time.deltaTime;

            slot.remainingTime =
                Mathf.Max(
                    0f,
                    slot.remainingTime
                );

            UpdateSlotUI(slot);

            return;
        }

        // 이전 생성 시도에 실패했다면
        // 일정 시간 후 다시 시도
        if (slot.retryRemainingTime > 0f)
        {
            slot.retryRemainingTime -=
                Time.deltaTime;

            slot.retryRemainingTime =
                Mathf.Max(
                    0f,
                    slot.retryRemainingTime
                );

            UpdateSlotUI(slot);

            return;
        }

        // 시간이 다 됐으므로
        // 실제 상자 생성 시도
        TryDropChest(slot);
    }

    private void TryDropChest(
        ChestDropSlot slot)
    {
        if (chestDirector == null)
        {
            Debug.LogWarning(
                $"{name}: ChestDirector가 없습니다."
            );

            slot.retryRemainingTime =
                retryDelay;

            return;
        }

        bool dropSucceeded =
            chestDirector.TryDropChest(
                slot.chestData
            );

        if (dropSucceeded)
        {
            Debug.Log(
                $"{slot.grade} 등급 상자가 떨어졌습니다."
            );

            // 다음 상자 시간 초기화
            slot.remainingTime =
                slot.intervalSeconds;

            slot.retryRemainingTime = 0f;

            // 낙하 UI 연출 실행
            StartCoroutine(
                PlayDropEffect(slot)
            );
        }
        else
        {
            Debug.LogWarning(
                $"{slot.grade} 등급 상자가 " +
                $"떨어질 위치를 찾지 못했습니다."
            );

            // 타이머는 0으로 유지하고
            // 잠시 후 다시 생성 시도
            slot.remainingTime = 0f;

            slot.retryRemainingTime =
                retryDelay;

            UpdateSlotUI(slot);
        }
    }

    private IEnumerator PlayDropEffect(
        ChestDropSlot slot)
    {
        if (slot == null)
            yield break;

        if (slot.isPlayingDropEffect)
            yield break;

        slot.isPlayingDropEffect = true;

        RectTransform root =
            slot.uiRoot;

        Vector3 originalScale =
            root != null
                ? root.localScale
                : Vector3.one;

        // -----------------------------
        // 1. DROP! 표시
        // -----------------------------

        if (slot.remainingTimeText != null)
        {
            slot.remainingTimeText.text =
                "DROP!";
        }

        // 상자가 떨어진 순간
        // 바를 한 번 꽉 채움
        if (slot.timeScrollbar != null)
        {
            slot.timeScrollbar.size = 1f;
            slot.timeScrollbar.value = 0f;
        }

        // -----------------------------
        // 2. UI 확대
        // -----------------------------

        if (root != null)
        {
            float timer = 0f;

            while (timer <
                   slot.scaleUpDuration)
            {
                timer +=
                    Time.unscaledDeltaTime;

                float t =
                    Mathf.Clamp01(
                        timer /
                        slot.scaleUpDuration
                    );

                float easedT =
                    EaseOutBack(t);

                float scale =
                    Mathf.Lerp(
                        1f,
                        slot.dropEffectScale,
                        easedT
                    );

                root.localScale =
                    originalScale * scale;

                yield return null;
            }

            root.localScale =
                originalScale *
                slot.dropEffectScale;
        }

        // -----------------------------
        // 3. DROP! 잠깐 유지
        // -----------------------------

        if (slot.holdDuration > 0f)
        {
            yield return
                new WaitForSecondsRealtime(
                    slot.holdDuration
                );
        }

        // -----------------------------
        // 4. 원래 크기로 복귀
        // -----------------------------

        if (root != null)
        {
            float timer = 0f;

            Vector3 startScale =
                root.localScale;

            while (timer <
                   slot.scaleDownDuration)
            {
                timer +=
                    Time.unscaledDeltaTime;

                float t =
                    Mathf.Clamp01(
                        timer /
                        slot.scaleDownDuration
                    );

                // 뒤로 갈수록 자연스럽게 감속
                float easedT =
                    1f -
                    Mathf.Pow(
                        1f - t,
                        3f
                    );

                root.localScale =
                    Vector3.Lerp(
                        startScale,
                        originalScale,
                        easedT
                    );

                yield return null;
            }

            root.localScale =
                originalScale;
        }

        // -----------------------------
        // 5. 다시 일반 타이머 UI로
        // -----------------------------

        slot.isPlayingDropEffect = false;

        UpdateSlotUI(slot);
    }

    private void UpdateSlotUI(
        ChestDropSlot slot)
    {
        if (slot == null)
            return;

        // 연출 중에는 DROP!을 유지하기 위해
        // UI를 덮어쓰지 않음
        if (slot.isPlayingDropEffect)
            return;

        float displayTime =
            Mathf.Max(
                0f,
                slot.remainingTime
            );

        // 시간 Text
        if (slot.remainingTimeText != null)
        {
            slot.remainingTimeText.text =
                FormatTime(displayTime);
        }

        // Scrollbar
        if (slot.timeScrollbar != null)
        {
            float remainingRatio = 0f;

            if (slot.intervalSeconds > 0f)
            {
                remainingRatio =
                    displayTime /
                    slot.intervalSeconds;
            }

            remainingRatio =
                Mathf.Clamp01(
                    remainingRatio
                );

            /*
             * 남은 시간이 많을수록
             * Scrollbar Size가 큼
             *
             * 60초 / 60초 = 1.0
             * 30초 / 60초 = 0.5
             *  0초 / 60초 = 0.0
             */

            slot.timeScrollbar.size =
                remainingRatio;

            slot.timeScrollbar.value = 0f;
        }
    }

    private void SetInvalidUI(
        ChestDropSlot slot)
    {
        if (slot.remainingTimeText != null)
        {
            slot.remainingTimeText.text =
                "--:--";
        }

        if (slot.timeScrollbar != null)
        {
            slot.timeScrollbar.size = 0f;
        }
    }

    private string FormatTime(
        float seconds)
    {
        int totalSeconds =
            Mathf.CeilToInt(
                Mathf.Max(
                    0f,
                    seconds
                )
            );

        int minutes =
            totalSeconds / 60;

        int remainingSeconds =
            totalSeconds % 60;

        return
            $"{minutes:00}:{remainingSeconds:00}";
    }

    private float EaseOutBack(
        float t)
    {
        const float c1 =
            1.70158f;

        const float c3 =
            c1 + 1f;

        return
            1f +
            c3 *
            Mathf.Pow(
                t - 1f,
                3f
            ) +
            c1 *
            Mathf.Pow(
                t - 1f,
                2f
            );
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (dropSlots == null)
        {
            dropSlots =
                new ChestDropSlot[3];
        }

        if (dropSlots.Length != 3)
        {
            Array.Resize(
                ref dropSlots,
                3
            );
        }

        retryDelay =
            Mathf.Max(
                0.1f,
                retryDelay
            );
    }

#endif
}