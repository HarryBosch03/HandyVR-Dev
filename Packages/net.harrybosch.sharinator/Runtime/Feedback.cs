using UnityEngine;

namespace Sharinator
{
    public static class Feedback
    {
        private static AudioClip captureSound;
        private static AudioListener listener;
        public static void PlayCaptureSound(float volume)
        {
            if (!listener) listener = Object.FindObjectOfType<AudioListener>();
            if (!captureSound) captureSound = Resources.Load<AudioClip>("CaptureSound");
            AudioSource.PlayClipAtPoint(captureSound, listener.transform.position, volume);
        }
    }
}