using System.Collections;
using UnityEngine;

namespace Boss.Attacks
{
    public class AttackTelegraph : MonoBehaviour
    {
        [SerializeField] private float blinkInterval = 0.1f;

        private Renderer[] renderers;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        public void StartBlinking(float duration)
        {
            StartCoroutine(BlinkRoutine(duration));
        }

        private IEnumerator BlinkRoutine(float duration)
        {
            float elapsed = 0f;
            bool visible = true;

            while (elapsed < duration)
            {
                visible = !visible;
                SetVisible(visible);
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            SetVisible(true);
        }

        private void SetVisible(bool visible)
        {
            foreach (var target in renderers)
            {
                target.enabled = visible;
            }
        }
    }
}
