using System.Collections;
using Boss.Core;
using UnityEngine;

namespace Boss.Animation
{
    [RequireComponent(typeof(Outline))]
    public class BossOutlineController : MonoBehaviour, IBossOutline
    {
        [SerializeField] private float blinkInterval = 0.1f;

        private Outline outline;
        private Coroutine blinkRoutine;

        private void Awake()
        {
            outline = GetComponent<Outline>();
            outline.enabled = true;
        }

        public void PlayAttackWarning(float duration)
        {
            if (blinkRoutine != null)
            {
                StopCoroutine(blinkRoutine);
            }

            blinkRoutine = StartCoroutine(BlinkRoutine(duration));
        }

        private IEnumerator BlinkRoutine(float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                outline.enabled = !outline.enabled;
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            outline.enabled = true;
            blinkRoutine = null;
        }
    }
}
