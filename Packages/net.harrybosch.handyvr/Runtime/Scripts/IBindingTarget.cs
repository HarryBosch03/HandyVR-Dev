using HandyVR.Bindables;
using UnityEngine;

namespace HandyVR
{
    public interface IBindingTarget
    {
        public const int HandPriority = 1;
        public const int SocketPriority = 0;
        
        public Vector3 BindingPosition { get; }
        public Quaternion BindingRotation { get; }
        public bool IsBindingFlipped { get; }
        public int BindingPriority { get; }
        
        // ReSharper disable once InconsistentNaming
        public GameObject gameObject { get; }
        // ReSharper disable once InconsistentNaming
        public Transform transform { get; }

        public void OnBindingActivated(VRBinding binding);
        public void OnBindingDeactivated(VRBinding binding);
    }
}