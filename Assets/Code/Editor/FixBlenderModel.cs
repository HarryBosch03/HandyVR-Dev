using UnityEditor;

namespace Editor
{
    public sealed class FixBlenderModel : AssetPostprocessor
    {
        private void OnPreprocessModel()
        {
            var importer = assetImporter as ModelImporter;
            if (!importer) return;
            importer.bakeAxisConversion = true;
        }
    }
}
