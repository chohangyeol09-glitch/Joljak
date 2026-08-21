using UnityEngine;

public class SoulSpawnPoint : MonoBehaviour
{
    [Header("생성 위치 보정")]
    [SerializeField] private float heightOffset = 1f;

    private Soul spawnedSoul;

    public Vector3 SpawnPosition =>
        transform.position + Vector3.up * heightOffset;

    public Quaternion SpawnRotation =>
        transform.rotation;

    public bool HasSoul => spawnedSoul != null;

    public void SpawnSoul(
        Soul soulPrefab,
        Transform parent = null)
    {
        if (soulPrefab == null)
        {
            Debug.LogWarning(
                $"{name}: Soul 프리팹이 없습니다."
            );

            return;
        }

        if (HasSoul)
            return;

        spawnedSoul = Instantiate(
            soulPrefab,
            SpawnPosition,
            SpawnRotation,
            parent
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            SpawnPosition,
            0.5f
        );

        Gizmos.DrawLine(
            transform.position,
            SpawnPosition
        );
    }
#endif
}