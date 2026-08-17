using CHG.Scripts.Agents;
using NKT.Enemy;
using NKT.FlyingEnemy.Modules;
using NKT.WayPoint;
using Unity.Behavior;
using UnityEngine;

namespace NKT.FlyingEnemy
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class FlyingEnemy : AbstractEnemy
    {
        public Shooter shooter {get; private set;}
        
        protected override void InitializeModules()
        {
            base.InitializeModules();
            shooter = GetModule<Shooter>();
        }
    }
}
