using System;
using HandyVR.Bindables;
using HandyVR.Player.Hands;
using HandyVR.Player.Input;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace HandyVR.Player
{
    /// <summary>
    /// Main controller for the Players Hands.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [AddComponentMenu("HandyVR/Hand", Reference.AddComponentMenuOrder.Components)]
    public sealed class PlayerHand : MonoBehaviour
    {
        [Space]
        [SerializeField] private Chirality chirality;
        [Tooltip("Chirality of the hand model used.")]
        [SerializeField] private Chirality defaultHandModelChirality;
        [Tooltip("Axis to flip the hand model on if there is a chiral mismatch")]
        [SerializeField] private Vector3 flipAxis = Vector3.right;

        [Space] 
        [SerializeField] private HandMovement movement;
        [SerializeField] private HandBinding binding;
        [SerializeField] private HandAnimator animator;

        private Transform pointRef;

        private HandInput input;

        public HandInput Input => input;
        public HandBinding BindingController => binding;
        public HandMovement Movement => movement;
        public Transform HandModel { get; private set; }

        public VRBinding ActiveBinding => binding.ActiveBinding;
        public Rigidbody Rigidbody { get; private set; }
        public Transform Target { get; private set; }
        public Transform PointRef => pointRef ? pointRef : transform;
        public Collider[] Colliders { get; private set; }
        public bool Flipped => chirality != defaultHandModelChirality;

        private void Awake()
        {
            // Clear Parent to stop the transform hierarchy from fucking up physics.
            // Group objects to keep hierarchy neat.
            Target = transform.parent;
            Utility.Scene.BreakHierarchyAndGroup(transform);
            
            Func<XRController> controller = chirality switch
            {
                Chirality.Left => () => XRController.leftHand,
                Chirality.Right => () => XRController.rightHand,
                _ => throw new ArgumentOutOfRangeException()
            };

            // Create input module with correct controller.
            input = new HandInput(controller);
            
            Rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
            Colliders = GetComponentsInChildren<Collider>();

            // Initialize Submodules.
            movement.Init(this);
            binding.Init(this);
            animator.Init(this);

            // Cache Hierarchy.
            pointRef = transform.DeepFind("Point Ref");
            HandModel = transform.DeepFind("Model");
            
            // Flip hand if chiral mismatch.
            if (Flipped)
            {
                var scale = Vector3.Reflect(Vector3.one, flipAxis.normalized);
                HandModel.localScale = scale;
            }
        }

        private void FixedUpdate()
        {
            // Update Submodules.
            movement.MoveTo(Target.position, Target.rotation);
            binding.FixedUpdate();
        }

        private void Update()
        {
            // Update Submodules.
            Input.Update();
            
            // Update Targets Pose.
            Target.position = Input.Position;
            Target.rotation = Input.Rotation;
            
            if (ActiveBinding)
            {
                // Hide hand model if we are bound to something
                HandModel.gameObject.SetActive(false);
                
                // Pass inputs to bound object.
                ActiveBinding.bindable.Trigger(this, Input.Trigger);
            }
            else
            {
                // Show hand and animate if unbound.
                HandModel.gameObject.SetActive(true);
                animator.Update();
            }
        }

        private void LateUpdate()
        {
            // Update Submodules.
            binding.Update();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Callback for movement collision.
            movement.OnCollision(collision);
        }

        public enum Chirality
        {
            Left,
            Right,
        }
    }
}