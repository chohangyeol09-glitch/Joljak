using NKT.Enemy;
using System;
using NKT.Enemy.Modules;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy find Target", story: "[Agent] find [Target]", category: "Action", id: "f41886b0ce3d852818ed64ad3b618f48")]
public partial class EnemyFindTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<AbstractEnemy> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Agent.Value.Sensor == null) 
            return Status.Failure;
        
        if(Target.Value != null) 
            return Status.Success;
        
        AgentSensor sensor = Agent.Value.Sensor;

        int detectCount = sensor.FindTargetsInRadius(Agent.Value.EnemyDataSo.DetectRadius);
        if(detectCount <= 0) return Status.Failure;


        for (int i = 0; i < detectCount; i++)
        {
            Transform findTarget = sensor.CollidersResults[i].transform;
            if (!sensor.IsTargetInViewAngle(findTarget, Agent.Value.EnemyDataSo.ViewAngle))
                continue;
            if (!sensor.IsTargetIsInSight(findTarget))
                continue;
                
            Target.Value = findTarget.gameObject;
            break;
        }
            
        return Target.Value == null ? Status.Failure : Status.Success;
    }
}

