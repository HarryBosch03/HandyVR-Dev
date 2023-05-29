using System;
using HandyVR.Bindables;
using HandyVR.Interfaces;
using HandyVR.Player;
using HandyVR.Player.Input;
using UnityEngine;

namespace HandyVR.Switches
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [AddComponentMenu("HandyVR/Float Drivers/Dial", Reference.AddComponentMenuOrder.Submenu)]
    public sealed class VRDial : FloatDriver, IVRHandle
    {
        [SerializeField] private bool limit;
        [SerializeField] private Vector2 range = new(-180.0f, 180.0f);
        [SerializeField] private int steps;
        [SerializeField] private float smoothing = 0.1f;
        
        private Quaternion lastRotation;
        private float tAngle, cAngle;
        
        public override float Value
        {
            get
            {
                if (!limit) return cAngle / 360.0f;

                var p = Mathf.InverseLerp(range.x, range.y, cAngle);
                return steps > 0 ? Mathf.Round(p * steps) / steps : p;
            }   
        }

        public VRBinding ActiveBinding { get; private set; }
        public Rigidbody Rigidbody => null;
        public bool IsValid() => this;

        private void OnEnable()
        {
            IVRBindable.All.Add(this);
        }

        private void OnDisable()
        {
            IVRBindable.All.Remove(this);
        }

        public void OnBindingActivated(VRBinding newBinding)
        {
            ActiveBinding = newBinding;

            lastRotation = newBinding.target.BindingRotation;
        }

        public void OnBindingDeactivated(VRBinding oldBinding)
        {
            
        }

        private void Update()
        {
            UpdatePose();
            ProcessBinding();
        }

        private void ProcessBinding()
        {
            if (!ActiveBinding) return;

            var delta = ActiveBinding.target.BindingRotation * Quaternion.Inverse(lastRotation);
            lastRotation = ActiveBinding.target.BindingRotation;

            delta.ToAngleAxis(out var angle, out var axis);
            tAngle += Vector3.Dot(transform.forward, axis) * angle;
        }

        private void UpdatePose()
        {
            if (limit)
            {
                tAngle = Mathf.Clamp(tAngle, range.x, range.y);
            }

            if (steps > 0)
            {
                var p = limit ? Mathf.InverseLerp(range.x, range.y, cAngle) : cAngle / 360.0f;
                p = Mathf.Round(p * steps) / steps;
                var t = limit ? Mathf.Lerp(range.x, range.y, p) : p * 360.0f;
                cAngle += Mathf.DeltaAngle(t, cAngle) / smoothing * Time.deltaTime;
            }
            else
            {
                cAngle = tAngle;
            }
            
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, cAngle);
        }

        public void InputCallback(VRHand hand, IVRBindable.InputType type, HandInput.InputWrapper input)
        {
            
        }

    }
}
