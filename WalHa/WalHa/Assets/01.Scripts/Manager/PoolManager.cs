using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public class PoolManager
{
    #region Pool
    class Pool
    {
        public GameObject Obj { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject obj, int count = 3)
        {
            Obj = obj;
            Root = new GameObject().transform;
            Root.name = $"{obj.name}_Root";

            for (int i = 0; i < count; i++)
            {
                Push(CreateObj());
            }
        }

        Poolable CreateObj()
        {
            GameObject obj = Object.Instantiate<GameObject>(Obj);
            obj.name = Obj.name;

            Poolable component = obj?.GetComponent<Poolable>();
            if (component == null)
            {
                component = obj.AddComponent<Poolable>();
            }
            return component;
        }

        public void Push(Poolable poolable) //풀 내부로 집어 넣을 때
        {
            if (poolable == null)
                return;

            poolable.transform.SetParent(Root);
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent) //풀에서 꺼낼 때
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = CreateObj();

            poolable.gameObject.SetActive(true);

            if (parent == null)
            {
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;
            }
            poolable.transform.parent = parent;
            poolable.transform.localPosition = Vector3.zero;
            
            poolable.IsUsing = true;

            return poolable;
        }
        public Poolable Pop(Vector3 parent) //풀에서 꺼낼 때
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = CreateObj();
            if (parent == null)
            {
                poolable.transform.SetParent(null);
            }
            poolable.gameObject.SetActive(true);
             poolable.transform.position = parent;
            poolable.IsUsing = true;

            return poolable;
        }
    }
    #endregion
    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root;

    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    public void CreatePool(GameObject obj, int count = 3)
    {
        Pool pool = new Pool();
        pool.Init(obj, count);
        pool.Root.parent = _root;

        _pool.Add(obj.name, pool);
    }

    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        var poolInit = poolable as IPoolable;
        if (poolInit != null)
            poolInit.PoolInit();

        _pool[name].Push(poolable);
    }

    public Poolable Pop(GameObject obj, Transform parent = null, Transform pos = null)
    {
        if (_pool.ContainsKey(obj.name) == false)
        {
            CreatePool(obj);
        }
        return _pool[obj.name].Pop(parent);
    }
    public Poolable Pop(GameObject obj, Vector3 pos)
    {
        if (_pool.ContainsKey(obj.name) == false)
        {
            CreatePool(obj);
        }

        return _pool[obj.name].Pop(pos);
    }
    public GameObject GetObject(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;
        return _pool[name].Obj;
    }
    //public GameObject PoolManaging(GameObject gameObject, Vector2 position, Quaternion rotation)
    //{
    //    Debug.Log(AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject)));
    //    return null;
    //}

    public Poolable PoolManaging(string path, Vector2 position, Quaternion rotation)
    {
        GameObject clone;
        string name = path;
        int index = name.LastIndexOf('/');

        if (index >= 0)
        {
            name = name.Substring(index + 1);
        }

        if (GetObject(name) == null)
            clone = Managers.Resource.Instantiate(path);
        else
        {
            clone = Pop(GetObject(name)).gameObject;
        }

        clone.transform.position = position;
        clone.transform.rotation = rotation;

        return clone?.GetComponent<Poolable>();
    }

    public Poolable PoolManaging(string path, Transform parent = null)
    {
        GameObject clone;
        string name = path;
        int index = name.LastIndexOf('/');

        if (index >= 0)
        {
            name = name.Substring(index + 1);
        }

        if (GetObject(name) == null)
            clone = Managers.Resource.Instantiate(path, parent);
        else
        {
            clone = Pop(GetObject(name), parent).gameObject;
        }

        clone.transform.position = parent.position;

        return clone.GetComponent<Poolable>();
        
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }
}
