using HandyVR.Bindables;
using HandyVR.Player;
using HandyVR.Player.Input;
using UnityEngine;

namespace HandyVR.Interfaces
{
    public interface IVRBindable : IBehaviour
    {
        public VRBinding ActiveBinding { get; }
        public Rigidbody Rigidbody { get; }
        
        void OnBindingActivated(VRBinding newBinding);
        void OnBindingDeactivated(VRBinding oldBinding);
        void InputCallback(PlayerHand hand, InputType type, HandInput.InputWrapper input);

        bool IsValid();

        public static bool Valid(IVRBindable ivrBindable)
        {
            return ivrBindable != null && ivrBindable.IsValid();
        }
        
        public enum InputType
        {
            Trigger,
        }
    }
}