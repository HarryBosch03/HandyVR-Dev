using System.Collections.Generic;
using HandyVR.Bindables;
using UnityEngine;

namespace HandyVR.Player.Hands
{
    /// <summary>
    /// Submodule used for managing the <see cref="PlayerHand"/>s current binding.
    /// </summary>
    [System.Serializable]
    public class HandBinding : IBindingTarget
    {
        [Tooltip("The radius to search for a pickup when the grab button is pressed")]
        [SerializeField] private float pickupRange = 0.2f;
        [Tooltip("The angle of the cone used to find Pickups to create a Detached Binding")]
        [SerializeField] private float detachedBindingAngle;
        [Tooltip("The magnitude of the force applied to a Detached Binding")]
        [SerializeField] private float detachedBindingForce = 400.0f;
        [Tooltip("The Minimum force applied to the Detached Binding")]
        [SerializeField] private float detachedBindingMinForce = 50.0f;

        private PlayerHand hand;
        private LineRenderer lines;

        private static HashSet<VRBindable> existingDetachedBindings = new();
        public VRBinding ActiveBinding { get; private set; }
        public VRBindable DetachedBinding { get; private set; }
        public VRBindable PointingAt { get; private set; }

        public Vector3 Position => hand.Target.position;
        public Quaternion Rotation => hand.Target.rotation;
        public bool Flipped => hand.Flipped;
        public int Priority => IBindingTarget.HandPriority;
        public GameObject gameObject => hand.gameObject;
        public Transform transform => hand.transform;

        public void Init(PlayerHand hand)
        {
            this.hand = hand;

            lines = hand.GetComponentInChildren<LineRenderer>();
            lines.enabled = false;
        }

        public void Update()
        {
            PointingAt = null;
            lines.enabled = false;

            // Call OnGrip if the Grip Input changed this frame.
            hand.Input.Grip.ChangedThisFrame(OnGrip);
            
            // Bail if binding is invalid.
            if (hand.ActiveBinding) return;
            
            // Update lines to match Detached Binding if we have one.
            if (DetachedBinding)
            {
                lines.SetLine(hand.PointRef.position, DetachedBinding.Rigidbody.position);
            }
            // Draw a line to the object were pointing at if there is one.
            else
            {
                PointingAt = GetPointingAt();
                if (PointingAt)
                {
                    lines.SetLine(hand.PointRef.position, PointingAt.transform.position);
                }
            }
        }

        public void FixedUpdate()
        {
            UpdateDetachedBinding();
        }

        private void UpdateDetachedBinding()
        {
            if (!DetachedBinding) return;

            // If we have an active binding, we cannot also have a detached binding, remove and bail.
            if (DetachedBinding.ActiveBinding)
            {
                RemoveDetachedBinding(false);
                return;
            }
            
            // Calculate force to move the detached binding towards the hand.
            var rb = DetachedBinding.Rigidbody;
            var direction = hand.PointRef.position - rb.position;
            var distance = direction.magnitude;
            direction /= distance;

            // If the object has the speed to get to the players hand this frame, remove the detached binding
            // and create an actual active binding, then bail.
            var delta = detachedBindingForce * Time.deltaTime / rb.mass;
            if (distance < delta * Time.deltaTime)
            {
                RemoveDetachedBinding(true);
                return;
            }

            // Apply the force
            var force = direction * (delta * (distance + detachedBindingMinForce / detachedBindingForce)) - rb.velocity;
            rb.AddForce(force, ForceMode.VelocityChange);
        }

        /// <summary>
        /// Removes the current Detached Binding
        /// </summary>
        /// <param name="bind">Should the detached binding be changed into a active binding?</param>
        private void RemoveDetachedBinding(bool bind)
        {
            if (!DetachedBinding) return;
            
            // Create actual binding.
            if (bind) Bind(DetachedBinding);
            
            existingDetachedBindings.Remove(DetachedBinding);
            DetachedBinding = null;
        }

        /// <summary>
        /// Creates binding between and and Pickup
        /// </summary>
        /// <param name="bindable">The Pickup to bind</param>
        private void Bind(VRBindable bindable)
        {
            // Create binding.
            new VRBinding(bindable, this);
        }

        /// <summary>
        /// Returns a Bindable that is being pointed at the most, while also being in line of sight.
        /// </summary>
        /// <returns>The Object being pointed at the most. Will return null if nothing can be found.</returns>
        private VRBindable GetPointingAt()
        {
            // Method used to find the bindable being pointed at the most.
            float getScore(VRBindable bindable)
            {
                if (existingDetachedBindings.Contains(bindable)) return -1.0f;
                
                // Do not use object we cannot see.
                if (!CanSee(bindable)) return -1.0f;

                var d1 = (bindable.transform.position - hand.PointRef.position).normalized;
                var d2 = hand.PointRef.forward;
                
                // Reciprocate the result to find the object with the smallest dot product.
                return 1.0f / (Mathf.Acos(Vector3.Dot(d1, d2)) * Mathf.Rad2Deg);
            }
            
            return Utility.Collections.Best(VRBindable.All, getScore, 1.0f / (detachedBindingAngle * 2.0f));
        }

        /// <summary>
        /// Is there a clear line of sight from the Index Finger to the Bindable.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Is there a clear line of sight from the Index Finger to the Bindable.</returns>
        private bool CanSee(VRBindable other)
        {
            var ray = new Ray(hand.PointRef.position, other.transform.position - hand.PointRef.position);
            return Physics.Raycast(ray, out var hit) && other.transform.IsChildOf(hit.transform);
        }
        
        private void OnGrip(bool state)
        {   
            DeactivateBinding();
            RemoveDetachedBinding(false);
            
            if (state) OnGripPressed();
        }

        private void DeactivateBinding()
        {
            // Ignore if no active binding.
            if (!ActiveBinding) return;

            // Deactivate the current binding.
            ActiveBinding.Deactivate();

            // Preemptively reset the state of the hand model and rigidbody.
            hand.HandModel.gameObject.SetActive(true);

            hand.Rigidbody.isKinematic = false;
            hand.Rigidbody.velocity = Vector3.zero;
            hand.Rigidbody.angularVelocity = Vector3.zero;
        }

        private void OnGripPressed()
        {
            // Try to pickup the closest object to the hand.
            // If none can be found, try to create a detached binding with
            // whatever is being pointed at.
            var pickup = VRBindable.GetPickup(hand.transform.position, pickupRange);
            if (!pickup)
            {
                TryGetDetachedBinding();
                return;
            }

            Bind(pickup);
        }

        private void TryGetDetachedBinding()
        {
            var pointingAt = GetPointingAt();
            if (!pointingAt) return;
            if (!pointingAt.Rigidbody) return;
            if (existingDetachedBindings.Contains(pointingAt)) return;
            
            if (pointingAt.ActiveBinding && pointingAt.ActiveBinding.target.Priority < Priority)
            {
                pointingAt.ActiveBinding.Deactivate();
            }

            // Create detached binding.
            DetachedBinding = pointingAt;
            // Add to ignore list to stop multiple object having detached bindings.
            existingDetachedBindings.Add(DetachedBinding);

            // Ignore collision between the hand and the new detached binding.
            Utility.Physics.IgnoreCollision(DetachedBinding.gameObject, hand.gameObject, true);
        }
        
        public void OnBindingActivated(VRBinding binding)
        {
            ActiveBinding = binding;
        }

        public void OnBindingDeactivated(VRBinding binding)
        {
            
        }
    }
}