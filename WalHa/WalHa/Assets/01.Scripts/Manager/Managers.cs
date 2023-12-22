using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers instance;

    public static Managers Instance
    {
        get
        {
            Init();
            return instance;
        }
    }

    SoundManager _sound = new SoundManager();
    ScenesManager _scene = new ScenesManager();
    ResourceManager _resource = new ResourceManager();
    PoolManager _pool = new PoolManager();

    public float MasterVolume = 1f;
    public float SfxVolume = 1f;
    public float BgmVolume = 1f;

    public static SoundManager Sound { get { return Instance._sound; } }
    public static ScenesManager Scene { get { return Instance._scene; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }

    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        Sound.SetVolume();
    }

    static void Init()
    {
        if (instance == null)
        {
            GameObject obj = GameObject.Find("@Managers");  
            if (obj == null)
            {
                obj = new GameObject { name = "@Managers" };
                instance = obj.AddComponent<Managers>();
            }
            DontDestroyOnLoad(obj);
            instance = obj.GetComponent<Managers>();

            instance._pool.Init();
            instance._sound.Init();
        }
    }
    public static void Clear()
    {
        Scene.Clear();
        Pool.Clear();
        Time.timeScale = 1f;
    }
}
