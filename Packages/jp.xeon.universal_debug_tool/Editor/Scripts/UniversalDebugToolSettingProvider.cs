using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

public static class UniversalDebugToolSettingProvider
{
    [SettingsProvider]
    public static SettingsProvider CreateProvider()
    {
        var provider = new SettingsProvider("UniversalDebugTool/", SettingsScope.User)
        {
            label = "Universal Debug Tool Setting",
            guiHandler = GUIHandler,
            keywords = new HashSet<string>(new[] { "UniversalDebugToolSetting" })
        };
        return provider;
    }

    private static void GUIHandler(string searchContext)
    {
        var fileGUIDs = AssetDatabase.FindAssets($"t:{nameof(UniversalDebugToolSetting)}");
        var guid = fileGUIDs.FirstOrDefault();
        if (string.IsNullOrEmpty(guid)) return;
        var path = AssetDatabase.GUIDToAssetPath(guid);
        var setting = AssetDatabase.LoadAssetAtPath<UniversalDebugToolSetting>(path);
        var serializedObject = new SerializedObject(setting);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabs"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("labelPrefab"));
    }
}
