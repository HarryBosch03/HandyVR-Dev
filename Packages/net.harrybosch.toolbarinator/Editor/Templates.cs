using UnityEditor;

namespace Toolbarinator.Editor
{
    public static class Templates
    {
        private const string TemplateGuid = "ec40a7cbc5cd419a80cafa899deb5c71";

        [MenuItem("Assets/Create/Toolbarinator/New Toolbar Element", priority = 100)]
        public static void NewToolbarElementFromTemplate()
        {
            var templatePath = AssetDatabase.GUIDToAssetPath(TemplateGuid);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewToolbarElement.cs");
        }
    }
}