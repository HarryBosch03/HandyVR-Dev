using System;
using UnityEditor;
using UnityEngine;

namespace HandyVREditor.Editor
{
    public class Editor : UnityEditor.Editor
    {
        public string KeyFromName(string name)
        {
            return $"{target.GetInstanceID()}.{name}";
        }

        public void Section(string name, Action body, bool fallback = false)
        {
            var foldout = EditorPrefs.GetBool(KeyFromName(name), fallback);
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, name);
            EditorPrefs.SetBool(KeyFromName(name), foldout);

            if (foldout)
                using (new EditorGUI.IndentLevelScope())
                {
                    body();
                }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public static void Div()
        {
            var rect = EditorGUILayout.GetControlRect();
            rect.y += rect.height / 2.0f;
            rect.height = 1.0f;
            EditorGUI.DrawRect(rect, new Color(1.0f, 1.0f, 1.0f, 0.1f));
        }
    }
}