using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [Header("상자에서 나올 수 있는 아이템")]
    [SerializeField] private List<ItemData> itemPool = new();

    [Header("아이템이 튀어나오는 높이")]
    [SerializeField] private float spawnHeight = 1f;

    public void DropItem(Vector3 spawnPosition)
    {
        if (itemPool == null || itemPool.Count == 0)
        {
            Debug.LogWarning($"{name}: 아이템 목록이 비어 있습니다.");
            return;
        }

        ItemData selectedItem = GetRandomItem();

        if (selectedItem == null)
        {
            Debug.LogWarning($"{name}: 선택된 아이템이 없습니다.");
            return;
        }

        if (selectedItem.pickupPrefab == null)
        {
            Debug.LogWarning(
                $"{selectedItem.itemName}의 pickupPrefab이 비어 있습니다."
            );
            return;
        }

        Vector3 finalPosition =
            spawnPosition + Vector3.up * spawnHeight;

        Instantiate(
            selectedItem.pickupPrefab,
            finalPosition,
            Quaternion.identity
        );
    }

    private ItemData GetRandomItem()
    {
        List<ItemData> validItems = new();

        foreach (ItemData item in itemPool)
        {
            if (item != null && item.pickupPrefab != null)
                validItems.Add(item);
        }

        if (validItems.Count == 0)
            return null;

        int randomIndex = Random.Range(0, validItems.Count);

        return validItems[randomIndex];
    }
}