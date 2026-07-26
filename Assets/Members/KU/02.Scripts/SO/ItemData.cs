using UnityEngine;

public enum ItemTier
{
    Common,
    Uncommon,
    Rare,
    Equipment
}

[CreateAssetMenu(fileName = "ItemData", menuName = "KU/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemTier tier;

    [Header("UI")]
    public Sprite icon;

    [Header("상점에 보일 전시용 모델")]
    public GameObject displayPrefab;

    [Header("구매 후 생성될 아이템")]
    public GameObject pickupPrefab;
}