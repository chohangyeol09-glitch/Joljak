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
        private IAgentMovement _navMovement;
        
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
            if (Vector3.Distance(_destination, newDestination) > 1f)//타겟이 기존 위치에서 어느정도 움직였을때
            {
                _destination = newDestination;//새로 갱신을 한다
                _navMovement.SetDestination(_destination);
            }

            Vector3 toTarget = Target.Value.transform.position - Enemy.Value.transform.position;
            toTarget.y = 0f;//공중 유닛이 타겟 위에 떠 있어도 도착 판정이 되도록 수평 거리만 본다

            return toTarget.magnitude <= Enemy.Value.EnemyDataSo.StopDistance ? Status.Success : Status.Running;
        }
    }
}

