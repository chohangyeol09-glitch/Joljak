namespace Boss.Core
{
    public interface IBossAnimator
    {
        void PlayAttack(string attackId);
        void PlayTrigger(string triggerName);
        void PlayDeath();
    }
}
