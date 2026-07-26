namespace Boss.Core
{
    public class BossAttackCooldown
    {
        private readonly float duration;
        private float remaining;

        public BossAttackCooldown(float duration)
        {
            this.duration = duration;
        }

        public bool IsReady => remaining <= 0f;

        public void Tick(float deltaTime)
        {
            if (remaining > 0f)
            {
                remaining -= deltaTime;
            }
        }

        public void Start()
        {
            remaining = duration;
        }
    }
}
