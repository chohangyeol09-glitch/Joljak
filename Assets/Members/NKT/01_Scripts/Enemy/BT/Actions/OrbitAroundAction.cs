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
    [NodeDescription(name: "OrbitAround", story: "[Enemy] orbit around [Target]", category: "Action", id: "c28d4895adf52a2fbad2cece7d6c4e33")]
    public partial class OrbitAroundAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Radius = new(5f);
        [SerializeReference] public BlackboardVariable<float> Height = new(2f);
        [SerializeReference] public BlackboardVariable<float> AngleStep = new(30f);

        private IAgentMovement _movement;
        private int _direction;

        protected override Status OnStart()
        {
            if(Enemy.Value == null || Target.Value == null || Enemy.Value.Movement == null)
                return Status.Failure;
        
            _movement = Enemy.Value.Movement;
            _direction = Random.value < 0.5f ? -1 : 1;
            MoveToNextOrbitPoint();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if(Target.Value == null) return Status.Failure;

            if (_movement.IsArrived && !MoveToNextOrbitPoint())
                return Status.Failure;
        
            return Status.Running;
        }

        private bool MoveToNextOrbitPoint()
        {
            Vector3 next = CalcOrbitPoint(_direction);

            //가려는 쪽이 막혔으면 반대 방향으로 돌아본다
            if (!Enemy.Value.Sensor.IsTargetIsInSight3D(next))
            {
                _direction *= -1;
                next = CalcOrbitPoint(_direction);

                if (!Enemy.Value.Sensor.IsTargetIsInSight3D(next))
                    return false;//양쪽 다 막힘
            }

            _movement.SetDestination(next);
            return true;
        }
        
        private Vector3 CalcOrbitPoint(int direction)
        {
            Vector3 center = Target.Value.transform.position;

            Vector3 offset = Enemy.Value.transform.position - center;
            offset.y = 0f;
            float angle = Mathf.Atan2(offset.z, offset.x);
            angle += direction * AngleStep.Value * Mathf.Deg2Rad;

            Vector3 point = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * Radius.Value;
            point.y = center.y + Height.Value;
            return point;
        }
    }
}

