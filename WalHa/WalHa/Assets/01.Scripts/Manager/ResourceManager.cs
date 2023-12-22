using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager
{
    public T GetResource<T>(string _name)
    {
        var _handle = Addressables.LoadAssetAsync<T>(_name);

        _handle.WaitForCompletion();

        return _handle.Result;
    }

    /// <summary>
    /// Assets/폴더명/파일명.확장자
    /// </summary>
    public T Load<T>(string path) where T : Object 
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
            {
                name = name.Substring(index + 1);
            }
            GameObject obj = Managers.Pool.GetObject(name);
            if (obj != null)
                return obj as T;
        }

        return GetResource<T>(path);
        //return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
    }

    /// <summary>
    /// Assets/랑 확장자는 생략해도 됨
    /// </summary>
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (prefab?.GetComponent<Poolable>() != null)
        {
            return Managers.Pool.Pop(prefab, parent).gameObject;
        }

        GameObject obj = Object.Instantiate(prefab, parent);
        obj.name = prefab.name;
        return obj;
    }
    public void Destroy(GameObject obj)
    {
        if (obj == null)
            return;

        Poolable poolable = obj?.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }
        Object.Destroy(obj);
    }
}
