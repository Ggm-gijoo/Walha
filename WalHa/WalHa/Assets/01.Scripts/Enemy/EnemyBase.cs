using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour,IHittable
{
    public EnemyBaseSO enemySO;

    [HideInInspector] public float hp;
    [HideInInspector] public float spd;
    [HideInInspector] public float atk;

    [SerializeField] private float attackDist;
    [SerializeField] private float chaseDist;

    public bool IsDead = false;
    public bool IsAct = false;
    public bool IsHit = false;

    protected readonly int _hit = Animator.StringToHash("isHitting");
    protected Material defaultMat;
    protected Material hitMat;

    protected Rigidbody2D rigid;
    protected SpriteRenderer sprite;
    protected Animator anim;

    protected Coroutine actCoroutine;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        hp = enemySO.Hp;
        spd = enemySO.Spd;
        atk = enemySO.Atk;

        defaultMat = sprite.material;
        hitMat = new Material(Managers.Resource.Load<Material>("HitMat"));
        
        StartCoroutine(SetAct());
    }

    public IEnumerator SetAct()
    {
        while (!IsDead)
        {
            yield return null;
            if (IsAct || IsHit) continue;

            float targetDist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            sprite.flipX = (PlayerController.Instance.transform.position.x - transform.position.x) < 0;
            actCoroutine = null;

            switch (targetDist)
            {
                case var dist when dist < attackDist:
                    actCoroutine = StartCoroutine(Attack());
                    break;
                case var dist when dist < chaseDist:
                    actCoroutine = StartCoroutine(Chase());
                    break;
                default:
                    rigid.velocity = Vector2.zero;
                    break;
            }
        }
    }

    public virtual void OnDamage(float damage, Transform origin = null)
    {
        if (IsDead) return;

        IsAct = false;
        hp -= damage;

        float hitTime = damage == 0 ? 1f : 0.3f;
        
        if(damage != 0)
        { 
            Managers.Pool.PoolManaging("HitEffect", transform.position, Quaternion.AngleAxis(Random.Range(0f,180f), Vector3.forward));
            Managers.Sound.Play("P_ToDamage");
        }
        
        OnKnockBack(origin);
        actCoroutine = StartCoroutine(OnHit(hitTime));

        if (hp <= 0)
            StartCoroutine(OnDie());
    }
    public virtual void OnKnockBack(Transform origin)
    {
        IsHit = true;
        sprite.material = hitMat;

        if (actCoroutine != null)
        {
            StopCoroutine(actCoroutine);
            actCoroutine = null;
        }

        rigid.velocity = Vector2.zero;
        Vector2 dir = (transform.position - origin.position).normalized;
        rigid.AddForce(dir * 2f, ForceMode2D.Impulse);
    }
    private IEnumerator OnHit(float hitTime = 0.3f)
    {
        yield return new WaitForSeconds(hitTime);

        IsHit = false;
        sprite.material = defaultMat;
        actCoroutine = null;
    }

    public abstract IEnumerator Chase();
    public abstract IEnumerator Attack();
    public IEnumerator OnDie()
    {
        IsDead = true;

        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
