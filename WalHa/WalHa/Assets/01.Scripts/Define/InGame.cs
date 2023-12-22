using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame : Base
{
    public override void Init()
    {
        base.Init();
        Poolable clone = Managers.Pool.PoolManaging("HitEffect", transform);
        Managers.Pool.Push(clone);
    }
    public override void Clear()
    {
        Portal.nowStage = 0;
    }
}
