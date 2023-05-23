using HandyVR.Bindables;
using UnityEngine;

namespace HandyVR
{
    public interface IBindingTarget
    {
        public const int HandPriority = 1;
        public const int SocketPriority = 0;
        
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public bool Flipped { get; }
        public int Priority { get; }
        
        // ReSharper disable once InconsistentNaming
        public GameObject gameObject { get; }
        // ReSharper disable once InconsistentNaming
        public Transform transform { get; }

        public void OnBindingActivated(VRBinding binding);
        public void OnBindingDeactivated(VRBinding binding);
    }
}