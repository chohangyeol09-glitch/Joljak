using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [Header("상점에서 나올 수 있는 아이템")]
    [SerializeField] private List<ItemData> itemPool = new();

    [Header("아이템 선택지 3개")]
    [SerializeField]
    private ShopChoice[] choices =
        new ShopChoice[3];

    [Header("가격")]
    [SerializeField] private int defaultPrice = 50;

    [Header("선택지의 RewardPoint가 없을 때 사용")]
    [SerializeField] private Transform fallbackRewardPoint;

    [Header("상점 문 애니메이션")]
    [SerializeField] private Animator animator;

    [SerializeField] private string closeTriggerName = "Close";

    private int purchasePrice;
    private bool isSoldOut;

    public int PurchasePrice => purchasePrice;
    public bool IsSoldOut => isSoldOut;

    private void Awake()
    {
        purchasePrice = defaultPrice;
    }

    private void Start()
    {
        GenerateChoices();
    }

    public void Initialize(int newPrice)
    {
        purchasePrice = Mathf.Max(0, newPrice);
    }

    private void GenerateChoices()
    {
        isSoldOut = false;

        List<ItemData> candidates = new();

        foreach (ItemData item in itemPool)
        {
            if (item == null)
                continue;

            if (item.displayPrefab == null)
            {
                Debug.LogWarning(
                    $"{item.name}: displayPrefab이 없습니다."
                );

                continue;
            }

            if (item.pickupPrefab == null)
            {
                Debug.LogWarning(
                    $"{item.name}: pickupPrefab이 없습니다."
                );

                continue;
            }

            candidates.Add(item);
        }

        Shuffle(candidates);

        if (candidates.Count < choices.Length)
        {
            Debug.LogWarning(
                $"{name}: 상점 아이템이 부족합니다. " +
                $"최소 {choices.Length}개가 필요합니다."
            );
        }

        for (int i = 0; i < choices.Length; i++)
        {
            ShopChoice choice = choices[i];

            if (choice == null)
                continue;

            if (i >= candidates.Count)
            {
                choice.DisableChoice();
                continue;
            }

            choice.Initialize(
                this,
                candidates[i]
            );
        }
    }

    public bool TryPurchase(
        ShopChoice selectedChoice,
        PlayerWallet wallet
    )
    {
        if (isSoldOut)
        {
            Debug.Log("이미 사용한 상점입니다.");
            return false;
        }

        if (selectedChoice == null ||
            wallet == null)
        {
            return false;
        }

        if (!selectedChoice.IsAvailable)
            return false;

        if (!wallet.TrySpend(purchasePrice))
        {
            Debug.Log(
                $"돈이 부족합니다. 필요 금액: {purchasePrice}"
            );

            return false;
        }

        // 돈이 빠진 즉시 상점 잠금
        // 애니메이션보다 먼저 처리해야 중복 구매 방지 가능
        isSoldOut = true;

        foreach (ShopChoice choice in choices)
        {
            if (choice != null)
                choice.SetInteractable(false);
        }

        selectedChoice.SpawnPurchasedItem(
            fallbackRewardPoint
        );

        if (animator != null &&
            !string.IsNullOrEmpty(closeTriggerName))
        {
            animator.SetTrigger(closeTriggerName);
        }

        Debug.Log(
            $"{selectedChoice.ItemData.itemName} 구매 완료"
        );

        return true;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex =
                Random.Range(0, i + 1);

            (list[i], list[randomIndex]) =
                (list[randomIndex], list[i]);
        }
    }
}