using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        var prefab = Resources.Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.LogError("$[ResourceManager.Load] Failed to load prefab : {path}");
            return null;
        }
        var go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }
}