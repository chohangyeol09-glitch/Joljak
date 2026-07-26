using UnityEngine;

namespace NKT.Enemy
{
    public enum EnemyDataField
    {
        DetectRadius,
        ViewAngle,
        StopDistance,
    }
    [CreateAssetMenu(fileName = "EnemyData", menuName = "KT/SO/EnemyData", order = 0)]
    public class EnemyDataSO : ScriptableObject
    {
        public float DetectRadius;
        public float ViewAngle;
        public float StopDistance;
    }
}