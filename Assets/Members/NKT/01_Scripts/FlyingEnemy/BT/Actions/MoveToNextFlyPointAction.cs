using System;
using NKT.WayPoint;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NKT.FlyingEnemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "MoveToNextFlyPoint", story: "[Agent] fly to point", category: "Action", id: "6a2c9e0f4b7c4a1abf0e3d5c8f6a2b91")]
    public partial class MoveToNextFlyPointAction : Action
    {
        [SerializeReference] public BlackboardVariable<FlyingEnemy> Agent;

        private IFlyMovement _movement;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Agent.Value.FlyMovement == null || Agent.Value.StageWayPoints == null)
                return Status.Failure;

            _movement = Agent.Value.FlyMovement;
            WayPointsContainer stageWayPoints = Agent.Value.StageWayPoints;

            int index = Agent.Value.CurrentWayPoint;
            index = index < 0
                ? stageWayPoints.GetClosestPointIndexFromPosition(Agent.Value.transform.position)
                : stageWayPoints.GetNextWayPoint(index);

            if (index < 0) return Status.Failure;

            Agent.Value.CurrentWayPoint = index;

            WayPoint.WayPoint targetPoint = stageWayPoints[index];
            _movement.SetDestination(targetPoint.Position);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _movement.IsArrived ? Status.Success : Status.Running;
        }
    }
}
