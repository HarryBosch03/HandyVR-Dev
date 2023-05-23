using HandyVR.Switches;
using UnityEngine;

namespace Interactions
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class Shutter : FloatDriven
    {
        [SerializeField] private float smoothTime;
        [SerializeField] private float maxSpeed;

        private float percent;
        private float velocity;
        
        private Vector3 closePosition;

        protected override void Awake()
        {
            base.Awake();
            closePosition = transform.localPosition;
        }

        protected override void Update()
        {
            base.Update();
            
            percent = Mathf.SmoothDamp(percent, Value, ref velocity, smoothTime, maxSpeed);
            transform.localPosition = Vector3.Lerp(Vector3.zero, closePosition, percent);
        }
    }
}
