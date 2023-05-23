using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Sharinator.Editor
{
    [CustomEditor(typeof(ProjectSettings))]
    public class ProjectSettingsEditor : UEditor
    {
        public static UEditor cachedEditor;

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/Sharinator", SettingsScope.Project)
            {
                guiHandler = _ =>
                {
                    if (ProjectSettings.Settings == null)
                    {
                        EditorGUILayout.HelpBox("Could not find Project Settings, Please Reinstall Package or Delete Library Folder.", MessageType.Error);
                        return;
                    }
                    CreateCachedEditor(ProjectSettings.Settings, null, ref cachedEditor);
                    cachedEditor.OnInspectorGUI();
                },
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Media Folder", GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight * 2.0f)))
            {
                Process.Start("explorer.exe", ProjectSettings.MediaLocation());
            }
        }
    }
}