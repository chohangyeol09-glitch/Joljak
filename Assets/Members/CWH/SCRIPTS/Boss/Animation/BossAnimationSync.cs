using Boss.Core;
using UnityEngine;

namespace Boss.Animation
{
    [RequireComponent(typeof(Animator))]
    public class BossAnimationSync : MonoBehaviour, IBossAnimator
    {
        [SerializeField] private string speedParameter = "Speed";
        [SerializeField] private string deathTrigger = "Die";

        private Animator animator;
        private IMovable mover;
        private bool isDead;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<IMovable>();
        }

        private void Update()
        {
            if (isDead)
            {
                return;
            }

            animator.SetFloat(speedParameter, mover.CurrentSpeed);
        }

        public void PlayAttack(string attackId)
        {
            if (isDead)
            {
                return;
            }

            animator.SetTrigger(attackId);
        }

        public void PlayDeath()
        {
            isDead = true;
            animator.SetTrigger(deathTrigger);
        }
    }
}
