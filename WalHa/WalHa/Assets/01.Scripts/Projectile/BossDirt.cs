using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDirt : MonoBehaviour
{
    private TrailRenderer trail;

    private void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnDisable()
    {
        trail.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            CinemachineCameraShaking.Instance.CameraShake(10);
            PlayerController.Instance.OnDamage(15);
            Managers.Pool.Push(GetComponent<Poolable>());
        }
    }
}
