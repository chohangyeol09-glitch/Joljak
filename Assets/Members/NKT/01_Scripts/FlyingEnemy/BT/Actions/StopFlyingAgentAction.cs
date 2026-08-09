using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NKT.FlyingEnemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "StopFlyingAgent", story: "[Agent] stop", category: "Action", id: "d17f5b3a92c1487a9e6f0b2d7c4a5e83")]
    public partial class StopFlyingAgentAction : Action
    {
        [SerializeReference] public BlackboardVariable<FlyingEnemy> Agent;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Agent.Value.FlyMovement == null) return Status.Failure;

            Agent.Value.FlyMovement.IsStopped = true;
            return Status.Success;
        }
    }
}
