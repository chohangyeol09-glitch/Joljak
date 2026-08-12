using UnityEngine;
using UnityEngine.Events;

public class PlayerSoulInventory : MonoBehaviour
{
    [Header("현재 획득한 영혼")]
    [SerializeField] private int soulCount;

    [Header("영혼 개수 변경 이벤트")]
    [SerializeField] private UnityEvent<int> onSoulCountChanged;

    public int SoulCount => soulCount;

    public void AddSoul(int amount = 1)
    {
        if (amount <= 0)
            return;

        soulCount += amount;

        onSoulCountChanged?.Invoke(soulCount);

        Debug.Log(
            $"영혼 획득! 현재 영혼: {soulCount}"
        );
    }
}