using CHG.Scripts.Agents;
using UnityEngine;

namespace CHG.Scripts.Test
{
    public class TestDummy : Agent
    {
        protected override void HandleHit()
        {
            Debug.Log($"Hit : {gameObject.name}");
        }
    }
}