using DefaultNamespace;
using Members.CHG.Scripts.Agents;
using Members.CHG.Scripts.Agents.FSM;
using Members.CHG.Scripts.Players.FSM;
using UnityEngine;

namespace Members.CHG.Scripts.Players
{
    public class PlayerController : Agent
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

        private void Update()
        {
            _stateMachine.UpdateMachine();
        }

        public void ChangeState(PlayerState newState, float transitionDuration)
            => _stateMachine.ChangeState((int)newState, transitionDuration);
     
        protected override void HandleHit()
        {
            
        }

        
    }
}