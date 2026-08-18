using System;
using Boss.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Boss.BehaviorNodes.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Perform Boss Attack", story: "[Agent] warns for [WarningSeconds] then performs [AttackId] on [Target], recovering for [RecoverySeconds]", category: "Boss/Action", id: "048904713695458580519889af1060de")]
    public partial class PerformBossAttackAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<string> AttackId;
        [SerializeReference] public BlackboardVariable<float> WarningSeconds = new(0f);
        [SerializeReference] public BlackboardVariable<float> RecoverySeconds = new(0f);

        private IBossAttacker attacker;
        private IBossOutline outline;
        private BossAttackContext context;
        private float warningTimer;
        private bool attackStarted;
        private bool attackFinished;
        private float recoveryTimer;

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

            outline = Agent.Value.GetComponent<IBossOutline>();
            context = new BossAttackContext(Agent.Value.transform, Target.Value.transform);

            attackStarted = false;
            attackFinished = false;
            warningTimer = WarningSeconds.Value;
            outline?.PlayAttackWarning(warningTimer);

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!attackStarted)
            {
                if (warningTimer > 0f)
                {
                    warningTimer -= Time.deltaTime;
                    return Status.Running;
                }

                attackStarted = true;
                if (!attacker.TryBeginAttack(AttackId.Value, context))
                {
                    return Status.Failure;
                }

                return Status.Running;
            }

            if (!attackFinished)
            {
                var result = attacker.TickCurrentAttack(context);
                if (result == BossAttackStatus.Running)
                {
                    return Status.Running;
                }

                if (result == BossAttackStatus.Failed)
                {
                    return Status.Failure;
                }

                attackFinished = true;
                recoveryTimer = RecoverySeconds.Value;
            }

            if (recoveryTimer > 0f)
            {
                recoveryTimer -= Time.deltaTime;
                return Status.Running;
            }

            return Status.Success;
        }
    }
}
