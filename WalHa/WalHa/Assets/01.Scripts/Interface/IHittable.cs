using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    abstract void OnDamage(float damage, Transform origin = null);
    virtual void OnKnockBack(Transform origin){}
}
