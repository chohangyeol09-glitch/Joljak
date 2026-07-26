using System;
using Boss.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Boss.BehaviorNodes.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Is Boss Phase At Least", story: "[Agent] phase is at least [Phase]", category: "Boss/Condition", id: "540caba82ccf4195afdb460cd07e88a1")]
    public partial class IsBossPhaseAtLeastCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<int> Phase = new(1);

        public override bool IsTrue()
        {
            var phaseController = Agent.Value != null ? Agent.Value.GetComponent<IBossPhaseController>() : null;
            return phaseController != null && phaseController.CurrentPhase >= Phase.Value;
        }
    }
}
