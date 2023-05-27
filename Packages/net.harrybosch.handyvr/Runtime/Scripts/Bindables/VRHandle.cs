using UnityEngine;

namespace HandyVR.Bindables
{
    /// <summary>
    /// A handle that can be added to a child of a physics object with constraints,
    /// allowing for complex behaviours like hinges or sliders.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [AddComponentMenu("HandyVR/Handle", Reference.AddComponentMenuOrder.Components)]
    public sealed class VRHandle : VRBindable
    {
        private bool wasBound;
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        
        // Look for Rigidbody in parents as well.
        public override Rigidbody GetRigidbody() => GetComponentInParent<Rigidbody>();

        public override void OnBindingActivated(VRBinding binding)
        {
            base.OnBindingActivated(binding);

            lastPosition = binding.target.BindingPosition;
            lastRotation = binding.target.BindingRotation;
        }

        private void FixedUpdate()
        {
            if (!ActiveBinding) return;

            // Match the hands position through a simple spring damper, applied to the handles parent at the handles position.
            var diff = (BindingPosition - lastPosition);
            var pointVelocity = Rigidbody.GetPointVelocity(Handle.position);
            var force = (diff / Time.deltaTime - pointVelocity) / Time.deltaTime;
            
            Rigidbody.AddForce(force, ForceMode.Acceleration);
            
            var delta = BindingRotation * Quaternion.Inverse(lastRotation.normalized);
            delta.ToAngleAxis(out var angle, out var axis);
            
            // Calculate a torque to move the rigidbody to the target rotation with zero angular velocity.
            var torque = (axis * (angle * Mathf.Deg2Rad / Time.deltaTime) - Rigidbody.angularVelocity) / Time.deltaTime;
            Rigidbody.AddTorque(torque, ForceMode.Acceleration);

            lastPosition = BindingPosition;
            lastRotation = BindingRotation;
        }
    }
}