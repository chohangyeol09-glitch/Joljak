using UnityEngine;

namespace Boss.Attacks
{
    [RequireComponent(typeof(LineRenderer))]
    public class BeamLineVisual : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 1f;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
        }

        private void Update()
        {
            if (!lineRenderer.enabled)
            {
                return;
            }

            lineRenderer.material.mainTextureOffset += new Vector2(scrollSpeed * Time.deltaTime, 0f);
        }

        public void ShowBeam(Vector3 origin, Vector3 endPoint)
        {
            lineRenderer.enabled = true;
            UpdateBeam(origin, endPoint);
        }

        public void UpdateBeam(Vector3 origin, Vector3 endPoint)
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, endPoint);
        }

        public void HideBeam()
        {
            lineRenderer.enabled = false;
        }
    }
}