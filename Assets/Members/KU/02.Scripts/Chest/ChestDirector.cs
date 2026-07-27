using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestDirector : MonoBehaviour
{
    [Header("상자 종류")]
    [SerializeField] private List<ChestData> chestPool = new();

    [Header("생성 설정")]
    [SerializeField] private int stageBudget = 100;
    [SerializeField] private int minimumChestCount = 5;
    [SerializeField] private int maximumChestCount = 12;

    [Header("가격 증가 설정")]
    [SerializeField] private int stageIndex;
    [SerializeField] private float priceIncreasePerStage = 0.25f;

    [Header("랜덤 시드")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int fixedSeed = 12345;

    private System.Random random;

    private void Start()
    {
        int seed = useRandomSeed
            ? Environment.TickCount
            : fixedSeed;

        random = new System.Random(seed);

        SpawnChests();
    }

    private void SpawnChests()
    {
        ChestSpawnPoint[] foundPoints =
            FindObjectsByType<ChestSpawnPoint>(
                FindObjectsSortMode.None
            );

        List<ChestSpawnPoint> availablePoints =
            new List<ChestSpawnPoint>(foundPoints);

        Shuffle(availablePoints);

        int remainingBudget = stageBudget;
        int spawnedCount = 0;

        while (availablePoints.Count > 0 &&
               spawnedCount < maximumChestCount)
        {
            List<ChestData> affordableChests =
                GetAffordableChests(remainingBudget);

            bool reachedMinimum =
                spawnedCount >= minimumChestCount;

            if (affordableChests.Count == 0)
            {
                if (reachedMinimum)
                    break;

                Debug.LogWarning(
                    "최소 상자 개수를 채우기 전에 예산이 부족합니다."
                );

                break;
            }

            ChestData selectedData =
                GetWeightedRandomChest(affordableChests);

            if (selectedData == null)
                break;

            ChestSpawnPoint selectedPoint =
                availablePoints[0];

            availablePoints.RemoveAt(0);

            Chest newChest = Instantiate(
                selectedData.chestPrefab,
                selectedPoint.SpawnPosition,
                selectedPoint.SpawnRotation
            );

            int purchasePrice =
                CalculatePurchasePrice(
                    selectedData.basePurchasePrice
                );

            newChest.Initialize(purchasePrice);

            remainingBudget -= selectedData.spawnCost;
            spawnedCount++;
        }

        Debug.Log(
            $"상자 {spawnedCount}개 생성 / 남은 예산 {remainingBudget}"
        );
    }

    private List<ChestData> GetAffordableChests(
        int remainingBudget
    )
    {
        List<ChestData> result = new();

        foreach (ChestData chestData in chestPool)
        {
            if (chestData == null ||
                chestData.chestPrefab == null)
            {
                continue;
            }

            if (chestData.spawnCost <= remainingBudget)
                result.Add(chestData);
        }

        return result;
    }

    private ChestData GetWeightedRandomChest(
        List<ChestData> candidates
    )
    {
        float totalWeight = 0f;

        foreach (ChestData candidate in candidates)
            totalWeight += candidate.spawnWeight;

        if (totalWeight <= 0f)
            return null;

        double randomValue =
            random.NextDouble() * totalWeight;

        float accumulatedWeight = 0f;

        foreach (ChestData candidate in candidates)
        {
            accumulatedWeight += candidate.spawnWeight;

            if (randomValue <= accumulatedWeight)
                return candidate;
        }

        return candidates[^1];
    }

    private int CalculatePurchasePrice(int basePrice)
    {
        float multiplier =
            1f + stageIndex * priceIncreasePerStage;

        return Mathf.RoundToInt(basePrice * multiplier);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(i + 1);

            (list[i], list[randomIndex]) =
                (list[randomIndex], list[i]);
        }
    }
}