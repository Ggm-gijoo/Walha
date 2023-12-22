using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Base : MonoSingleton<Base>
{
    [SerializeField]
    protected Define.Scene SceneType = Define.Scene.Unknown;

    public virtual void Init()
    {
        Managers.Sound.Clear();
    }

    public abstract void Clear();
}
