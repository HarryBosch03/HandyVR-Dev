using System;
using UnityEngine;

namespace Sharinator
{
    public static class Screenshot
    {
        private static ProjectSettings Settings => ProjectSettings.Settings;

        public static void Take()
        {
            var subpath = $"Screenshots/Screenshot-{DateTime.Now:yy.MM.dd-hh.mm.ss}.png";
            var fullpath = ProjectSettings.MediaLocation(subpath);

            ScreenCapture.CaptureScreenshot(fullpath, ScreenCapture.StereoScreenCaptureMode.RightEye);
            Debug.Log($"Captured Screenshot! Save at \"{subpath}\" - [{fullpath}]");
            Feedback.PlayCaptureSound(Settings.captureSoundVolume);
        }
    }
}