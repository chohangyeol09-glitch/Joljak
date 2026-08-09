using UnityEngine;

[CreateAssetMenu(fileName = "ChestData",menuName = "KU/Chest Data")]
public class ChestData : ScriptableObject
{
    [Header("상자 프리팹")]
    public Chest chestPrefab;

    [Header("플레이어가 지불할 기본 가격")]
    [Min(0)]
    public int basePurchasePrice = 25;

    [Header("스테이지 생성 예산")]
    [Min(1)]
    public int spawnCost = 10;

    [Header("선택 확률 가중치")]
    [Min(0f)]
    public float spawnWeight = 1f;
}