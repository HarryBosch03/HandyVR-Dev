using HandyVR.Player;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace HandyVR.Editor.Player
{
    [CustomEditor(typeof(PlayerHand))]
    public class PlayerHandEditor : UEditor
    {
        private PlayerHand hand;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying) return;
            
            hand = target as PlayerHand;
            if (!hand) return;

            var hasIssues = false;
            hasIssues = !HasPointReference() || hasIssues;
            hasIssues = !HasModel() || hasIssues;

            if (!hasIssues)
            {
                EditorGUILayout.HelpBox("No Problems here :D", MessageType.Info);
            }
        }

        private bool HasPointReference()
        {
            if (hand.transform.DeepFind("Point Ref")) return true;
            EditorGUILayout.HelpBox("PlayerHand must have a child named \"Point Ref,\" as a reference as to where the index finger is pointing", MessageType.Error);
            return false;
        }

        private bool HasModel()
        {
            if (hand.transform.DeepFind("Model")) return true;
            EditorGUILayout.HelpBox("PlayerHand must have a child named \"Model,\" that contains the hand's renderers and colliders.", MessageType.Error);

            return false;
        }
    }
}