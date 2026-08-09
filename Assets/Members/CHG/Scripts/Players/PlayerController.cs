using CHG.Scripts.Agents;
using CHG.Scripts.Agents.FSM;
using CHG.Scripts.Players.FSM;
using CHG.Scripts.SkillSystem;
using CHG.Scripts.Weapon;
using DefaultNamespace;
using Members.CHG.Scripts.Players.FSM;
using UnityEngine;

namespace CHG.Scripts.Players
{
    public class PlayerController : Agent
    {
        [field:SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] public StateListSO playerStates;
        
        private StateMachine _stateMachine;
        private IControlMovement _controlMovement;
        private IAimModule _aimModule;
        private IWeapon _weapon;
        private IReloadable _reloadable;
        private ConstraintModule _constraint;
        private ISkillModule _skillModule;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            _stateMachine = new StateMachine(this, playerStates.states);
            _controlMovement = GetModule<IControlMovement>();
            _aimModule = GetModule<IAimModule>();
            _weapon = GetModule<IWeapon>();
            _reloadable = _weapon as IReloadable;
            _constraint = GetModule<ConstraintModule>();
            _skillModule = GetModule<ISkillModule>();
            
            Debug.Assert(_controlMovement != null, $"ControlMovement is null : {gameObject.name}");
            Debug.Assert(_aimModule != null, $"AimModule is null : {gameObject.name}");
            Debug.Assert(_weapon != null, $"Weapon is null : {gameObject.name}");
            Debug.Assert(_constraint != null, $"Constraint is null : {gameObject.name}");
            Debug.Assert(_skillModule != null, $"SkillModule is null : {gameObject.name}");
        }

        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();

            PlayerInput.OnAttackKeyDown += HandleAttackStart;
            PlayerInput.OnAttackKeyUp += HandleAttackEnd;
            PlayerInput.OnAimKeyDown += HandleAimingStart;
            PlayerInput.OnAimKeyUp += HandleAimingEnd;
            PlayerInput.OnJumpKeyDown += HandleJump;
            PlayerInput.OnDashKeyDown += HandleDash;
            PlayerInput.OnReloadKeyDown += HandleReload;
            PlayerInput.OnSkillKeyDown += HandelSkillKeyDown;
            PlayerInput.OnSkillKeyUp += HandleSkillKeyUp;
            _constraint.OnConstraintAdded += HandleConstraintAdded;
            _constraint.OnConstraintRemoved += HandleConstraintRemoved;
            
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            PlayerInput.OnAttackKeyDown -= HandleAttackStart;
            PlayerInput.OnAttackKeyUp -= HandleAttackEnd;
            PlayerInput.OnAimKeyDown -= HandleAimingStart;
            PlayerInput.OnAimKeyUp -= HandleAimingEnd;
            PlayerInput.OnJumpKeyDown -= HandleJump;
            PlayerInput.OnDashKeyDown -= HandleDash;
            PlayerInput.OnReloadKeyDown -= HandleReload;
            PlayerInput.OnSkillKeyDown -= HandelSkillKeyDown;
            PlayerInput.OnSkillKeyUp -= HandleSkillKeyUp;
            _constraint.OnConstraintAdded -= HandleConstraintAdded;
            _constraint.OnConstraintRemoved -= HandleConstraintRemoved;
        }
        
        protected override void Start()
        {
            base.Start();
            ChangeState(PlayerState.IDLE, transitionDuration: 0);
        }

        private void Update()
        {
            _stateMachine.UpdateMachine();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdateMachine();
        }
        
        public void ChangeState(PlayerState newState, float transitionDuration)
            => _stateMachine.ChangeState((int)newState, transitionDuration);
        public void TryChangeState(PlayerState newState, float transitionDuration)
            => _stateMachine.TryChangeState((int)newState, transitionDuration);

        private void HandleAttackStart()
        {
            _aimModule.RequestAim(this);
            _weapon.StartFire();
        }
        private void HandleAttackEnd()
        {
            _aimModule.ReleaseAim(this);
            _weapon.StopFire();
        }
        
        private void HandleAimingStart()
        {
            _aimModule.Aiming(true);
        }

        private void HandleAimingEnd()
        {
            _aimModule.Aiming(false);
        }
        
        private void HandleJump() => _controlMovement.TryJump();
        
        private void HandleDash()
        {
            if (_constraint.Has(ConstraintType.Rooted)) return;
            TryChangeState(PlayerState.DASH, 0.05f);
        }

        private void HandleConstraintAdded(ConstraintType type)
        {
            if (type == ConstraintType.Stunned)
                TryChangeState(PlayerState.STUN, 0.1f);
        }
        
        private void HandleConstraintRemoved(ConstraintType type)
        {
            if (type != ConstraintType.Stunned && type != ConstraintType.Rooted) return;
            if (_constraint.Has(ConstraintType.Stunned)) return;

            bool hasInput = PlayerInput.CurrentMovement.magnitude > 0.1f;
            ChangeState(hasInput ? PlayerState.RUN : PlayerState.IDLE, 0.1f);
        }
        
        private void HandleReload()
        {
            _reloadable?.Reload();
        }
        
        private void HandelSkillKeyDown(int slot)
        {
            _reloadable?.CancelReload();
            _skillModule.TryUseSkill(slot);
        }

        private void HandleSkillKeyUp(int slot)
        {
            _skillModule.ReleaseSkill(slot);
        }
        
        protected override void HandleHit()
        {
            
        }
    }
}