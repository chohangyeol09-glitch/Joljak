using UnityEngine;

public class ShopChoice : MonoBehaviour
{
    [Header("아이템 표시 위치")]
    [SerializeField] private Transform displayPoint;

    [Header("구매한 아이템 생성 위치")]
    [SerializeField] private Transform rewardPoint;

    [Header("상호작용 Collider")]
    [SerializeField] private Collider interactionCollider;

    private Shop ownerShop;
    private ItemData itemData;
    private GameObject displayObject;

    public bool IsAvailable =>
        ownerShop != null &&
        itemData != null &&
        !ownerShop.IsSoldOut;

    public int Price =>
        ownerShop != null
            ? ownerShop.PurchasePrice
            : 0;

    public bool IsSoldOut =>
        ownerShop == null || ownerShop.IsSoldOut;

    public ItemData ItemData => itemData;

    private void Awake()
    {
        if (interactionCollider == null)
            interactionCollider = GetComponent<Collider>();
    }

    public void Initialize(
        Shop shop,
        ItemData newItemData
    )
    {
        ownerShop = shop;
        itemData = newItemData;

        ClearDisplay();

        if (itemData == null)
        {
            DisableChoice();
            return;
        }

        if (itemData.displayPrefab != null &&
            displayPoint != null)
        {
            displayObject = Instantiate(
                itemData.displayPrefab,
                displayPoint.position,
                displayPoint.rotation,
                displayPoint
            );
        }

        SetInteractable(true);
    }

    public bool TryPurchase(PlayerWallet wallet)
    {
        if (ownerShop == null)
            return false;

        return ownerShop.TryPurchase(this, wallet);
    }

    public void SpawnPurchasedItem(
        Transform fallbackSpawnPoint
    )
    {
        if (itemData == null ||
            itemData.pickupPrefab == null)
        {
            Debug.LogWarning(
                $"{name}: 생성할 아이템 프리팹이 없습니다."
            );

            return;
        }

        Vector3 spawnPosition;

        if (rewardPoint != null)
        {
            spawnPosition = rewardPoint.position;
        }
        else if (fallbackSpawnPoint != null)
        {
            spawnPosition = fallbackSpawnPoint.position;
        }
        else
        {
            spawnPosition =
                transform.position + Vector3.up;
        }

        Instantiate(
            itemData.pickupPrefab,
            spawnPosition,
            Quaternion.identity
        );

        ClearDisplay();
    }

    public void SetInteractable(bool value)
    {
        if (interactionCollider != null)
            interactionCollider.enabled = value;
    }

    public void DisableChoice()
    {
        ownerShop = null;
        itemData = null;

        SetInteractable(false);
        ClearDisplay();
    }

    private void ClearDisplay()
    {
        if (displayObject == null)
            return;

        Destroy(displayObject);
        displayObject = null;
    }
}