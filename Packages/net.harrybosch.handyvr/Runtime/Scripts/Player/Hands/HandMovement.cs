using UnityEngine;

namespace HandyVR.Player.Hands
{
    /// <summary>
    /// Submodule for PlayerHand that manages the movement and collision.
    /// </summary>
    [System.Serializable]
    public class HandMovement
    {
        [Tooltip("Magnitude of the rumble used when the hand is dragged along a surface")]
        [SerializeField] [Range(0.0f, 1.0f)] private float rumbleMagnitude;
        [Tooltip("How much of the calculated force is applied per frame. Lower numbers result in a smoother result but can cause rubber-banding.")]
        [SerializeField] [Range(0.0f, 1.0f)] private float forceScaling = 1.0f;

        private PlayerHand hand;
        
        private bool wasBound;
        private Vector3 boundOffsetTranslation;
        private Quaternion boundOffsetRotation;

        public void Init(PlayerHand hand)
        {
            this.hand = hand;
            foreach (var collider in hand.Colliders)
            {
                collider.isTrigger = false;
            }

            var rb = hand.Rigidbody;
            rb.useGravity = false;
            rb.angularDrag = 0.0f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.mass = 0.01f;
        }

        public void MoveTo(Vector3 newPosition, Quaternion newRotation)
        {
            if (hand.ActiveBinding != wasBound)
            {
                BindingChanged(hand.ActiveBinding && !wasBound);
            }
            wasBound = hand.ActiveBinding;
            
            // If the hand has a binding, just teleport hand to tracked position, the
            // bound object will have their own collision.
            var rb = hand.Rigidbody;
            if (hand.ActiveBinding)
            {
                rb.isKinematic = true;
                return;
            }

            rb.isKinematic = false;
            rb.centerOfMass = Vector3.zero;

            // Add a force that effectively cancels out the current velocity, and translates the hand to the target position.
            // Using a force instead is purely for collision and stability, MovePosition ended up causing horrific desync.
            var force = (newPosition - rb.position) / Time.deltaTime - rb.velocity;
            rb.AddForce(force * forceScaling, ForceMode.VelocityChange);

            // Do the same with a torque, match the current target rotation.
            var delta = newRotation * Quaternion.Inverse(rb.rotation);
            delta.ToAngleAxis(out var angle, out var axis);
            var torque = axis * (angle * Mathf.Deg2Rad / Time.deltaTime) - rb.angularVelocity;
            rb.AddTorque(torque * forceScaling, ForceMode.VelocityChange);
        }

        private void BindingChanged(bool down)
        {
            var other = hand.ActiveBinding.bindable.transform;
            if (down)
            {
                boundOffsetTranslation = other.InverseTransformPoint(hand.transform.position);
                boundOffsetRotation = hand.transform.rotation * Quaternion.Inverse(other.rotation);
            }
            else
            {
                hand.transform.position = other.TransformPoint(boundOffsetTranslation);
                hand.transform.rotation = other.rotation * boundOffsetRotation;
            }
        }

        public void OnCollision(Collision collision)
        {
            hand.Input.Rumble(rumbleMagnitude, 0.0f);
        }
    }
}