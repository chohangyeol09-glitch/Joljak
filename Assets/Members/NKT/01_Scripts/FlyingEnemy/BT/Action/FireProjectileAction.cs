using System;
using NKT.Enemy;
using NKT.FlyingEnemy.Modules;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace NKT.FlyingEnemy.BT.Action
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "FireProjectile", story: "[Enemy] fire [Count] shoot at [Target]", category: "Action", id: "099e6790f7cd77b4268a5ae04e11b2e0")]
    public partial class FireProjectileAction : Unity.Behavior.Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<int> Count;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Interval = new(0.4f);
        
        private Shooter _shooter;
        private int _firedCount;
        private float _nextFireTime;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || Target.Value == null)
                return Status.Failure;

            _shooter = Enemy.Value.GetModule<Shooter>();
            if(_shooter == null)
                return Status.Failure;

            _firedCount = 0;
            _nextFireTime = Time.time;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Target.Value == null) return Status.Failure;

            if (Time.time < _nextFireTime)
                return Status.Running;

            _shooter.Fire(Target.Value.transform.position);
            _firedCount++;
            _nextFireTime = Time.time + Interval.Value;

            return _firedCount >= Count.Value ? Status.Success : Status.Running;
        }
    }
}

