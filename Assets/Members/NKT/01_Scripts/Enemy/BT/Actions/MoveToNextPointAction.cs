using System;
using NKT.Enemy.Modules;
using NKT.WayPoint;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NKT.Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "MoveToNextPoint", story: "[Agent] move to point", category: "Action", id: "faa18112e2c2ad3d26be4a757de5f648")]
    public partial class MoveToNextPointAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Agent;
        
        private IAgentMovement _navMovement;

        protected override Status OnStart()
        {
            if(Agent.Value == null || Agent.Value.Movement == null || Agent.Value.StageWayPoints == null)
                return Status.Failure;
            
            _navMovement = Agent.Value.Movement;
            WayPointsContainer stageWayPoints = Agent.Value.StageWayPoints;

            int index = Agent.Value.CurrentWayPoint;
            
            index = index < 0 ? stageWayPoints.GetClosestPointIndexFromPosition(Agent.Value.transform.position)
                : stageWayPoints.GetNextWayPoint(index);
            
            if(index < 0) return Status.Failure;
            
            Agent.Value.CurrentWayPoint = index;
            
            WayPoint.WayPoint targetPoint = stageWayPoints[index];
            _navMovement.SetDestination(targetPoint.Position);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if(_navMovement.IsArrived)
                return Status.Success;
            
            return Status.Running;
        }
    }
}

