using System.Collections.Generic;
using UnityEngine;

public class SoulDirector : MonoBehaviour
{
    [Header("생성할 영혼")]
    [SerializeField] private Soul soulPrefab;

    [Header("영혼 스폰포인트")]
    [Tooltip("비워두면 씬 안의 모든 SoulSpawnPoint를 자동으로 찾습니다.")]
    [SerializeField] private List<SoulSpawnPoint> spawnPoints = new();

    [Header("생성된 영혼 부모")]
    [SerializeField] private Transform spawnedSoulParent;

    [Header("시작 시 자동 생성")]
    [SerializeField] private bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
            SpawnAllSouls();
    }

    public void SpawnAllSouls()
    {
        if (soulPrefab == null)
        {
            Debug.LogError(
                $"{name}: Soul 프리팹이 연결되지 않았습니다."
            );

            return;
        }

        FindSpawnPointsIfNeeded();

        int spawnedCount = 0;

        foreach (SoulSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint == null)
                continue;

            if (spawnPoint.HasSoul)
                continue;

            spawnPoint.SpawnSoul(
                soulPrefab,
                spawnedSoulParent
            );

            spawnedCount++;
        }

        Debug.Log(
            $"영혼 {spawnedCount}개를 생성했습니다."
        );
    }

    private void FindSpawnPointsIfNeeded()
    {
        spawnPoints.RemoveAll(
            spawnPoint => spawnPoint == null
        );

        if (spawnPoints.Count > 0)
            return;

        SoulSpawnPoint[] foundPoints =
            FindObjectsByType<SoulSpawnPoint>(
                FindObjectsSortMode.None
            );

        spawnPoints.AddRange(foundPoints);
    }
}