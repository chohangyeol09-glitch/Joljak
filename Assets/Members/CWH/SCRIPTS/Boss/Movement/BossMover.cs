using Boss.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Boss.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BossMover : MonoBehaviour, IMovable
    {
        [SerializeField] private float stoppingDistance = 0.5f;

        private NavMeshAgent agent;

        public float MoveSpeed
        {
            get => agent.speed;
            set => agent.speed = value;
        }

        public float CurrentSpeed => agent.velocity.magnitude;

        public bool HasReachedDestination =>
            !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = stoppingDistance;

            // Animator가 Root Motion이 꺼져 있어도 Transform에 개입해서 NavMeshAgent의 자동 이동을 상쇄시키는 문제가 있어,
            // NavMeshAgent의 자동 위치 갱신을 끄고 Animator가 프레임을 다 처리한 뒤(LateUpdate)에 직접 위치를 맞춘다.
            agent.updatePosition = false;
        }

        private void LateUpdate()
        {
            transform.position = agent.nextPosition;
        }

        public void MoveTo(Vector3 destination)
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
        }

        public void Stop()
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
        }
    }
}
