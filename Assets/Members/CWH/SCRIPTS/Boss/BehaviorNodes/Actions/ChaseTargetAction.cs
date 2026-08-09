using System;
using Boss.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Boss.BehaviorNodes.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Chase Target", story: "[Agent] chases [Target] until within [StopRange]", category: "Boss/Action", id: "6cc6e542ee4441488cdab578d44d27dd")]
    public partial class ChaseTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> StopRange = new(0f);

        private const float RepathThreshold = 0.5f;

        private IMovable mover;
        private Vector3 lastDestination;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                return Status.Failure;
            }

            mover = Agent.Value.GetComponent<IMovable>();
            if (mover == null)
            {
                return Status.Failure;
            }

            lastDestination = Target.Value.transform.position;
            mover.MoveTo(lastDestination);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            var targetPosition = Target.Value.transform.position;
            if (Vector3.Distance(targetPosition, lastDestination) > RepathThreshold)
            {
                lastDestination = targetPosition;
                mover.MoveTo(lastDestination);
            }

            float distanceToTarget = Vector3.Distance(Agent.Value.transform.position, targetPosition);
            if (distanceToTarget <= StopRange.Value)
            {
                return Status.Success;
            }

            return mover.HasReachedDestination ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            mover?.Stop();
        }
    }
}
