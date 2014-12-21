using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility
{
    [MenuItem("Assets/Create/Asset From Selected Script")]
    public static void CreateAssetFromSelectedScript()
    {
        MonoScript ms = Selection.objects[0] as MonoScript;

        string path = EditorUtility.SaveFilePanel("Save location", "Assets", "New " + ms.name, "asset");

        if (string.IsNullOrEmpty(path))
            return;

        //Get project relative path and ensure path is within project
        var projectRelative = FileUtil.GetProjectRelativePath(path);
        if (string.IsNullOrEmpty(projectRelative))
        {
            EditorUtility.DisplayDialog("Error", "Please select somewhere within your assets folder.", "OK");
            return;
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(projectRelative);

        ScriptableObject scriptableObject = ScriptableObject.CreateInstance(ms.GetClass());
        AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = scriptableObject;
    }

    [MenuItem("Assets/Create/Asset From Selected Script", true)]
    public static bool CreateAssetFromSelectedScript_Validator()
    {
        if (Selection.objects != null &&
                Selection.objects.Length == 1 &&
                Selection.objects[0] is MonoScript &&
                (Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(ScriptableObject)) &&
            !(Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(UnityEditor.Editor))
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}