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
        public INavMovement Movement { get; private set; }
        public AgentSensor Sensor { get; private set; }
        [field: SerializeField] public WayPointsContainer StageWayPoints { get; private set; }
        public int CurrentWayPoint { get; set; } = -1;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            Sensor = GetModule<AgentSensor>();
            Movement = GetModule<NavMovement>();
        }

        protected override void HandleHit()
        {
            
        }
    }
}