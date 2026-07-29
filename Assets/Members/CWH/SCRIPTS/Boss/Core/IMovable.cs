using UnityEngine;

namespace Boss.Core
{
    public interface IMovable
    {
        float MoveSpeed { get; set; }
        float CurrentSpeed { get; }
        Vector3 Velocity { get; }
        bool HasReachedDestination { get; }

        void MoveTo(Vector3 destination);
        void Stop();
    }
}
