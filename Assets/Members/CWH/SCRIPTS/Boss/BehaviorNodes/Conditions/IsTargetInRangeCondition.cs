using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Boss.BehaviorNodes.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Is Target In Range", story: "[Target] is within [Range] of [Agent]", category: "Boss/Condition", id: "f7ffe32008584728999b1ebf71c13637")]
    public partial class IsTargetInRangeCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Range = new(2.5f);

        public override bool IsTrue()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                return false;
            }

            float distance = Vector3.Distance(Agent.Value.transform.position, Target.Value.transform.position);
            return distance <= Range.Value;
        }
    }
}
