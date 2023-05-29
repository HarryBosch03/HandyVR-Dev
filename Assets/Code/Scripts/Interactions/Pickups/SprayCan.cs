using HandyVR.Bindables;
using HandyVR.Bindables.Pickups;
using HandyVR.Interfaces;
using HandyVR.Player;
using HandyVR.Player.Input;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactions.Pickups
{
    [RequireComponent(typeof(VRPickup))]
    public class SprayCan : MonoBehaviour, IVRBindableListener
    {
        [SerializeField] private ParticleSystem spray;
        [SerializeField] private ParticleSystem residue;
        [SerializeField] private float sprayDistance;

        private bool spraying;

        private ParticleSystem.Particle[] sprayBuffer;

        private void Awake()
        {
            sprayBuffer = new ParticleSystem.Particle[spray.main.maxParticles];
        }

        private void Update()
        {
            if (spraying) SpawnResidue();

            switch (spraying)
            {
                case true when !spray.isPlaying:
                    spray.Play();
                    break;
                case false when spray.isPlaying:
                    spray.Stop();
                    break;
            }

            spraying = false;
        }

        private static float ScoringMethod(RaycastHit hit)
        {
            return 1.0f / hit.distance;
        }
        
        private void SpawnResidue()
        {
            var numAlive = spray.GetParticles(sprayBuffer);
            if (numAlive == 0) return;

            var reference = sprayBuffer[Random.Range(0, numAlive)];

            var ray = new Ray(spray.transform.position, reference.velocity);
            var hits = Physics.RaycastAll(ray, sprayDistance);
            if (hits.Length == 0) return;

            if (!HandyVR.Utility.Collections.Best(hits, out var hit, ScoringMethod)) return;

            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position = hit.point;
            residue.Emit(emitParams, 1);
        }

        public void InputCallback(VRHand hand, VRBindable bindable, IVRBindable.InputType type, HandInput.InputWrapper input)
        {
            if (type == IVRBindable.InputType.Trigger && input.Down)
            {
                spraying = true;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!spray) return;
            Gizmos.DrawRay(spray.transform.position, spray.transform.forward * sprayDistance);
        }
    }
}