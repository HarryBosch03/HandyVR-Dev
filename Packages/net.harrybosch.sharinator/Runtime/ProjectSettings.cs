using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sharinator
{
    public sealed class ProjectSettings : ScriptableObject
    {
        [SerializeField] private InputAction screenshot;
        [Range(0.0f, 1.0f)] public float captureSoundVolume = 0.2f;

        public static string MediaLocation(string subpath = "")
        {
#if UNITY_EDITOR
            var path = Path.GetFullPath(Path.Combine(Application.dataPath, "../Sharinator", subpath));
#else
            var path = Path.GetFullPath(Path.Combine(Application.dataPath, "Sharinator", subpath));
#endif
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return path;
        }

        private void OnEnable()
        {
            Bind(screenshot, Screenshot.Take);
        }

        private void Bind(InputAction action, Action callback)
        {
            action.Enable();
            action.performed += _ => callback();
        }

        private static ProjectSettings cachedSettingsDoNotUse;

        public static ProjectSettings Settings
        {
            get
            {
                if (!cachedSettingsDoNotUse)
                {
                    cachedSettingsDoNotUse = Resources.Load<ProjectSettings>("Sharinator Project Settings");
                }

                return cachedSettingsDoNotUse;
            }
        }
    }
}