using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Boss.Bootstrap
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public class BossFaceTarget : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 720f;
        [SerializeField] private string targetVariableName = "Target";

        private BehaviorGraphAgent agent;

        private void Awake()
        {
            GetComponent<NavMeshAgent>().updateRotation = false;
            agent = GetComponent<BehaviorGraphAgent>();
        }

        private void OnAnimatorMove()
        {
            if (agent.BlackboardReference == null
                || !agent.BlackboardReference.GetVariableValue(targetVariableName, out GameObject target)
                || target == null)
            {
                return;
            }

            Vector3 direction = target.transform.position - transform.position;
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
