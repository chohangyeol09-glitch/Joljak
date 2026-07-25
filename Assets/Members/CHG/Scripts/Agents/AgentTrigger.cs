using System;
using Members.CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace Members.CHG.Scripts.Agents
{
    public class AgentTrigger : MonoBehaviour, IModule
    {
        public event Action OnAnimationEnd;
        public event Action OnDamageCast;
        
        public void Initialize(ModuleOwner owner) { }
        
        private void AnimationEndTrigger() => OnAnimationEnd?.Invoke();
        private void DamageCastTrigger() => OnDamageCast?.Invoke();
    }
}