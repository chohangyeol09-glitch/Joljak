using System;
using Boss.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Boss.BehaviorNodes.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Perform Boss Attack", story: "[Agent] performs [AttackId] on [Target]", category: "Boss/Action", id: "048904713695458580519889af1060de")]
    public partial class PerformBossAttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<string> AttackId;

        private IBossAttacker attacker;
        private BossAttackContext context;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                return Status.Failure;
            }

            attacker = Agent.Value.GetComponent<IBossAttacker>();
            if (attacker == null)
            {
                return Status.Failure;
            }

            context = new BossAttackContext(Agent.Value.transform, Target.Value.transform);
            return attacker.TryBeginAttack(AttackId.Value, context) ? Status.Running : Status.Failure;
        }

        protected override Status OnUpdate()
        {
            var result = attacker.TickCurrentAttack(context);
            return result switch
            {
                BossAttackStatus.Running => Status.Running,
                BossAttackStatus.Completed => Status.Success,
                _ => Status.Failure
            };
        }
    }
}
