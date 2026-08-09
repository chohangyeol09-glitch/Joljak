using CHG.Scripts.Agents;
using NKT.Enemy;
using NKT.WayPoint;
using Unity.Behavior;
using UnityEngine;

namespace NKT.FlyingEnemy
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class FlyingEnemy : AbstractEnemy
    {
        public IFlyMovement FlyMovement { get; private set; }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            FlyMovement = GetModule<FlyMovement>();
        }
    }
}
