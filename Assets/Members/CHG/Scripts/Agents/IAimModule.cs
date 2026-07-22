using UnityEngine;

namespace Members.CHG.Scripts.Agents
{
    public interface IAimModule
    {
        Vector3 AimDirection { get; }
        Vector3 AimForward { get; }
        Vector3 AimPoint { get; }
        Vector3 AimOrigin { get; }
        
        bool IsAiming { get; }
        void AddAimLock(object key);
        void RemoveAimLock(object key);
    }
}   