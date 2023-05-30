using System;
using System.Collections.Generic;
using Code.Scripts.Animation;
using UnityEngine;

namespace Code.Scripts.AI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class SpiderLegs : MonoBehaviour
    {
        public const int DefaultIKIterations = 10;
        
        [SerializeField] private float stepDistance = 0.1f;
        [SerializeField] private float smoothTime = 0.1f;
        [SerializeField] private float legLift = 0.01f;
        [SerializeField] private List<Leg> legs;

        private void LateUpdate()
        {
            UpdateLegTargets();

            foreach (var leg in legs)
            {
                leg.Animate(legLift, smoothTime);
            }
        }

        private void UpdateLegTargets()
        {
            foreach (var leg in legs)
            {
                leg.target -= transform.forward * Vector3.Dot(transform.forward, leg.target - transform.position);
            }

            var legCenter = Vector3.zero;
            foreach (var leg in legs)
            {
                legCenter += leg.target;
            }

            legCenter /= legs.Count;

            Debug.DrawLine(legCenter, transform.position);

            if ((legCenter - transform.position).magnitude < stepDistance / (legs.Count - 1)) return;

            var dir = (transform.position - legCenter).normalized;
        
            var lastLeg = legs[0];
            for (var i = 1; i < legs.Count; i++)
            {
                var d1 = Vector3.Dot(lastLeg.target - transform.position, dir);
                var d2 = Vector3.Dot(legs[i].target - transform.position, dir);
                if (d2 > d1) continue;
                lastLeg = legs[i];
            }

            lastLeg.SetTarget(transform.position + dir * stepDistance);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;


            if (legs == null) return;

            Gizmos.color = Color.green;
            foreach (var leg in legs)
            {
                Gizmos.DrawLine(leg.root.position, leg.mid.position);
                Gizmos.DrawLine(leg.mid.position, leg.tip.position);
                Gizmos.DrawSphere(leg.target, stepDistance / 4.0f);
            }
        }

        [ContextMenu("Bake Legs")]
        private void BakeLegs()
        {
            legs.Clear();
            foreach (Transform child in transform)
            {
                if (child.name[..3] != "Leg") continue;
                var leg = new Leg(transform, child);
                legs.Add(leg);
            }
        }

        [Serializable]
        public class Leg
        {
            public Transform center;

            public Transform root;
            public Transform mid;
            public Transform tip;

            public IK ik;

            public Vector3 start;
            public Vector3 localOffset;
        
            private float lastTargetChangeTime;
            private Vector3 lastTarget;
            [NonSerialized]public Vector3 target;
            private Vector3 lastOffset;
            private Vector3 offset;

            public Leg(Transform center, Transform root)
            {
                this.center = center;
                this.root = root;

                mid = root.GetChild(0);
                tip = mid.GetChild(0);

                ik = new IK(root, mid, tip).Iterations(DefaultIKIterations);

                start = center.InverseTransformPoint(root.position);
                localOffset = center.InverseTransformVector(tip.position - center.position);
            }

            public void Animate(float legLift, float smoothTime)
            {
                var t = Mathf.Clamp01((Time.time - lastTargetChangeTime) / smoothTime);
                var p = Vector3.Lerp(lastTarget, target, t) + Vector3.up * (Mathf.Sin(t * Mathf.PI) * legLift);
                var offset = Vector3.Lerp(lastOffset, this.offset, t);
            
                ik.Solve(center.TransformPoint(start), p + offset, -center.forward);
            }

            public void SetTarget(Vector3 target)
            {
                lastTarget = this.target;
                this.target = target;

                lastOffset = offset;
                offset = center.TransformVector(localOffset);
            
                lastTargetChangeTime = Time.time;
            }
        }
    }
}