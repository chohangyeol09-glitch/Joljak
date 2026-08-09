using System;
using NKT.Enemy.Modules;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NKT.Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "ApproachTarget", story: "[Enemy] approach [Target]", category: "Action", id: "13a32dbe5f0fbbb3eee50077705bb65a")]
    public partial class ApproachTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private Vector3 _destination;
        private INavMovement _navMovement;
        
        protected override Status OnStart()
        {
            if (Enemy.Value == null || Target.Value == null || Enemy.Value.Movement == null)
                return Status.Failure;

            _navMovement = Enemy.Value.Movement;
            _destination = Target.Value.transform.position;
            _navMovement.SetDestination(_destination);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Target.Value == null) return Status.Failure;

            Vector3 newDestination = Target.Value.transform.position;
            if (Vector3.Distance(_destination, newDestination) > 1f)
            {
                _destination = newDestination;
                _navMovement.SetDestination(_destination);
            }

            float distance = Vector3.Distance(Enemy.Value.transform.position, Target.Value.transform.position);
            return distance <= Enemy.Value.EnemyDataSo.StopDistance ? Status.Success : Status.Running;
        }
    }
}

