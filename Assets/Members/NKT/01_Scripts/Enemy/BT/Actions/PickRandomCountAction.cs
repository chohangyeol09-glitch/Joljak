using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace NKT.Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "PickRandomCount", story: "pick random [Min] and [Max] into [Count]", category: "Action", id: "740c003f2e5a814fbe156216f250a248")]
    public partial class PickRandomCountAction : Action
    {
        [SerializeReference] public BlackboardVariable<int> Min;
        [SerializeReference] public BlackboardVariable<int> Max;
        [SerializeReference] public BlackboardVariable<int> Count;
        [SerializeReference] public BlackboardVariable<int> Counter;

        protected override Status OnStart()
        {
            Count.Value = Random.Range(Min.Value, Max.Value + 1);
            Counter.Value = 0;
            return Status.Running;
        }
    }
}

