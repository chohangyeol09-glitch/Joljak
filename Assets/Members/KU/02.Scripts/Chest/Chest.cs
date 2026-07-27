using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    [Header("연출")]
    [SerializeField] private Animator animator;

    [Header("아이템 생성 위치")]
    [SerializeField] private Transform itemSpawnPoint;

    [Header("아이템 드롭")]
    [SerializeField] private ItemDropper itemDropper;

    private int purchasePrice;
    private bool isOpened;

    public int PurchasePrice => purchasePrice;
    public bool IsOpened => isOpened;

    public void Initialize(int newPrice)
    {
        purchasePrice = Mathf.Max(0, newPrice);
        isOpened = false;
    }

    public bool TryOpen(PlayerWallet wallet)
    {
        if (isOpened)
            return false;

        if (wallet == null)
            return false;

        if (!wallet.TrySpend(purchasePrice))
            return false;

        OpenChest();

        return true;
    }

    private void OpenChest()
    {
        isOpened = true;

        if (animator != null)
            animator.SetTrigger("Open");

        if (itemDropper != null)
        {
            itemDropper.DropItem(
                itemSpawnPoint != null
                    ? itemSpawnPoint.position
                    : transform.position + Vector3.up
            );
        }
    }
}