using UnityEngine;

namespace Code.Scripts.Painting
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class Paintbrush : MonoBehaviour
    {
        [SerializeField] private PaintManager.Brush brush;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private float spacing = 0.05f;

        private Vector3 lastPosition;

        private void Update()
        {
            var l = (transform.position - lastPosition).magnitude;
            var dir = (transform.position - lastPosition) / l;
            for (var d = 0.0f; d < l - spacing; d += spacing)
            {
                var p = transform.position + dir * d;
                brush.position = p;
                foreach (var renderer in renderers) PaintManager.Paint(brush, renderer);
                lastPosition = p;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = brush.color;
            Gizmos.DrawWireSphere(transform.position, brush.radius);
        }
    }
}