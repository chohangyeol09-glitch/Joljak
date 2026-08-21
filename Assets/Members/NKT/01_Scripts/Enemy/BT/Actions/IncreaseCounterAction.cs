using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NKT.Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "IncreaseCounter", story: "increase [Counter]", category: "Action", id: "3cc849b7e5a6e6787962ea101d2fe825")]
    public partial class IncreaseCounterAction : Action
    {
        [SerializeReference] public BlackboardVariable<int> Counter;

        protected override Status OnStart()
        {
            Counter.Value++;
            return Status.Running;
        }
    }
}

