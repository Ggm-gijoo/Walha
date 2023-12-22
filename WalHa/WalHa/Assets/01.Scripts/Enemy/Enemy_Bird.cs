using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bird : EnemyBase
{
    public override IEnumerator Chase()
    {
        if (IsHit || IsAct) yield break;
        float dir = sprite.flipX ? -1 : 1;
        rigid.velocity = (Vector2.right * dir * spd) + Vector2.up * rigid.velocity.y;
        
        yield return null;
        actCoroutine = null;
    }

    public override IEnumerator Attack()
    {
        if (IsHit || IsAct) yield break;
        IsAct = true;
        rigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        float dir = sprite.flipX ? -1 : 1;
        rigid.DOMoveX(transform.position.x + dir * 2f, 0.2f).SetEase(Ease.InQuint);

        yield return new WaitForSeconds(0.1f);

        Collider2D col = Physics2D.OverlapBox(transform.position + dir * Vector3.right, Vector2.one * 2, 0, 1 << 6);
        if (col != null)
        {
            CinemachineCameraShaking.Instance.CameraShake(6);
            col.GetComponent<IHittable>().OnDamage(atk, transform);
        }

        yield return new WaitForSeconds(2f);
        IsAct = false;
        actCoroutine = null;
    }
}
