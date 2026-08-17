using System;
using CHG.Scripts.Agents;
using Members.CHG.Scripts;
using NKT.Enemy.Modules;
using NKT.WayPoint;
using UnityEngine;

namespace NKT.Enemy
{
    public class AbstractEnemy : Agent
    {
        [SerializeField] private EnemyDataSO enemyData;

        public EnemyDataSO EnemyDataSo => enemyData;
        public IAgentMovement Movement { get; private set; }
        public AgentRenderer Renderer { get; private set; }
        public AgentSensor Sensor { get; private set; }
        public AgentTrigger Trigger { get; private set; }
        [field: SerializeField] public WayPointsContainer StageWayPoints { get; private set; }
        public int CurrentWayPoint { get; set; } = -1;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            Sensor = GetModule<AgentSensor>();
            Movement = GetModule<IAgentMovement>();
            Renderer = GetModule<AgentRenderer>();
            Trigger = GetModule<AgentTrigger>();
        }

        protected override void HandleHit()
        {
            
        }

        private void OnDrawGizmosSelected()
        {
            if (enemyData == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.DetectRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, enemyData.StopDistance);
        }
    }
}