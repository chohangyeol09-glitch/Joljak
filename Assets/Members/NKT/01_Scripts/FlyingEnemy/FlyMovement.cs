using System.Collections.Generic;
using System.Linq;
using DevLib.ModuleSystem;
using NKT.Enemy.Modules;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace NKT.FlyingEnemy
{
    public class FlyMovement : MonoBehaviour, IModule, IAgentMovement
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private float arriveThreshold = 0.3f;
        [SerializeField] private float cornerThreshold = 1f;//중간 경유지는 느슨하게 지나간다
        [SerializeField] private float rotateSpeed = 8f;
        [SerializeField] private float repathThreshold = 0.5f;//목적지가 이만큼 안 변했으면 경로 재계산 생략
        [SerializeField] private float bobAmplitude = 0.3f;
        [SerializeField] private float bobFrequency = 1.5f;
        [SerializeField] private float hoverHeight = 2f;
        [SerializeField] private float riseSpeed = 2f;

        public Vector3 Velocity { get; set; }
        public float Speed { get => speed; set => speed = value; }
        public float StoppingDistance { get; set; }
        public bool IsStopped { get; set; }
        public bool IsArrived => !_hasDestination || _pathIndex >= _path.Count;

        private Transform _owner;
        private AgentSensor _sensor;

        private List<Vector3> _path = new List<Vector3>();
        private int _pathIndex;
        private Vector3 _corePosition;
        private Vector3 _destination;
        private bool _hasDestination;
        private float _bobPhase;
        private float _targetHoverY;
        private bool _hasRisen;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner.transform;
            _sensor = owner.GetModule<AgentSensor>();
            
            _corePosition = transform.position;
            _targetHoverY = _corePosition.y + hoverHeight;
            _destination = _corePosition;
            _bobPhase = Random.Range(0f, Mathf.PI * 2f);
        }

        public void SetDestination(Vector3 destination)
        {
            //이동 중에 거의 같은 곳을 다시 요청하면 경로 재계산을 건너뛴다
            if (_hasDestination && !IsStopped &&
                (destination - _destination).sqrMagnitude < repathThreshold * repathThreshold)
                return;

            _destination = destination;
            FindPath(_corePosition, destination);
            _pathIndex = 0;
            _hasDestination = true;
            IsStopped = false;
        }

        public void Stop()
        {
            IsStopped = true;
            Velocity = Vector3.zero;
            _hasDestination = false;
            _destination = _corePosition;
            _path.Clear();
            _pathIndex = 0;
        }

        //todo: (타켓 기준으로) 높이가 너무 낮을때는 최소 높이 만큼은 올라가게 하기
        private void FindPath(Vector3 startPos, Vector3 targetPos)//뚫여있으면 목표만 넣고 아니면 경로 계산해서 넣기
        {
            _path.Clear();

            if (_sensor.IsTargetIsInSight3D(targetPos))
            {
                _path.Add(targetPos);//직선으로 갈 수 있으면 목표 하나로 충분하다
                return;
            }

            //todo: 시야가 막혔을 때 우회 경로 계산. 그때까지는 직선으로 폴백
            _path.Add(targetPos);
        }

        private void Update()
        {
            if (!_hasRisen)
            {
                _corePosition.y = Mathf.MoveTowards(_corePosition.y, _targetHoverY, riseSpeed * Time.deltaTime);
                _owner.position = _corePosition;
                if (Mathf.Approximately(_corePosition.y, _targetHoverY))
                {
                    _hasRisen = true;
                    _destination = _corePosition;
                }
                return;
            }

            FollowPath();

            float bob = Mathf.Sin(Time.time * bobFrequency + _bobPhase) * bobAmplitude;
            _owner.position = _corePosition + Vector3.up * bob;
        }

        private void FollowPath()
        {
            if (IsStopped || !_hasDestination || _pathIndex >= _path.Count)
            {
                Velocity = Vector3.zero;
                return;
            }

            Vector3 corner = _path[_pathIndex];
            Vector3 toCorner = corner - _corePosition;

            //마지막 지점만 정밀하게, 중간 경유지는 스치듯 지나가게
            bool isLastCorner = _pathIndex == _path.Count - 1;
            float threshold = isLastCorner ? arriveThreshold : cornerThreshold;

            if (toCorner.sqrMagnitude <= threshold * threshold)
            {
                _pathIndex++;
                Velocity = Vector3.zero;
                return;
            }

            Vector3 direction = toCorner.normalized;
            Vector3 move = direction * (speed * Time.deltaTime);
            _corePosition += move;
            Velocity = move / Time.deltaTime;

            //경유지에서 방향이 꺾일 때 뚝 끊기지 않게
            _owner.rotation = Quaternion.Slerp(_owner.rotation, Quaternion.LookRotation(direction),
                rotateSpeed * Time.deltaTime);
        }
    }
}
