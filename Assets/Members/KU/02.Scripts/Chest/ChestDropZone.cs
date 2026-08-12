using System;
using UnityEngine;

public class ChestDropZone : MonoBehaviour
{
    [Header("낙하 범위")]
    [SerializeField] private Vector2 areaSize = new Vector2(30f, 30f);

    [Header("상자 낙하 높이")]
    [SerializeField] private float dropHeight = 20f;

    [Header("지면 탐색")]
    [SerializeField] private float rayStartHeight = 100f;
    [SerializeField] private float rayDistance = 300f;
    [SerializeField] private LayerMask groundMask;

    [Header("허용할 최대 경사")]
    [SerializeField, Range(0f, 90f)]
    private float maximumSlopeAngle = 35f;

    public bool TryGetDropPosition(
        System.Random random,
        out Vector3 spawnPosition,
        out Vector3 landingPosition)
    {
        spawnPosition = default;
        landingPosition = default;

        if (random == null)
            return false;

        float halfX = areaSize.x * 0.5f;
        float halfZ = areaSize.y * 0.5f;

        float localX = Mathf.Lerp(
            -halfX,
            halfX,
            (float)random.NextDouble()
        );

        float localZ = Mathf.Lerp(
            -halfZ,
            halfZ,
            (float)random.NextDouble()
        );

        Vector3 horizontalPosition = transform.TransformPoint(
            new Vector3(localX, 0f, localZ)
        );

        Vector3 rayOrigin = new Vector3(
            horizontalPosition.x,
            transform.position.y + rayStartHeight,
            horizontalPosition.z
        );

        bool hitGround = Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            rayDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hitGround)
            return false;

        float slopeAngle = Vector3.Angle(
            hit.normal,
            Vector3.up
        );

        if (slopeAngle > maximumSlopeAngle)
            return false;

        landingPosition = hit.point;
        spawnPosition = hit.point + Vector3.up * dropHeight;

        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Quaternion yRotation = Quaternion.Euler(
            0f,
            transform.eulerAngles.y,
            0f
        );

        Matrix4x4 previousMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            yRotation,
            Vector3.one
        );

        Gizmos.color = new Color(0f, 1f, 1f, 0.8f);

        Gizmos.DrawWireCube(
            Vector3.zero,
            new Vector3(areaSize.x, 0.2f, areaSize.y)
        );

        Gizmos.matrix = previousMatrix;

        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.up * dropHeight
        );

        Gizmos.DrawWireSphere(
            transform.position + Vector3.up * dropHeight,
            0.5f
        );
    }
#endif
}