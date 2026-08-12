using System;
using System.Collections.Generic;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.Agents
{
    public enum ConstraintType
    {
        Rooted, //수동 이동, 점프, 대쉬 불가
        Disarmed, //무기 발사 불가
        Silenced, //스킬 사용 불가
        Stunned, // 스턴 상태 전환 
        Weightless, //중력 무시
    }
    
    public class ConstraintModule : MonoBehaviour, IModule
    {
        private readonly Dictionary<ConstraintType, HashSet<object>> _holders = new();

        public event Action<ConstraintType> OnConstraintAdded;
        public event Action<ConstraintType> OnConstraintRemoved;

        public void Initialize(ModuleOwner owner) { }

        public bool Has(ConstraintType type)
            => _holders.TryGetValue(type, out var set) && set.Count > 0;

        public void Add(ConstraintType type, object key)
        {
            if (!_holders.TryGetValue(type, out var set))
                _holders[type] = set = new HashSet<object>();

            bool wasEmpty = set.Count == 0;
            if (set.Add(key) && wasEmpty)
                OnConstraintAdded?.Invoke(type);
        }

        public void Remove(ConstraintType type, object key)
        {
            if (!_holders.TryGetValue(type, out var set)) return;

            if (set.Remove(key) && set.Count == 0)
                OnConstraintRemoved?.Invoke(type);
        }
    }
}