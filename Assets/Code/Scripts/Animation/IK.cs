using UnityEngine;

namespace Code.Scripts.Animation
{
    [System.Serializable]
    public class IK
    {
        public const int DefaultIterations = 50;
        
        [SerializeField] [HideInInspector] private Transform[] joints;
        [SerializeField] [HideInInspector] private float[] lengths;

        public int iterations;

        private Vector3[] points;

        public IK(params Transform[] joints)
        {
            this.joints = joints;
            lengths = new float[joints.Length - 1];
            for (var i = 0; i < lengths.Length; i++)
            {
                lengths[i] = (joints[i + 1].position - joints[i].position).magnitude;
            }
        }

        public IK Iterations(int val)
        {
            iterations = val;
            return this;
        }

        private void Pass(Vector3 point, System.Func<int, int> index, System.Func<int, float> lengths)
        {
            points[index(points.Length - 1)] = point;

            for (var i = points.Length - 2; i >= 0; i--)
            {
                var dir = (points[index(i)] - points[index(i + 1)]).normalized;
                points[index(i)] = points[index(i + 1)] + dir * lengths(i);
            }
        }

        public void Solve(Vector3 start, Vector3 end, Vector3 hint)
        {
            points = new Vector3[joints.Length];
            for (var i = 0; i < joints.Length; i++)
            {
                points[i] = joints[i].position;
            }

            for (var i = 0; i < iterations; i++)
            {
                Pass(end, j => j, j => lengths[j]);
                Pass(start, j => points.Length - j - 1, j => lengths[lengths.Length - j - 1]);
            }

            for (var i = 1; i < points.Length - 1; i++)
            {
                var a = points[i - 1];
                var b = points[i];
                var c = points[i + 1];

                var n = (c - a).normalized;
                var t = Vector3.Cross(n, -Vector3.Cross(n, hint).normalized).normalized;

                var v = b - a;
                var nl = Vector3.Dot(v, n);
                var tl = (v - n * nl).magnitude;

                points[i] = a + n * nl + t * tl;
            }
            
            var tangent = Vector3.Cross((points[0] - end).normalized, hint).normalized;
            for (var i = 0; i < joints.Length - 1; i++)
            {
                joints[i].rotation = Quaternion.LookRotation(points[i + 1] - points[i], tangent) *
                                     Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }

            for (var i = 0; i < joints.Length; i++)
            {
                joints[i].position = points[i];
            }
        }
    }
}