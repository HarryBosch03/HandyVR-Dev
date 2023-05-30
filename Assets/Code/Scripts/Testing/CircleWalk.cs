using UnityEngine;

namespace Code.Scripts.Testing
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class CircleWalk : MonoBehaviour
    {
        [SerializeField] float radius, speed;

        private Vector3 center;
        private float d;

        private void Awake()
        {
            center = transform.position;
        }

        private void Update()
        {
            var a = d / radius;
            var newPos = center + new Vector3(Mathf.Cos(a), 0.0f, Mathf.Sin(a)) * radius;
            transform.forward = transform.position - newPos;
            transform.position = newPos;
            d += speed * Time.deltaTime;
        }
    }
}
