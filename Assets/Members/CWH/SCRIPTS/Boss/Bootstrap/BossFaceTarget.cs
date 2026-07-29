using Boss.Core;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Boss.Bootstrap
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public class BossFaceTarget : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float movingThreshold = 0.1f;
        [SerializeField] private string targetVariableName = "Target";

        private BehaviorGraphAgent agent;
        private IBossAttacker attacker;
        private IMovable mover;

        private void Awake()
        {
            GetComponent<NavMeshAgent>().updateRotation = false;
            agent = GetComponent<BehaviorGraphAgent>();
            attacker = GetComponent<IBossAttacker>();
            mover = GetComponent<IMovable>();
        }

        private void OnAnimatorMove()
        {
            if (attacker != null && attacker.IsAttacking)
            {
                return;
            }

            Vector3 direction;
            if (mover != null && mover.CurrentSpeed > movingThreshold)
            {
                direction = mover.Velocity;
            }
            else if (agent.BlackboardReference != null
                && agent.BlackboardReference.GetVariableValue(targetVariableName, out GameObject target)
                && target != null)
            {
                direction = target.transform.position - transform.position;
            }
            else
            {
                return;
            }

            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
