using System;
using System.Collections.Generic;
using Boss.Core;
using UnityEngine;

namespace Boss.Attacks
{
    public class BossPillarAttack : MonoBehaviour, IBossAttack
    {
        [SerializeField] private string attackId = "Pillar";
        [Header("보스 공격 관련")] [SerializeField] private float range = 2.5f; // 보스가 공격을할 수 있게 인식 범위
        [SerializeField] private float selfDamage = 500f; // 보스에게 넣을 대미지
        [SerializeField] private float bonusDamage = 100f; // 그로기 시간내 기둥 전체 파괴시 보너스 데미지
        [SerializeField] private float windupSeconds = 0.5f; // 
        [SerializeField] private float cooldownSeconds = 2f; // 공격 쿨
        [SerializeField] private float groggyDuration = 10f; // 그로기 시간

        [Header("기둥 관련")] [SerializeField] private int pillarCount = 3; // 활성화될 기둥 수 
        [SerializeField] private Pillar[] pillars; // 활성화할 기둥 모음 (배열인 이유 게임 진행 중 동적으로 변할 이유가 없어서)
        [SerializeField] private GameObject telegraphPrefab; // 기둥이 생성될 곳 경고용 오브제

        private float _windupTimer; // windup 때 얼머나 시간이 지났는지 알기 위해 필요한 변수
        private float _groggyElapsed; // 그로기 때 //
        private int _activeCount; // 활성화된 기둥 수
        private bool _hasActivePillars;
        private BossAttackCooldown _cooldown; // 공격 쿨
        private List<Pillar> _chosenPillars = new List<Pillar>();

        public string AttackId => attackId;
        public float Range => range;
        public bool IsAttackable => _cooldown.IsReady && HasAvailablePillar();

        private void Awake()
        {
            _cooldown = new BossAttackCooldown(cooldownSeconds);
        }

        private bool HasAvailablePillar()
        {
            foreach (var pillar in pillars)
            {
                if (pillar.IsAvailable)
                {
                    return true;
                }
            }

            return false;
        }

        private void Update()
        {
            _cooldown.Tick(Time.deltaTime);
        }

        public void Begin(BossAttackContext context)
        {
            List<Pillar> pool = new List<Pillar>();
            foreach (var pillar in pillars)
            {
                if (pillar.IsAvailable)
                {
                    pool.Add(pillar);
                }
            }
            int count = Mathf.Min(pillarCount, pool.Count);
            _activeCount = count;

            _groggyElapsed = 0f;
            _windupTimer = windupSeconds;
            _chosenPillars.Clear();
            for (int i = 0; i < count; i++)
            {
                int index = UnityEngine.Random.Range(0, pool.Count);
                Pillar picked = pool[index];
                pool.RemoveAt(index);

                GameObject telegraphObject = null;
                AttackTelegraph telegraph = null;

                if (telegraphPrefab != null)
                {
                    var groundPosition = new Vector3(picked.transform.position.x, 0.0001f,
                        picked.transform.position.z);
                    telegraphObject = Instantiate(telegraphPrefab, groundPosition, Quaternion.Euler(90f, 0f, 0f));
                    telegraphObject.TryGetComponent(out telegraph);
                    telegraph?.StartBlinking(windupSeconds);
                }

                _chosenPillars.Add(picked);
                _hasActivePillars = false;

                if (telegraphObject != null)
                {
                    Destroy(telegraphObject, _windupTimer);
                }
            }
        }

        public BossAttackStatus Tick(BossAttackContext context)
        {
            _windupTimer -= Time.deltaTime;
            if (_windupTimer > 0f)
            {
                return BossAttackStatus.Running;
            }

            if (!_hasActivePillars)
            {
                _hasActivePillars = true;
                foreach (var picked in _chosenPillars)
                {
                    picked.Activate();
                    Action handler = null;
                    handler = () =>
                    {
                        context.Self.GetComponent<IDamageable>()?.TakeDamage(selfDamage);
                        picked.Died -= handler;
                        _activeCount--;
                        if (_activeCount <= 0)
                        {
                            context.Self.GetComponent<IDamageable>()?.TakeDamage(bonusDamage);
                        }
                    };
                    picked.Died += handler;
                }
            }

            _groggyElapsed += Time.deltaTime;
            if (_groggyElapsed > groggyDuration || _activeCount <= 0)
            { 
                _cooldown.Start();
                return BossAttackStatus.Completed;
            }


            return BossAttackStatus.Running;
        }
    }
}