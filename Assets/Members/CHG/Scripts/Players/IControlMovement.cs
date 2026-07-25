using UnityEngine;

namespace Members.CHG.Scripts.Players
{
    public interface IControlMovement
    {
        bool CanManualMove { get; set; }

        /// [월드 공간] 한 번 주면 스스로 잦아드는 힘. 넉백, 점프대. y가 양수면 위로 띄운다
        void AddImpulse(Vector3 impulse);

        /// [월드 공간] 매 FixedUpdate마다 다시 호출해야 유지되는 힘. 돌진, 컨베이어
        void SetDrivenVelocity(Vector3 velocity);

        bool IsGround { get; }

        /// 땅에 있고 이동이 잠기지 않았을 때만 뛴다. 실제로 뛰었는지 반환
        bool TryJump();

        /// [로컬/입력 공간] 원본 WASD 입력. 모듈 안에서 카메라 yaw로 월드로 변환된다
        void SetMovementDirection(Vector2 movementInput);
    }
}