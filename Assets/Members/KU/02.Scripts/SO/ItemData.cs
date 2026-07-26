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
    public Sprite icon;
    public GameObject pickupPrefab;
}