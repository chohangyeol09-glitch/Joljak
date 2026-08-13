using System.Collections.Generic;
using CHG.Scripts.CoreSystem.ModuleSystem;
using NKT.Enemy.Modules;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace NKT.FlyingEnemy
{
    public class FlyMovement : MonoBehaviour, IModule, IAgentMovement
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private float arriveThreshold = 0.3f;
        [SerializeField] private float bobAmplitude = 0.3f;
        [SerializeField] private float bobFrequency = 1.5f;
        [SerializeField] private float hoverHeight = 2f;
        [SerializeField] private float riseSpeed = 2f;

        public Vector3 Velocity { get; set; }
        public float Speed { get => speed; set => speed = value; }
        public float StoppingDistance { get; set; }
        public bool IsStopped { get; set; }
        public bool IsArrived => Vector3.Distance(_corePosition, _destination) <= arriveThreshold;

        private Transform _owner;
        private AgentSensor _sensor;
        
        private List<Vector3> _path = new List<Vector3>();
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
            if (Mathf.Approximately(_destination.x, destination.x) &&
                Mathf.Approximately(_destination.z, destination.z))
                return;
            _destination = destination;
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
        }

        //todo: findPath로 경로 계산한걸 update에서 자동으로 움직이게 하고
        //todo: (타켓 기준으로) 높이가 너무 낮을때는 최소 높이 만큼은 올라가게 하기
        public Vector3[] FindPath(Vector3 startPos, Vector3 targetPos)//여기서 뚫여있으면 목표만 반환하고 아니면 경로 계산해서 넘겨주기
        {
            _path.Clear();
            if (_sensor.IsTargetIsInSight3D(targetPos))
            {//경로 계산 로직
                
            }
            else
            {
                _path.Add(targetPos);
            }
            return _path.ToArray();
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

            if (_hasDestination && !IsStopped)
            {
                Vector3 toDestination = _destination - _corePosition;
                if (toDestination.sqrMagnitude > arriveThreshold * arriveThreshold)
                {
                    Vector3 direction = toDestination.normalized;
                    Vector3 move = direction * (speed * Time.deltaTime);
                    _corePosition += move;
                    Velocity = move / Time.deltaTime;
                    _owner.rotation = Quaternion.LookRotation(direction);
                }
                else
                {
                    Velocity = Vector3.zero;
                }
            }
            else
            {
                Velocity = Vector3.zero;
            }

            float bob = Mathf.Sin(Time.time * bobFrequency + _bobPhase) * bobAmplitude;
            _owner.position = _corePosition + Vector3.up * bob;
        }
    }
}
