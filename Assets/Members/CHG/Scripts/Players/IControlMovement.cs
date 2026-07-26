using UnityEngine;

namespace CHG.Scripts.Players
{
    public interface IControlMovement
    {
        bool CanManualMove { get; }
        float DashSpeed { get; }
        float DashDuration { get; }
        float DashCooldown { get; }
        float VerticalVelocity { get; }
        
        Vector3 HorizontalVelocity { get; }

        void AddImpulse(Vector3 impulse);

        void SetDrivenVelocity(Vector3 velocity);

        bool IsGround { get; }

        bool TryJump();

        void SetMovementDirection(Vector2 movementInput);
        
        
    }
}