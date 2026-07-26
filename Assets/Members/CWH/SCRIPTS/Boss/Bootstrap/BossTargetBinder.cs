using Unity.Behavior;
using UnityEngine;

namespace Boss.Bootstrap
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class BossTargetBinder : MonoBehaviour
    {
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private string blackboardVariableName = "Target";

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag(targetTag);
            if (player == null)
            {
                return;
            }

            var agent = GetComponent<BehaviorGraphAgent>();
            agent.BlackboardReference.SetVariableValue(blackboardVariableName, player);
        }
    }
}
