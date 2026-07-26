using System.Collections.Generic;
using CHG.Scripts.Agents;
using CHG.Scripts.CoreSystem.ModuleSystem;
using DefaultNamespace;
using UnityEngine;

namespace CHG.Scripts.Players
{
    public class PlayerLookModule : MonoBehaviour, IModule, IAimModule
    {
        [SerializeField] private Transform lookPivot;
        [SerializeField] private float sensitivity = 0.15f;
        [SerializeField] private float pitchMin = -40f;
        [SerializeField] private float pitchMax = 70f;
        [SerializeField] private bool invertY;

        [SerializeField] private float maxAimDistance = 100f;
        [SerializeField] private LayerMask aimMask;

        private float _yaw;
        private float _pitch;
        private PlayerInputSO _playerInput;
        private readonly HashSet<object> _aimKeys = new();

        public Quaternion YawRotation => Quaternion.Euler(0f, _yaw, 0f);

        public Vector3 AimOrigin => lookPivot.position;
        public Vector3 AimForward => lookPivot.forward;
        public Vector3 AimPoint { get; private set; }

        public bool IsAiming => _aimKeys.Count > 0;
        public void RequestAim(object key) => _aimKeys.Add(key);
        public void ReleaseAim(object key) => _aimKeys.Remove(key);

        public void Initialize(ModuleOwner owner)
        {
            _playerInput = (owner as PlayerController).PlayerInput;

            Debug.Assert(lookPivot != null, $"LookPivot is null : {gameObject.name}");
            _yaw = lookPivot.eulerAngles.y;

            UpdateAimPoint();

            _playerInput.OnLookChange += HandleLookChange;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDestroy()
        {
            if (_playerInput != null)
                _playerInput.OnLookChange -= HandleLookChange;
        }

        private void HandleLookChange(Vector2 lookDelta)
        {
            _yaw += lookDelta.x * sensitivity;
            _pitch += lookDelta.y * sensitivity * (invertY ? 1f : -1f);

            _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
        }

        private void Update()
        {
            lookPivot.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            UpdateAimPoint();
        }

        private void UpdateAimPoint()
        {
            AimPoint = Physics.Raycast(AimOrigin, AimForward, out RaycastHit hit, maxAimDistance, aimMask)
                ? hit.point
                : AimOrigin + AimForward * maxAimDistance;
        }
    }
}
