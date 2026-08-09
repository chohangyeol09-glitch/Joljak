using System;
using CHG.Scripts.CoreSystem.AnimationSystem;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace NKT.Enemy.BT.Events
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/AnimationChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "AnimationChannel", message: "play [Clip]", category: "Events", id: "df01422756072d5d0c0cca49d939d5cc")]
    public sealed partial class AnimationChannelAction : EventChannel<AnimParamSO>
    {
    }
}

