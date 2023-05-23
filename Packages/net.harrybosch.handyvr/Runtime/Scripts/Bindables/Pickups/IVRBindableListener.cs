using HandyVR.Player;
using HandyVR.Player.Input;

namespace HandyVR.Bindables.Pickups
{
    /// <summary>
    /// Interface for when User wants to add custom logic to a Pickup, like a gun's trigger pull.
    /// </summary>
    public interface IVRBindableListener
    {
        void Trigger(PlayerHand hand, VRBindable bindable, HandInput.InputWrapper input);
    }
}