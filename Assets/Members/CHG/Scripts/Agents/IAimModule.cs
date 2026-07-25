using UnityEngine;

namespace Members.CHG.Scripts.Agents
{
    /// "이 에이전트가 어디를 겨냥하는가"의 계약.
    /// 플레이어는 카메라로, 적은 AI로 구현하되 소비자(총, 회전)는 출처를 모른다
    public interface IAimModule
    {
        /// [월드] 조준 광선의 출발점. 여기서 AimForward로 쏜다
        Vector3 AimOrigin { get; }

        /// [월드] 조준 방향(pitch 포함). 광선의 방향
        Vector3 AimForward { get; }

        /// [월드] 광선이 실제로 닿는 지점. 발사·회전은 방향이 아니라 이 지점을 향한다
        Vector3 AimPoint { get; }

        /// 하나라도 조준을 요청 중이면 true. 여러 소스가 겹쳐도 키 기반으로 안전하게 센다
        bool IsAiming { get; }
        void RequestAim(object key);
        void ReleaseAim(object key);
    }
}
