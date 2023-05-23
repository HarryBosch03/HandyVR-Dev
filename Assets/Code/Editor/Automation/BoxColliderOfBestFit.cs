using UnityEditor;
using UnityEngine;

namespace Editor.Automation
{
    public static class BoxColliderOfBestFit
    {
        [MenuItem("Automation/Actions/Box Collider of Best Fit")]
        public static void Execute()
        {
            if (Selection.gameObjects == null) return;
            if (Selection.gameObjects.Length == 0) return;

            foreach (var sel in Selection.gameObjects)
            {
                var mc = sel.GetComponent<MeshCollider>();
                if (!mc) continue;

                var bounds = mc.sharedMesh.bounds;
                var bc = sel.AddComponent<BoxCollider>();
                bc.center = bounds.center;
                bc.size = bounds.size;
            }
        }
    }
}