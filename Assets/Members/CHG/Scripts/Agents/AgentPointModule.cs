using System;
using System.Collections.Generic;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.Agents
{
    public class AgentPointModule : MonoBehaviour, IModule, IPointModule
    {
        [Serializable]
        private struct Point
        {
            public PointType type;
            public Transform point;
        }

        [SerializeField] private Point[] points;

        private readonly Dictionary<PointType, Transform> _map = new();

        public void Initialize(ModuleOwner owner)
        {
            foreach (Point point in points)
                if (point.point != null) _map[point.type] = point.point;
        }

        public Transform GetPoint(PointType type)
        {
            if (_map.TryGetValue(type, out Transform point)) return point;
            
            Debug.LogWarning($"{type} point not found");
            return transform;
        }
    }
}