using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NKT.Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "StopAgent", story: "[Agent] stop", category: "Action", id: "fbf0732ba2b04c45b85577bde4ee34a7")]
    public partial class StopAgentAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Agent;

        protected override Status OnStart()
        {
            if (Agent.Value == null|| Agent.Value.Movement == null) return Status.Failure;
            
            Agent.Value.Movement.Stop();
            return Status.Running;
        }
    }
}

