using System;
using System.Collections.Generic;
using UnityEngine;

namespace HandyVR.Bindables.Pickups
{
    /// <summary>
    /// Physics object that can be picked up by the player.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [AddComponentMenu("HandyVR/Basic Pickup", Reference.AddComponentMenuOrder.Components)]
    public sealed class VRPickup : VRBindable
    {
        [Tooltip("A optional binding type used for sockets.")]
        [SerializeField] private VRBindingType bindingType;
        [Tooltip("Positional offset for when the object is held.")]
        [SerializeField] private Vector3 boundTranslation;
        [Tooltip("Rotational offset for when the object is held.")]
        [SerializeField] private Quaternion boundRotation = Quaternion.identity;
        [Tooltip("Should the object flip over when in different hands.")]
        [SerializeField] private bool flipWithHand;
        [Tooltip("If the object should flip, what addition rotation should be applied.")]
        [SerializeField] private Quaternion additionalFlipRotation = Quaternion.identity;

        private readonly List<ColliderData> colliderData = new();

        public event Action BindingActivatedEvent;
        public event Action BindingDeactivatedEvent;
        
        
        // Physics material for when the object is held, this is to stop wierd jitter caused from bouncy objects.
        private static PhysicMaterial overridePhysicMaterial;
        private static PhysicMaterial OverridePhysicMaterial
        {
            get
            {
                // Lazy Initialization.
                if (overridePhysicMaterial) return overridePhysicMaterial;
                
                overridePhysicMaterial = new PhysicMaterial();
                overridePhysicMaterial.name = "VR Pickup | Override Physics Material [SHOULD ONLY BE ON WHILE OBJECT IS HELD]";
                overridePhysicMaterial.bounciness = 0.0f;
                overridePhysicMaterial.dynamicFriction = 0.0f;
                overridePhysicMaterial.staticFriction = 0.0f;
                overridePhysicMaterial.bounceCombine = PhysicMaterialCombine.Multiply;
                overridePhysicMaterial.frictionCombine = PhysicMaterialCombine.Multiply;
                return overridePhysicMaterial;
            }
        }
        
        public VRBindingType BindingType => bindingType;
        
        public override void OnBindingActivated(VRBinding binding)
        {
            base.OnBindingActivated(binding);
            
            // Cache each colliders material and override it with the special held material.
            foreach (var data in colliderData)
            {
                data.lastMaterial = data.collider.sharedMaterial;
                data.collider.sharedMaterial = OverridePhysicMaterial;
            }
            
            BindingActivatedEvent?.Invoke();
        }

        public override void OnBindingDeactivated(VRBinding binding)
        {
            // Restore original physics material to each collider from cache.
            foreach (var data in colliderData)
            {
                data.collider.sharedMaterial = data.lastMaterial;
            }
            
            BindingDeactivatedEvent?.Invoke();
        }

        // Override to add a Rigidbody if there isn't one rather than it be optional.
        public override Rigidbody GetRigidbody() => gameObject.GetOrAddComponent<Rigidbody>();

        protected override void Awake()
        {
            base.Awake();

            // Force rigidbody to use continuous collision for stability when held.
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                colliderData.Add(new ColliderData(collider));
            }
        }

        private void FixedUpdate()
        {
            MoveIfBound();
        }

        private void MoveIfBound()
        {
            if (!ActiveBinding) return;

            // Calculate force needed to be applied to translate the object from its current position
            // to the binding position ( + offset ) with zero velocity.
            var force = (BindingPosition + boundTranslation - Rigidbody.position) / Time.deltaTime - Rigidbody.velocity;
            Rigidbody.AddForce(force, ForceMode.VelocityChange);

            // Calculate the finalized rotational offset.
            var offset = boundRotation.normalized;
            if (flipWithHand && BindingFlipped) offset = additionalFlipRotation;
            
            // Calculate the angle axis rotation needed to move from our current rotation to the target ( + offset )
            var delta = BindingRotation * offset * Quaternion.Inverse(Rigidbody.rotation);
            delta.ToAngleAxis(out var angle, out var axis);
            
            // Calculate a torque to move the rigidbody to the target rotation with zero angular velocity.
            var torque = axis * (angle * Mathf.Deg2Rad / Time.deltaTime) - Rigidbody.angularVelocity;
            Rigidbody.AddTorque(torque, ForceMode.VelocityChange);
        }

        /// <summary>
        /// Class for caching collider data for restoration when a pickup is released.
        /// </summary>
        private class ColliderData
        {
            public Collider collider;
            public PhysicMaterial lastMaterial;

            public ColliderData(Collider collider)
            {
                this.collider = collider;
                lastMaterial = collider.sharedMaterial;
            }
        }
    }
}