using UnityEngine;

public class Soul : MonoBehaviour
{
    [Header("획득 수량")]
    [SerializeField] private int soulAmount = 1;

    [Header("상호작용 Collider")]
    [SerializeField] private Collider interactionCollider;

    [Header("획득 연출")]
    [SerializeField] private Animator animator;
    [SerializeField] private string collectTriggerName = "Collect";

    [SerializeField] private GameObject collectEffectPrefab;
    [SerializeField] private AudioClip collectSound;

    [Header("획득 후 제거 시간")]
    [SerializeField] private float destroyDelay = 0.2f;

    private bool isCollected;

    public bool IsCollected => isCollected;

    private void Awake()
    {
        if (interactionCollider == null)
        {
            interactionCollider =
                GetComponentInChildren<Collider>();
        }
    }

    public bool TryCollect(
        PlayerSoulInventory inventory)
    {
        if (isCollected)
            return false;

        if (inventory == null)
        {
            Debug.LogWarning(
                $"{name}: PlayerSoulInventory가 없습니다."
            );

            return false;
        }

        // 연속 입력으로 여러 번 획득하는 것 방지
        isCollected = true;

        if (interactionCollider != null)
            interactionCollider.enabled = false;

        inventory.AddSoul(soulAmount);

        PlayCollectEffect();

        Destroy(
            gameObject,
            Mathf.Max(0f, destroyDelay)
        );

        return true;
    }

    private void PlayCollectEffect()
    {
        if (animator != null &&
            !string.IsNullOrEmpty(collectTriggerName))
        {
            animator.SetTrigger(
                collectTriggerName
            );
        }

        if (collectEffectPrefab != null)
        {
            Instantiate(
                collectEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(
                collectSound,
                transform.position
            );
        }
    }
}