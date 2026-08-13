using CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace NKT.Enemy.Modules
{
    public class AgentSensor : MonoBehaviour, IModule
    {
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private int maxColliderCount = 5;

        public Collider[] CollidersResults => _colliderResults;

        private ModuleOwner _owner;
        private Collider[] _colliderResults;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            Debug.Assert(maxColliderCount > 0, "maxColliderCount > 0이어야 함");
            _colliderResults = new Collider[maxColliderCount];
        }
        
        public bool IsTargetInViewAngle(Transform targetTrm, float viewAngle)
        {
            Vector3 direction = targetTrm.position - transform.position;
            direction.y = 0;
            float angle = Vector3.Angle(transform.forward, direction);
            return angle <= viewAngle * 0.5f;
        }

        public bool IsTargetIsInSight(Transform targetTrm)
        {
            Vector3 targetPosition = targetTrm.position;
            targetPosition.y = transform.position.y;
            Vector3 direction = targetPosition - transform.position;
            float distance = direction.magnitude;
            if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, obstacleLayer))
            {
                Debug.Log(hit.collider.gameObject.name);
                return false; //장애물이 시야를 가로막는중이다.
            }
            return true;
        }

        public bool IsTargetIsInSight3D(Vector3 target)
        {
            target.y += 1f;//여유분으로 살짝 위를 기준으로
            Vector3 direction = target - transform.position;
            float distance = direction.magnitude;
            if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, obstacleLayer))
            {
                Debug.Log(hit.collider.gameObject.name);
                return false; //장애물이 시야를 가로막는중이다.
            }
            return true;
        }
        
        public bool IsTargetInViewRadius(Transform targetTrm, float viewRadius)
            => (targetTrm.position - transform.position).sqrMagnitude <=  viewRadius * viewRadius;
        public int FindTargetsInRadius(float viewRadius)
            => Physics.OverlapSphereNonAlloc(transform.position, viewRadius, _colliderResults, targetLayer);
    }
}