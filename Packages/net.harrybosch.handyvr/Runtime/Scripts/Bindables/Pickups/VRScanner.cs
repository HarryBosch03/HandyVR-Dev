using HandyVR.Player;
using HandyVR.Player.Input;
using HandyVR.Switches;
using TMPro;
using UnityEngine;

namespace HandyVR.Bindables.Pickups
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class VRScanner : MonoBehaviour, IVRBindableListener
    {
        [SerializeField] private Transform scannerTarget;
        [SerializeField] private float radius = 0.1f;

        [Space] 
        [SerializeField] private string template;
        [SerializeField] private TMP_Text uiText;

        public void Trigger(PlayerHand hand, VRBindable bindable, HandInput.InputWrapper input)
        {
            uiText.text = string.Empty;
            
            if (!input.Down) return;   
            
            var ray = new Ray(scannerTarget.position, scannerTarget.forward);
            if (!Physics.SphereCast(ray, radius, out var hit)) return;

            var driver = hit.transform.GetComponentInParent<FloatDriver>();
            if (!driver) return;

            uiText.text = string.Format(template, driver.Value.ToString("G2"));
        }
    }
}
