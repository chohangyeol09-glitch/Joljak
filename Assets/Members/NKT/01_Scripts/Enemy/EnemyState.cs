using Unity.Behavior;

namespace NKT.Enemy
{
    [BlackboardEnum]
    public enum EnemyState
    {
        IDLE,
        MOVE,
        COMBAT,
        HIT,
        DEAD
    }
}