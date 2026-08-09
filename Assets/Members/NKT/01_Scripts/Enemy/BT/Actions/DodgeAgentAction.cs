using System;
using NKT.Enemy.Modules;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

namespace NKT.Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "DodgeAgent", story: "[Enemy] dodge from [Target]", category: "Action", id: "9b30e15fdb6e0bb164cd254cf2d7c7d3")]
    public partial class DodgeAgentAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> MinDistance = new(2f);
        [SerializeReference] public BlackboardVariable<float> MaxDistance = new(4f);
        
        private INavMovement _navMovement;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Target.Value == null || Enemy.Value.Movement == null)
                return Status.Failure;

            _navMovement = Enemy.Value.Movement;

            Vector3 toTarget = (Target.Value.transform.position - Enemy.Value.transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, toTarget);
            Vector3[] candidates = { right, -right, -toTarget }; // 오른쪽, 왼쪽, 뒤
            Vector3 dir = candidates[Random.Range(0, candidates.Length)];

            float distance = Random.Range(MinDistance.Value, MaxDistance.Value);
            _navMovement.SetDestination(Enemy.Value.transform.position + dir * distance);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _navMovement.IsArrived ? Status.Success : Status.Running;
        }
    }
}

