using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class EditorHelper
{
    [MenuItem("Assets/CreateAtlas")]
    public static void CreateAtlas()
    {
        var selections = Selection.assetGUIDs;

        foreach (var selection in selections)
        {
            var path = AssetDatabase.GUIDToAssetPath(selection);
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            var atlas = new SpriteAtlasAsset();
            atlas.Add(new[] { asset });
            try
            {
                AssetDatabase.CreateAsset(atlas, path + "/[a]" + asset.name + ".spriteatlasv2");
            }
            catch
            {
                // ignored
            }

            AssetDatabase.SaveAssets();
        }

        AssetDatabase.Refresh();
    }
}