using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NKT.Enemy.BT.Events
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/EnemyStateChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "EnemyStateChannel", message: "[State] change", category: "Events", id: "2509e9940bbec1f569f5e9c380faa3f3")]
    public sealed partial class EnemyStateChannelAction : EventChannel<EnemyState>
    {
    }
}

