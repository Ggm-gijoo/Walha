using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class BossBase : MonoBehaviour, IHittable
{
    public static BossBase NowBoss;

    public float MaxHp;
    [HideInInspector] public float Hp;

    protected PlayerController target;
    protected Transform targetTransform;

    [Space]
    [SerializeField] protected GameObject bossHPBar;
    [SerializeField] protected CinemachineVirtualCamera bossVCam;

    protected Material defaultMat;
    protected Material hitMat;

    protected bool isAct = false;
    protected bool isAttack = false;
    protected bool isDead = false;
    protected bool[] isCoolDowned = new bool[5];

    protected float moveSpeed = 7.5f;

    #region animHash
    protected int _attack = Animator.StringToHash("attack");
    protected int _attackMove = Animator.StringToHash("attackMove");
    protected int _move = Animator.StringToHash("isMoving");
    #endregion

    protected Animator anim;
    protected SpriteRenderer sprite;
    protected Rigidbody2D rigid;

    private void Awake()
    {
        NowBoss = this;
    }

    void OnEnable()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        Init();
    }
    private void Update()
    {
        if (!isAttack && !isDead)
            SetMove();
    }
    public void Init()
    {
        Hp = MaxHp;
        bossVCam.Priority = 0;

        target = PlayerController.Instance;
        targetTransform = target.transform;

        defaultMat = sprite.material;
        hitMat = new Material(Managers.Resource.Load<Material>("HitMat"));

        StartCoroutine(SetAct());
    }

    private IEnumerator SetAct()
    {
        isAttack = true;

        yield return new WaitUntil(() => Portal.nowStage == 1);
        bossHPBar.SetActive(true);
        bossVCam.Priority = 11;

        yield return new WaitForSeconds(2f);

        while (!isDead)
        {
            yield return null;
            if (isAct) continue;

            Flip();
            int randomPattern = Random.Range(0, 5);

            if (isCoolDowned[randomPattern]) continue;
            StartCoroutine(CoolDown(randomPattern));

            anim.SetInteger(_attackMove, randomPattern);
            isAct = true;
            isAttack = true;

            rigid.velocity = Vector2.zero;
            anim.SetBool(_move, false);

            switch (randomPattern)
            {
                case 0:
                    StartCoroutine(Attack1());
                    break;
                case 1:
                    StartCoroutine(Attack2());
                    break;
                case 2:
                    StartCoroutine(Attack3());
                    break;
                case 3:
                    StartCoroutine(Attack4());
                    break;
                case 4:
                    StartCoroutine(Attack5());
                    break;
            }
        }
    }
    protected void SetMove()
    {
        if (Vector2.Distance(targetTransform.position, transform.position) <= 3) return;

        Flip();
        float dir = targetTransform.position.x > transform.position.x ? 1 : -1;
        anim.SetBool(_move, true);
        rigid.velocity = new Vector2(dir * moveSpeed, rigid.velocity.y);
    }
    protected IEnumerator CoolDown(int num)
    {
        isCoolDowned[num] = true;
        yield return new WaitForSeconds(6f);
        isCoolDowned[num] = false;
    }

    protected virtual IEnumerator Attack1() { yield return null; }
    protected virtual IEnumerator Attack2() { yield return null; }
    protected virtual IEnumerator Attack3() { yield return null; }
    protected virtual IEnumerator Attack4() { yield return null; }
    protected virtual IEnumerator Attack5() { yield return null; }

    void Flip()
    {
        sprite.flipX = targetTransform.position.x > transform.position.x;
    }
    public void OnDamage(float damage, Transform origin = null)
    {
        if (isDead) return;

        Hp -= damage;
        if (damage != 0)
        {
            Managers.Pool.PoolManaging("HitEffect", transform);
            Managers.Sound.Play("P_ToDamage");
        }
        else
            StartCoroutine(OnParry());
        
        StartCoroutine(OnHit());
        if (Hp <= 0) StartCoroutine(OnDie());
    }
    protected virtual IEnumerator OnParry()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1f;
    }
    private IEnumerator OnHit()
    {
        sprite.material = hitMat;

        yield return new WaitForSeconds(0.3f);

        sprite.material = defaultMat;
    }
    private IEnumerator OnDie()
    {
        isDead = true;

        rigid.velocity = Vector2.zero;
        anim.SetBool(_move, false);

        bossHPBar.SetActive(false);
        bossVCam.Priority = 0;

        yield return new WaitForSeconds(1f);

        GameManager.Instance.OnEnding();
    }
}
