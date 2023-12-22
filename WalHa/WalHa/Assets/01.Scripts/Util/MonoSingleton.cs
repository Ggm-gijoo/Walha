using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static bool shuttingDown = false;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (shuttingDown)
            {
                Debug.Log("[Singleton] Instance" + typeof(T) + " already  CHOled. Returning null");
                return null;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    _instance = obj.AddComponent<T>();
                }

                // DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        shuttingDown = false;
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}
