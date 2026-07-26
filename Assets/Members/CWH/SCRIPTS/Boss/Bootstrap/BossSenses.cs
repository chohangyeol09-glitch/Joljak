using Unity.Behavior;
using UnityEngine;

namespace Boss.Bootstrap
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class BossSenses : MonoBehaviour
    {
        [SerializeField] private float detectionRange = 15f;
        [SerializeField] private float attackRange = 2.5f;

        private void Start()
        {
            var agent = GetComponent<BehaviorGraphAgent>();
            agent.BlackboardReference.SetVariableValue("DetectionRange", detectionRange);
            agent.BlackboardReference.SetVariableValue("AttackRange", attackRange);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
