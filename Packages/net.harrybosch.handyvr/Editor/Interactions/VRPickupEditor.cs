using HandyVR.Bindables.Pickups;
using UnityEditor;
using static UnityEditor.EditorGUILayout;
using static UnityEngine.GUILayout;

namespace HandyVREditor.Editor.Interactions
{
    [CustomEditor(typeof(VRPickup))]
    public class VRPickupEditor : Editor
    {
        private bool handlesControlBoundOffset;
        private bool handlesControlFlipped;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            Section("Socket Settings", () => { PropertyField(serializedObject.FindProperty("bindingType")); });

            Section("Bound Pose Settings", () =>
            {
                PropertyField(serializedObject.FindProperty("boundTranslation"));
                PropertyField(serializedObject.FindProperty("boundRotation"));
                PropertyField(serializedObject.FindProperty("flipWithHand"));
                PropertyField(serializedObject.FindProperty("additionalFlipRotation"));
            });

            Section("Editor Actions", () =>
            {
                if (Button("Edit Bound Offset"))
                {
                    handlesControlBoundOffset = !handlesControlBoundOffset;
                }

                if (Button("Edit Flipped Offset"))
                {
                    handlesControlFlipped = !handlesControlFlipped;
                }
            });

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
        }
    }
}