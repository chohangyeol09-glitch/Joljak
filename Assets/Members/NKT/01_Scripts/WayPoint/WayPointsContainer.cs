using UnityEngine;

namespace NKT.WayPoint
{
    public class WayPointsContainer : MonoBehaviour
    {
        [SerializeField] private WayPoint[] wayPoints;
        public WayPoint this[int index] => wayPoints[index];

        public int GetNextWayPoint(int currentIndex)
        {
            return (currentIndex + 1) % wayPoints.Length;
        }
        
        public int GetClosestPointIndexFromPosition(Vector3 position)
        {
            float minDistance = float.MaxValue;
            int closestIndex = -1;

            for (int i = 0; i < wayPoints.Length; i++)
            {
                Vector3 wayPointPos = wayPoints[i].Position;
                float distance = Vector3.SqrMagnitude(wayPointPos - position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }
    }
}