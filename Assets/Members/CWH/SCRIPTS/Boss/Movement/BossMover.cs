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
        private bool autoMovementSuspended;

        public float MoveSpeed
        {
            get => agent.speed;
            set => agent.speed = value;
        }

        public float CurrentSpeed => agent.velocity.magnitude;

        public Vector3 Velocity => agent.velocity;

        public bool HasReachedDestination =>
            !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = stoppingDistance;
            agent.updatePosition = false;
        }

        private void LateUpdate()
        {
            if (autoMovementSuspended)
            {
                return;
            }

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

        public void SuspendAutoMovement()
        {
            autoMovementSuspended = true;
            agent.isStopped = true;
        }

        public void ResumeAutoMovement(Vector3 currentPosition)
        {
            agent.Warp(currentPosition);
            autoMovementSuspended = false;
        }
    }
}
