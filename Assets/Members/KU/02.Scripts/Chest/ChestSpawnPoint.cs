using UnityEngine;

public class ChestSpawnPoint : MonoBehaviour
{
    [Header("지면에서 띄울 높이")]
    [SerializeField] private float heightOffset;

    public Vector3 SpawnPosition =>
        transform.position + Vector3.up * heightOffset;

    public Quaternion SpawnRotation =>
        transform.rotation;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(SpawnPosition, new Vector3(1.2f, 0.8f, 1.2f));

        Gizmos.DrawLine(
            SpawnPosition,
            SpawnPosition + transform.forward * 1.5f
        );
    }
#endif
}