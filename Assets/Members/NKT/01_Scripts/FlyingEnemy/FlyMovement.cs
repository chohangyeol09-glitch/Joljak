using CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace NKT.FlyingEnemy
{
    public class FlyMovement : MonoBehaviour, IModule, IFlyMovement
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private float arriveThreshold = 0.3f;
        [SerializeField] private float bobAmplitude = 0.3f;
        [SerializeField] private float bobFrequency = 1.5f;
        [SerializeField] private float hoverHeight = 2f;
        [SerializeField] private float riseSpeed = 2f;

        public Vector3 Velocity { get; set; }
        public float Speed { get => speed; set => speed = value; }
        public bool IsStopped { get; set; }
        public bool IsArrived => Vector3.Distance(_corePosition, _destination) <= arriveThreshold;

        private Vector3 _corePosition;
        private Vector3 _destination;
        private bool _hasDestination;
        private float _bobPhase;
        private float _targetHoverY;
        private bool _hasRisen;

        public void Initialize(ModuleOwner owner)
        {
            _corePosition = transform.position;
            _targetHoverY = _corePosition.y + hoverHeight;
            _destination = _corePosition;
            _bobPhase = Random.Range(0f, Mathf.PI * 2f);
        }

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
            _hasDestination = true;
            IsStopped = false;
        }

        private void Update()
        {
            if (!_hasRisen)
            {
                _corePosition.y = Mathf.MoveTowards(_corePosition.y, _targetHoverY, riseSpeed * Time.deltaTime);
                transform.position = _corePosition;
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
                    transform.rotation = Quaternion.LookRotation(direction);
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
            transform.position = _corePosition + Vector3.up * bob;
        }
    }
}
