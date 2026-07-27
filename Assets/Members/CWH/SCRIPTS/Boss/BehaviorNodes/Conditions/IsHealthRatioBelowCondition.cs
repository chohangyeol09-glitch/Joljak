using System;
using Boss.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Boss.BehaviorNodes.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Is Health Ratio Below", story: "[Agent] health ratio is below [Ratio]", category: "Boss/Condition", id: "a512c10e813a4f3ab670aa8baf0740d0")]
    public partial class IsHealthRatioBelowCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<float> Ratio = new(0.5f);

        public override bool IsTrue()
        {
            var health = Agent.Value != null ? Agent.Value.GetComponent<IDamageable>() : null;
            return health != null && health.MaxHealth > 0f && (health.CurrentHealth / health.MaxHealth) < Ratio.Value;
        }
    }
}
