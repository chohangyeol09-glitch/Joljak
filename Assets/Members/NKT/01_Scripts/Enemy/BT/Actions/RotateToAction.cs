using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NKT.Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "RotateTo", story: "[Agent] rotate to [Target]", category: "Action", id: "59c22cf6b3712b49f872032d06a4b330")]
    public partial class RotateToAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        
        [SerializeReference] public BlackboardVariable<float> RotateSpeed = new(10f);
        [SerializeReference] public BlackboardVariable<float> RotateDuration = new(0.4f);

        private float _startTime;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
                return Status.Failure;

            _startTime = Time.time;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if(_startTime + RotateDuration.Value < Time.time)
                return Status.Success;

            Vector3 direction = (Target.Value.transform.position - Agent.Value.transform.position);
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            Agent.Value.transform.rotation = Quaternion.Lerp(
                Agent.Value.transform.rotation, targetRotation, RotateSpeed.Value * Time.deltaTime);
            
            return Status.Running;
        }
    }
}

