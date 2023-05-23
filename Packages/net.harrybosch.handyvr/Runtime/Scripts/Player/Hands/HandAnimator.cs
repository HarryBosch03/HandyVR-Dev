using UnityEngine;

namespace HandyVR.Player.Hands
{
    /// <summary>
    /// Submodule for animating hand.
    /// </summary>
    [System.Serializable]
    public class HandAnimator
    {
        [Tooltip("Amount of smoothing to apply to inputs. Zero == no smooth")]
        [SerializeField] private float smoothing = 0.1f;

        private static readonly int Trigger = Animator.StringToHash("trigger");
        private static readonly int Grip = Animator.StringToHash("grip");

        private float gripValue, triggerValue;
        
        private PlayerHand hand;
        private Animator animator;

        public void Init(PlayerHand hand)
        {
            this.hand = hand;
            animator = hand.GetComponentInChildren<Animator>();
        }

        public void Update()
        {
            float tGripValue, tTriggerValue;
         
            // Overrides target values based on conditions that a different pose may be appropriate.
            // Matches grip pose if something is being held.
            if (hand.BindingController.DetachedBinding)
            {
                tTriggerValue = 1.0f;
                tGripValue = 1.0f;
            }
            // Matches pointing pose if an object is being pointed at.
            else if (hand.BindingController.PointingAt)
            {
                tTriggerValue = 0.0f;
                tGripValue = 1.0f;
            }
            else
            {
                tGripValue = hand.Input.Grip.Value;
                tTriggerValue = hand.Input.Trigger.Value;
            }

            // Move actual animator value towards target, using the smoothing value.
            gripValue += smoothing > 0.0f ? (tGripValue - gripValue) / smoothing * Time.deltaTime : tGripValue;
            triggerValue += smoothing > 0.0f ? (tTriggerValue - triggerValue) / smoothing * Time.deltaTime : tTriggerValue;
            
            animator.SetFloat(Grip, gripValue);
            animator.SetFloat(Trigger, triggerValue);
        }
    }
}