using UnityEngine;

namespace CHG.Scripts.Agents
{
    public interface IPointModule
    {
        Transform GetPoint(PointType type);
    }
}