using System;
using Boss.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Boss.BehaviorNodes.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Stop Moving", story: "[Agent] stops moving", category: "Boss/Action", id: "623fd51fbb6441b59062f1199bc73cd6")]
    public partial class StopMovingAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            var mover = Agent.Value != null ? Agent.Value.GetComponent<IMovable>() : null;
            mover?.Stop();
            return Status.Success;
        }
    }
}
