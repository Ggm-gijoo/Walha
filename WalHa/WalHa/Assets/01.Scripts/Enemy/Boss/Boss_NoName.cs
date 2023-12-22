using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss_NoName : BossBase
{
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private GameObject[] vfxs;
    [SerializeField] private GameObject dashWarning;

    protected override IEnumerator Attack1()//근접 후 내려찍기
    {
        moveSpeed = 15f;
        while (Mathf.Abs(targetTransform.position.x - transform.position.x) > 3.5f)
        {
            SetMove();
            yield return null;
        }
        moveSpeed = 7.5f;

        rigid.velocity = Vector2.zero;
        anim.SetBool(_move, false);
        anim.SetTrigger(_attack);

        float dir = sprite.flipX ? 1 : -1;
        vfxs[0].transform.localRotation = Quaternion.AngleAxis((dir - 1) * 90, Vector2.up);

        yield return new WaitForSeconds(0.3f);
        Managers.Sound.Play(sounds[0]);
        vfxs[0].SetActive(false);
        vfxs[0].SetActive(true);

        Collider2D col = Physics2D.OverlapBox(transform.position + (dir * 3.5f * Vector3.right), new Vector2(6.5f, 4f), 0, 1 << 6);

        if (col != null)
            PlayerController.Instance.OnDamage(25, transform);

        CinemachineCameraShaking.Instance.CameraShake(15, 0.2f);

        yield return new WaitForSeconds(1f);
        isAttack = false;

        yield return new WaitForSeconds(1f);
        isAct = false;
    }
    protected override IEnumerator Attack2()//러시 
    {
        Managers.Sound.Play(sounds[4]);
        float dir = sprite.flipX ? 1 : -1;

        dashWarning.transform.localPosition = new Vector2(dir * 12.5f, -3f);
        dashWarning.SetActive(true);

        yield return new WaitForSeconds(1f);
        dashWarning.SetActive(false);

        CinemachineCameraShaking.Instance.CameraShake(7, 0.3f);

        Managers.Sound.Play(sounds[1]);
        anim.SetTrigger(_attack);

        vfxs[1].transform.rotation = Quaternion.AngleAxis((dir - 1) * 90, Vector2.up);

        vfxs[1].SetActive(false);
        vfxs[1].SetActive(true);

        Vector2 originPos = transform.position;
        rigid.DOMoveX(transform.position.x + dir * 20f, 0.5f).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(0.15f);

        Collider2D col = Physics2D.OverlapBox(originPos + Vector2.right * dir * 10f, new Vector2(20f, 5f), 0, 1 << 6);
        if (col != null)
            PlayerController.Instance.OnDamage(20, transform);

        yield return new WaitForSeconds(1f);
        isAttack = false;

        yield return new WaitForSeconds(1f);
        isAct = false;
    }
    protected override IEnumerator Attack3()//광범위 공격
    {
        anim.SetTrigger(_attack);

        float dir = sprite.flipX ? 1 : -1;
        vfxs[2].transform.localRotation = Quaternion.AngleAxis((dir - 1) * 90, Vector2.up);
        Managers.Sound.Play(sounds[4]);

        vfxs[2].SetActive(false);
        vfxs[2].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        CinemachineCameraShaking.Instance.CameraShake(10);
        Managers.Sound.Play(sounds[2]);

        yield return new WaitForSeconds(0.2f);

        Collider2D col = Physics2D.OverlapCircle(transform.position, 40, 1 << 6);
        if (col != null)
            PlayerController.Instance.OnDamage(10, transform);

        yield return new WaitForSeconds(1f);
        isAttack = false;

        yield return new WaitForSeconds(1f);
        isAct = false;
    }
    protected override IEnumerator Attack4()//올려치기 후 내려찍기
    {
        yield return new WaitForSeconds(0.25f);

        float dir = sprite.flipX ? 1 : -1;

        anim.SetTrigger(_attack);
        transform.DOMoveX(transform.position.x + dir * 4f, 0.2f).SetEase(Ease.InQuint);

        yield return new WaitForSeconds(0.2f);

        CinemachineCameraShaking.Instance.CameraShake(10);
        Managers.Sound.Play(sounds[3]);
        vfxs[3].transform.localRotation = Quaternion.AngleAxis((dir - 1) * 90, Vector2.up);

        vfxs[3].SetActive(false);
        vfxs[3].SetActive(true);

        Collider2D col = Physics2D.OverlapBox(transform.position + (dir * 3.5f * Vector3.right), new Vector2(3f, 6.5f), 0, 1 << 6);

        if (col != null)
            PlayerController.Instance.OnDamage(15, transform);

        yield return new WaitForSeconds(0.6f);

        anim.SetInteger(_attackMove, 0);
        anim.SetTrigger(_attack);
        transform.DOMoveX(transform.position.x + dir * 4f, 0.2f).SetEase(Ease.InQuint);

        yield return new WaitForSeconds(0.2f);

        CinemachineCameraShaking.Instance.CameraShake(10);
        Managers.Sound.Play(sounds[0]);
        vfxs[0].transform.localRotation = Quaternion.AngleAxis((dir - 1) * 90, Vector2.up);

        vfxs[0].SetActive(false);
        vfxs[0].SetActive(true);

        col = Physics2D.OverlapBox(transform.position + (dir * 3.5f * Vector3.right), new Vector2(6.5f, 4f), 0, 1 << 6);

        if (col != null)
            PlayerController.Instance.OnDamage(20, transform);

        yield return new WaitForSeconds(1f);
        isAttack = false;

        yield return new WaitForSeconds(1f);
        isAct = false;
    }
    protected override IEnumerator Attack5()//올려치기(투사체 발사)
    {
        anim.SetInteger(_attackMove, 3);
        anim.SetTrigger(_attack);

        yield return new WaitForSeconds(0.2f);
        Managers.Sound.Play(sounds[3]);
        CinemachineCameraShaking.Instance.CameraShake(10);

        float dir = sprite.flipX ? 1 : -1;
        for (int i = 1; i < 4; i++)
        {
            GameObject clone = Managers.Pool.PoolManaging("BossDirt", transform.position, transform.rotation).gameObject;
            clone.GetComponent<Rigidbody2D>().AddForce(new Vector2(i * 4 * dir, 15 - (i * 3)), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(1f);
        isAct = false;
    }

}
