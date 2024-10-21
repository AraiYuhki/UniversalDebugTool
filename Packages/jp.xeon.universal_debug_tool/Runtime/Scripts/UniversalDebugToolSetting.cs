using System;
using System.Collections.Generic;
using UnityEngine;
using Xeon.UniversalUI;

[CreateAssetMenu(fileName = "PrefabDictionary", menuName = "XDebugTool/PrefabDictionary")]
public class UniversalDebugToolSetting : ScriptableObject
{
    [SerializeField]
    private List<GameObject> prefabs;
    [SerializeField]
    private DebugMenuLabel labelPrefab;

    private Dictionary<Type, IUniversalControllable> prefabDictionary = new();

    private void OnEnable()
    {
        prefabDictionary.Clear();
        foreach (var prefab in prefabs)
        {
            var component = prefab.GetComponent<IUniversalControllable>();
            if (component != null)
                prefabDictionary[component.GetType()] = component;
        }
    }

    public T Instantiate<T>(Transform parent = null) where T : UnityEngine.Object, IUniversalControllable
    {
        var type = typeof(T);
        if (prefabDictionary.TryGetValue(type, out var prefab))
            return GameObject.Instantiate((T)prefab, parent);
        return null;
    }

    public DebugMenuLabel InstantiateLabel(Transform parent = null)
    {
        return GameObject.Instantiate(labelPrefab, parent);
    }
}
