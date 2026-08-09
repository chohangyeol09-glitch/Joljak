using System;

namespace Boss.Core
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDead { get; }

        void TakeDamage(float amount);

        event Action<float, float> HealthChanged;
        event Action Died;
    }
}
