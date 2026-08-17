using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Chest : MonoBehaviour
{
    [Header("기본 가격")]
    [SerializeField] private int defaultPurchasePrice = 25;

    [Header("상자 열기")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private ItemDropper itemDropper;

    [Header("낙하 물리")]
    [SerializeField] private Rigidbody body;

    [Tooltip("상자가 착지할 수 있는 지면 레이어")]
    [SerializeField] private LayerMask landingMask;

    [Tooltip("선택 사항입니다. 착지 전에는 꺼지고 착지 후 켜집니다.")]
    [SerializeField] private Collider interactionCollider;

    [SerializeField] private float landingSettleTime = 0.2f;

    [SerializeField, Range(0f, 1f)]
    private float minimumLandingNormalY = 0.45f;

    private int purchasePrice;
    private bool isOpened;
    private bool isLanded = true;
    private bool isLanding;

    public int PurchasePrice => purchasePrice;
    public bool IsOpened => isOpened;
    public bool IsLanded => isLanded;

    private void Awake()
    {
        purchasePrice = defaultPurchasePrice;

        if (body == null)
            body = GetComponent<Rigidbody>();

        // 씬에 직접 둔 상자는 처음부터 고정 상태
        LockPhysics();

        if (interactionCollider != null)
            interactionCollider.enabled = true;
    }

    public void Initialize(int newPrice)
    {
        purchasePrice = Mathf.Max(0, newPrice);
        isOpened = false;
    }

    public void BeginDrop()
    {
        if (body == null)
        {
            Debug.LogError(
                $"{name}: Rigidbody가 없습니다."
            );

            return;
        }

        isLanded = false;
        isLanding = false;

        if (interactionCollider != null)
            interactionCollider.enabled = false;

        body.isKinematic = false;
        body.useGravity = true;

        // 낙하 중 상자가 옆으로 심하게 넘어지지 않도록 제한
        body.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        // 높은 곳에서 떨어질 때 지면을 뚫는 현상 방지
        body.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        body.interpolation =
            RigidbodyInterpolation.Interpolate;

        body.WakeUp();
    }

    public bool TryOpen(PlayerWallet wallet)
    {
        if (!isLanded)
        {
            Debug.Log("상자가 아직 착지하지 않았습니다.");
            return false;
        }

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

        if (itemDropper == null)
            return;

        Vector3 spawnPosition =
            itemSpawnPoint != null
                ? itemSpawnPoint.position
                : transform.position + Vector3.up;

        itemDropper.DropItem(spawnPosition);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isLanded || isLanding)
            return;

        bool isLandingSurface =
            (landingMask.value &
             (1 << collision.gameObject.layer)) != 0;

        if (!isLandingSurface)
            return;

        bool hasValidGroundContact = false;

        foreach (ContactPoint contact
                 in collision.contacts)
        {
            if (contact.normal.y >= minimumLandingNormalY)
            {
                hasValidGroundContact = true;
                break;
            }
        }

        if (!hasValidGroundContact)
            return;

        StartCoroutine(SettleAndLock());
    }

    private IEnumerator SettleAndLock()
    {
        isLanding = true;

        yield return new WaitForSeconds(
            landingSettleTime
        );

        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;

            body.useGravity = false;
            body.isKinematic = true;

            body.constraints =
                RigidbodyConstraints.FreezeAll;
        }

        // 상자를 똑바로 세움
        Vector3 currentEuler = transform.eulerAngles;

        transform.rotation = Quaternion.Euler(
            0f,
            currentEuler.y,
            0f
        );

        isLanded = true;
        isLanding = false;

        if (interactionCollider != null)
            interactionCollider.enabled = true;

        Debug.Log($"{name}: 상자 착지 완료");
    }

    private void LockPhysics()
    {
        if (body == null)
            return;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = false;
        body.isKinematic = true;
        body.constraints = RigidbodyConstraints.FreezeAll;
    }
}