using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestDirector : MonoBehaviour
{
    [Header("상자 낙하 구역")]
    [Tooltip("비워두면 씬의 모든 ChestDropZone을 자동으로 찾습니다.")]
    [SerializeField] private List<ChestDropZone> dropZones = new();

    [Header("배치 검사")]
    [SerializeField] private int placementAttempts = 30;

    [Tooltip("기존 상자와 떨어져야 하는 최소 거리")]
    [SerializeField] private float minimumChestDistance = 5f;

    [Tooltip("현재 생성된 상자가 사용하는 레이어")]
    [SerializeField] private LayerMask chestMask;

    [Tooltip("건물과 바위처럼 착지 위치를 막는 레이어")]
    [SerializeField] private LayerMask blockingMask;

    [SerializeField]
    private Vector3 landingCheckHalfExtents =
        new Vector3(0.8f, 1f, 0.8f);

    [Header("가격 증가")]
    [SerializeField] private int stageIndex;
    [SerializeField] private float priceIncreasePerStage = 0.25f;

    [Header("랜덤 시드")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int fixedSeed = 12345;

    [Header("생성된 상자 부모")]
    [SerializeField] private Transform spawnedChestParent;

    private System.Random random;
    private bool isInitialized;

    private void Awake()
    {
        InitializeDirector();
    }

    private void InitializeDirector()
    {
        if (isInitialized)
            return;

        int seed = useRandomSeed
            ? Environment.TickCount
            : fixedSeed;

        random = new System.Random(seed);

        FindDropZonesIfNeeded();

        isInitialized = true;
    }

    public bool TryDropChest(ChestData chestData)
    {
        if (!isInitialized)
            InitializeDirector();

        if (chestData == null)
        {
            Debug.LogWarning(
                $"{name}: 전달받은 ChestData가 없습니다."
            );

            return false;
        }

        if (chestData.chestPrefab == null)
        {
            Debug.LogWarning(
                $"{chestData.name}: Chest Prefab이 없습니다."
            );

            return false;
        }

        if (dropZones.Count == 0)
        {
            Debug.LogWarning(
                $"{name}: ChestDropZone이 없습니다."
            );

            return false;
        }

        bool foundPosition = TryFindDropPosition(
            out Vector3 spawnPosition
        );

        if (!foundPosition)
            return false;

        SpawnChest(
            chestData,
            spawnPosition
        );

        return true;
    }

    private void SpawnChest(
        ChestData chestData,
        Vector3 spawnPosition)
    {
        float randomYRotation =
            (float)random.NextDouble() * 360f;

        Quaternion spawnRotation = Quaternion.Euler(
            0f,
            randomYRotation,
            0f
        );

        Chest newChest = Instantiate(
            chestData.chestPrefab,
            spawnPosition,
            spawnRotation,
            spawnedChestParent
        );

        int purchasePrice = CalculatePurchasePrice(
            chestData.basePurchasePrice
        );

        newChest.Initialize(purchasePrice);
        newChest.BeginDrop();

        Debug.Log(
            $"{chestData.name} 낙하 시작 / 가격: {purchasePrice}"
        );
    }

    private bool TryFindDropPosition(
        out Vector3 spawnPosition)
    {
        spawnPosition = default;

        for (int attempt = 0;
             attempt < placementAttempts;
             attempt++)
        {
            ChestDropZone selectedZone =
                dropZones[random.Next(dropZones.Count)];

            if (selectedZone == null)
                continue;

            bool foundGround =
                selectedZone.TryGetDropPosition(
                    random,
                    out Vector3 candidateSpawnPosition,
                    out Vector3 candidateLandingPosition
                );

            if (!foundGround)
                continue;

            if (IsBlocked(candidateLandingPosition))
                continue;

            if (IsTooCloseToExistingChest(
                    candidateLandingPosition))
            {
                continue;
            }

            spawnPosition = candidateSpawnPosition;
            return true;
        }

        return false;
    }

    private bool IsBlocked(Vector3 landingPosition)
    {
        if (blockingMask.value == 0)
            return false;

        Vector3 checkCenter =
            landingPosition +
            Vector3.up *
            (landingCheckHalfExtents.y + 0.05f);

        return Physics.CheckBox(
            checkCenter,
            landingCheckHalfExtents,
            Quaternion.identity,
            blockingMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private bool IsTooCloseToExistingChest(
        Vector3 landingPosition)
    {
        if (chestMask.value == 0)
            return false;

        Vector3 checkPosition =
            landingPosition + Vector3.up * 0.5f;

        return Physics.CheckSphere(
            checkPosition,
            minimumChestDistance,
            chestMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void FindDropZonesIfNeeded()
    {
        dropZones.RemoveAll(zone => zone == null);

        if (dropZones.Count > 0)
            return;

        ChestDropZone[] foundZones =
            FindObjectsByType<ChestDropZone>(
                FindObjectsSortMode.None
            );

        dropZones.AddRange(foundZones);
    }

    private int CalculatePurchasePrice(int basePrice)
    {
        float multiplier =
            1f +
            stageIndex *
            priceIncreasePerStage;

        return Mathf.RoundToInt(
            basePrice * multiplier
        );
    }

    public void SetStageIndex(int newStageIndex)
    {
        stageIndex = Mathf.Max(0, newStageIndex);
    }
}