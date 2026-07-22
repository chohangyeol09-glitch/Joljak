using DefaultNamespace;
using Members.CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace Members.CHG.Scripts.Players
{
    /// 마우스 입력을 yaw/pitch로 누적해서 LookPivot을 회전시킨다.
    /// 카메라 자체는 시네머신이 이 피벗을 따라다니며 처리한다
    public class PlayerLookModule : MonoBehaviour, IModule
    {
        [SerializeField] private Transform lookPivot;
        [SerializeField] private float sensitivity = 0.15f;
        [SerializeField] private float pitchMin = -40f;
        [SerializeField] private float pitchMax = 70f;
        [SerializeField] private bool invertY;

        private float _yaw;
        private float _pitch;
        private PlayerInputSO _playerInput;

        /// 이동 기준축. pitch를 빼야 위를 보면서 걸어도 앞으로 간다
        public Quaternion YawRotation => Quaternion.Euler(0f, _yaw, 0f);

        /// 조준 방향. 위아래를 겨눠야 하므로 pitch를 포함한다
        public Vector3 AimDirection => lookPivot.forward;

        public void Initialize(ModuleOwner owner)
        {
            _playerInput = (owner as PlayerController).PlayerInput;

            //시작 각도만 Transform에서 한 번 읽는다. 이후로는 쓰기만 한다
            Debug.Assert(lookPivot != null, $"LookPivot is null : {gameObject.name}");
            _yaw = lookPivot.eulerAngles.y;

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
            //마우스 delta는 이미 "이번 프레임에 움직인 양"이라 deltaTime을 곱하지 않는다
            _yaw += lookDelta.x * sensitivity;
            _pitch += lookDelta.y * sensitivity * (invertY ? 1f : -1f);

            _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
        }

        private void Update()
        {
            //누적한 각도를 매번 새로 써넣는다. eulerAngles로 되읽으면 0~360으로 감겨서 Clamp가 깨진다
            lookPivot.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
    }
}
