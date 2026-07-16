using System;
using DefaultNamespace;
using Members.CHG._02.Scripts.Agents;
using Members.CHG._02.Scripts.Agents.FSM;
using UnityEngine;
using UnityEngine.XR;

namespace Members.CHG._02.Scripts.Players
{
    public class Player : Agent
    {
        [field:SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] public StateListSO playerStates;
        
        private StateMachine _stateMachine;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            _stateMachine = new StateMachine(this, playerStates.states);
        }
        
        private void Start()
        {
            ChangeState(PlayerState.IDLE, transitionDuration: 0);
        }

        private void ChangeState(PlayerState newState, int transitionDuration)
            => _stateMachine.ChangeState((int)newState, transitionDuration);
        protected override void HandleHit()
        {
            
        }

        
    }
}