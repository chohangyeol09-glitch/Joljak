using UnityEngine;

namespace NKT.FlyingEnemy
{
    public interface IFlyMovement
    {
        Vector3 Velocity { get; set; }
        float Speed { get; set; }
        bool IsStopped { get; set; }
        bool IsArrived { get; }

        void SetDestination(Vector3 destination);
    }
}
