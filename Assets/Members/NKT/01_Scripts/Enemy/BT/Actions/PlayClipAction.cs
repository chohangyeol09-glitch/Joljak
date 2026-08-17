using System;
using CHG.Scripts.CoreSystem.AnimationSystem;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NKT.Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Play Clip", story: "[Enemy] play [Clip] at [Layer] and [Position]", category: "Action", id: "fa6dacb4063e3a7e234d129ef6ffa4c0")]
    public partial class PlayClipAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<AnimParamSO> Clip;
        [SerializeReference] public BlackboardVariable<int> Layer;
        [SerializeReference] public BlackboardVariable<float> Position;

        [SerializeReference] public BlackboardVariable<float> CrossDuration = new(0.2f);
        
        protected override Status OnStart()
        {
            if (Enemy.Value == null || Enemy.Value.Renderer == null || Clip.Value == null)
                return Status.Failure;

            Enemy.Value.Renderer.PlayClip(Clip.Value.ParamHash, Position.Value, CrossDuration.Value, Layer.Value);
            return Status.Success;
        }
    }
}

