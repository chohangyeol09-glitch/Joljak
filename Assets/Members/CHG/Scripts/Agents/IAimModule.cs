using UnityEngine;

namespace CHG.Scripts.Agents
{
    public interface IAimModule
    {
        Vector3 AimOrigin { get; }
        Vector3 AimForward { get; }
        Vector3 AimPoint { get; }

        bool IsAiming { get; }
        void Aiming(bool starting);
        void RequestAim(object key);
        void ReleaseAim(object key);
    }
}
