using System;
using CHG.Scripts.Agents.StatSystem;
using UnityEngine;

public enum ItemTier
{
    Common,
    Uncommon,
    Rare,
    Equipment
}

[Serializable]
public class ItemStatEffect
{
    public StatSO Stat;
    public float Value;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "KU/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemTier tier;

    [Header("UI")]
    public Sprite icon;

    [Header("������ ���� ���ÿ� ��")]
    public GameObject displayPrefab;

    [Header("���� �� ������ ������")]
    public GameObject pickupPrefab;

    public ItemStatEffect[] statEffects;
}