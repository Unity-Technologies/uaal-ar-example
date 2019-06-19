#if UNITY_EDITOR
using UnityEditor;
using LeTai.Asset.TranslucentImage;

public class MigrationTool : EditorWindow
{
    [MenuItem("Tools/Le Tai's Asset/Translucent Image/Migrate")]
    public static void Migrate()
    {
        var old = FindObjectsOfType<TranslucentImage>();
        Undo.RecordObjects(old, "Migrate Translucent Images");
        foreach (TranslucentImage translucentImage in old)
        {
            var oldColor = translucentImage.color;
            translucentImage.spriteBlending = oldColor.a;
            oldColor.a                      = 1;
            translucentImage.color          = oldColor;
            translucentImage.SetAllDirty();
        }
    }
}

#endif