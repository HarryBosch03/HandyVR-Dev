using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.Automation
{
    public class CreateEditors : EditorWindow
    {
        [SerializeField] [HideInInspector] private List<MonoScript> scripts = new();
        
        [MenuItem(Reference.Automation + "Create Editors")]
        public static void CreateWizard()
        {
            CreateWindow<CreateEditors>("Create Editors Wizard");
        }

        private void OnGUI()
        {
            var serializeObject = new SerializedObject(this);
            var property = serializeObject.FindProperty("scripts");
            EditorGUILayout.PropertyField(property);

            if (GUILayout.Button("Clear"))
            {
                scripts.Clear();
            }

            if (GUILayout.Button("Add Selection"))
            {
                var selection = Selection.objects;
                foreach (var selected in selection)
                {
                    if (selected is not MonoScript script) continue;
                    scripts.Add(script);
                }
            }

            if (GUILayout.Button("Execute"))
            {
                Execute();
            }
        }

        public void Execute()
        {
            foreach (var script in scripts)
            {
                var path = AssetDatabase.GetAssetPath(script);

                var editorPath = path.Replace("Scripts", "Editor");
                var directory = Path.GetDirectoryName(editorPath);
                Directory.CreateDirectory(Application.dataPath + "/../" + directory);

                var @namespace = GetNamespace(script);
                
                using var stream = new StreamWriter(editorPath);
                stream.Write("using UnityEditor;\nusing UnityEngine;\n");
                if (!string.IsNullOrEmpty(@namespace)) stream.Write($"using {@namespace};\n");
                stream.Write($"\n[CustomEditor(typeof({script.name}))]\npublic class {script.name}Editor : UnityEditor.Editor\n");
                stream.Write("{\n\n}");
            }
            AssetDatabase.Refresh();
            scripts.Clear();
        }

        private static string GetNamespace(MonoScript script)
        {
            var start = script.text.IndexOf("namespace ") + "namespace ".Length;
            if (start < 0) return string.Empty;
            
            var end = script.text.IndexOf('\n', start);
            if (end < 0) return string.Empty;

            return script.text.Substring(start, end - start - 1);
        }
    }
}