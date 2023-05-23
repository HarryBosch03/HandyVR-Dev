using System.Collections.Generic;
using HandyVR.Bindables.Pickups;
using HandyVR.Player;
using HandyVR.Player.Input;
using UnityEngine;

namespace HandyVR.Bindables
{
    /// <summary>
    /// The base for objects that can be bound, either via hands, sockets, etc...
    /// </summary>
    public abstract class VRBindable : MonoBehaviour
    {
        // ReSharper disable once InconsistentNaming
        private Rigidbody rigidbody_DoNotUse;

        public Rigidbody Rigidbody
        {
            get
            {
                if (!rigidbody_DoNotUse) rigidbody_DoNotUse = GetRigidbody();
                return rigidbody_DoNotUse;
            }
        }
        
        public VRBinding ActiveBinding { get; private set; }

        /// <summary>
        /// The handle that will be used when matching the Binding Pose
        /// TODO Needs Implementation.
        /// </summary>
        public Transform Handle { get; set; }
        public virtual Rigidbody GetRigidbody() => GetComponent<Rigidbody>();
        
        public static readonly List<VRBindable> All = new();

        public Vector3 BindingPosition => ActiveBinding.target.Position;
        public Quaternion BindingRotation => ActiveBinding.target.Rotation;
        public bool BindingFlipped => ActiveBinding.target.Flipped;

        protected virtual void Awake()
        {
            Handle = transform;
        }

        protected virtual void OnEnable()
        {
            All.Add(this);
        }

        protected virtual void OnDisable()
        {
            All.Remove(this);
        }

        public virtual void OnBindingActivated(VRBinding binding)
        {
            ActiveBinding = binding;
        }
        public virtual void OnBindingDeactivated(VRBinding binding) { }

        /// <summary>
        /// Used to find pickups within a range.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static VRBindable GetPickup(Vector3 from, float range)
        {
            VRBindable res = null;
            foreach (var pickup in All)
            {
                var d1 = (pickup.Handle.position - from).sqrMagnitude;
                if (d1 > range * range) continue;
                if (!res)
                {
                    res = pickup;
                    continue;
                }

                var d2 = (res.Handle.position - from).sqrMagnitude;
                if (d1 < d2)
                {
                    res = pickup;
                }
            }

            return res;
        }

        /// <summary>
        /// Calls action on any children of this object with a <see cref="IVRBindableListener"/> on it.
        /// This can be used for functional pickups, like a gun.
        /// </summary>
        /// <param name="hand">The hand providing the input</param>
        /// <param name="action">The action that was called</param>
        public void Trigger(PlayerHand hand, HandInput.InputWrapper action)
        {
            var listeners = GetComponentsInChildren<IVRBindableListener>();
            foreach (var listener in listeners)
            {
                listener.Trigger(hand, this, action);
            }
        }
    }
}