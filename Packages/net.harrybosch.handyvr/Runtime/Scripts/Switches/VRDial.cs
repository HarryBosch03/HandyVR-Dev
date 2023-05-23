using HandyVR.Bindables;
using UnityEngine;

namespace HandyVR.Switches
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [AddComponentMenu("HandyVR/Float Drivers/Dial", Reference.AddComponentMenuOrder.Submenu)]
    public sealed class VRDial : FloatDriver
    {
        private VRHandle handle;
        private new Rigidbody rigidbody;

        private float angle;
        private float lastRotation;
        
        // Remap the local position of the handle so its offset along the Y axis represents a value between 0 and 1.
        public override float Value => angle / 360.0f;

        private void Awake()
        {
            // Autocomplete object if partially setup.
            rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            
            handle = GetComponentInChildren<VRHandle>();
            SetupHandle();
        }

        private void FixedUpdate()
        {
            angle += Mathf.DeltaAngle(rigidbody.rotation.eulerAngles.y, lastRotation);
            lastRotation = rigidbody.rotation.eulerAngles.y;
        }

        private void SetupHandle()
        {
            var rigidbody = handle.gameObject.GetOrAddComponent<Rigidbody>();
            rigidbody.mass = 0.02f;
            rigidbody.angularDrag = 6.0f;
            rigidbody.useGravity = false;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            
            var joint = handle.gameObject.GetOrAddComponent<ConfigurableJoint>();
            joint.connectedBody = this.rigidbody;
            
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Free;

            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = Vector3.zero;
        }
    }
}
