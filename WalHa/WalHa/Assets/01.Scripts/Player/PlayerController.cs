using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class PlayerController : MonoSingleton<PlayerController>, IHittable
{

    #region status
    [Header("Status")]
    [SerializeField] private float maxHp;
    public float MaxHp { get { return maxHp; } }
    [SerializeField] private float maxMp;
    public float MaxMp { get { return maxMp; } }

    [Space]
    [Header("Move Status")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float dashPower;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float fallMultiplier;
    
    [HideInInspector] public float hp;
    [HideInInspector] public float mp;
    #endregion
    #region Attacks
    [Space]
    [Header("Attacks")]
    [SerializeField] private int maxAttack;
    [SerializeField] private GameObject[] attackVfxs;
    [SerializeField] private GameObject healVfx;
    [SerializeField] private GameObject beamVfx;
    [SerializeField] private GameObject useSkillVfx;
    [SerializeField] private TrailRenderer dashTrail;
    #endregion
    #region soul
    [Space]
    [Header("Soul")]
    [SerializeField] private GameObject soul;
    private Material soulMat;
    private Animator soulAnim;

    private readonly int _soulDisappear = Animator.StringToHash("disappear");
    #endregion

    #region AnimHash
    private readonly int _walk = Animator.StringToHash("isWalking");
    private readonly int _jump = Animator.StringToHash("isJumping");
    private readonly int _attack = Animator.StringToHash("attack");
    private readonly int _parry = Animator.StringToHash("parry");
    private readonly int _soul = Animator.StringToHash("soul");
    private readonly int _attackMove = Animator.StringToHash("attackMove");
    #endregion
    #region Boolean
    [HideInInspector] public bool isGround = false;

    private bool isAct = false;
    private bool isDash = false;
    private bool isDead = false;
    private bool isDashCooldowned = false;
    
    private bool isSoul = false;
    private bool isParry = false;
    private bool isInvincible = false;
    #endregion
    #region Volume
    [Space]
    [Header("Volume")]
    [SerializeField] private VolumeProfile hitVolumeProfile;
    private Volume globalVolume;
    private VolumeProfile defaultVolumeProfile;
    #endregion

    [HideInInspector] public Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer sprite;

    private int attackMove = 0;
    private int nowSoul = 0;
    
    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        dashTrail.gameObject.SetActive(true);
        dashTrail.emitting = false;
    }
    private void Update()
    {
        PlayerInput();
    }
    private void FixedUpdate()
    {
        Move();
        GroundCheck();
    }

    public void PlayerInput()
    {
        if (isAct) return;

        if (Input.GetKeyDown(KeyCode.Space)) Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift)) StartCoroutine(Dash());

        if (Input.GetKeyDown(KeyCode.J)) StartCoroutine(Attack());

        if (Input.GetKeyDown(KeyCode.K)) StartCoroutine(Parry());

        if (Input.GetKeyDown(KeyCode.L)) StartCoroutine(Soul(nowSoul));

        if (Input.GetKeyDown(KeyCode.N)) StartCoroutine(Skill());

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            nowSoul = (nowSoul + 1) % SoulManager.Instance.soulSprites.Length;
            PlayerIcons.Instance.SetSoulIcon(nowSoul);
        }
    }

    #region Init
    private void Init()
    {
        isDead = false;

        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        soulMat = soul.GetComponent<SpriteRenderer>().material;
        soulAnim = soul.GetComponent<Animator>();

        globalVolume = FindObjectOfType<Volume>();
        defaultVolumeProfile = globalVolume.profile;

        SetStatus();
        StopCoroutine(UpdateMP());
        StartCoroutine(UpdateMP());
    }
    private void SetStatus()
    {
        hp = maxHp;
        mp = maxMp;
    }
    #endregion
    #region Moving
    private void Move()
    {
        if (isDash) return;

        float h = Input.GetAxisRaw("Horizontal");
        anim.SetBool(_walk, h != 0);

        if (rigid.velocity.y < 0)
        {
            if (isGround)
            {
                anim.SetBool(_walk, false);
                anim.SetBool(_jump, true);
                isGround = false;
            }
            else
            {
                rigid.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
        if(h == 0 || (isAct && isGround))
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            return;
        }

        sprite.flipX = h < 0;
        rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);
    }
    private void Jump()
    {
        if(isGround)
        {
            DOTween.Kill(rigid);
            anim.SetBool(_jump, true);
            
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            StartCoroutine(LongJump());
            isGround = false;
        }
    }
    private IEnumerator LongJump()
    {
        float maxTime = 0.3f;
        float endVelocity = rigid.velocity.y + jumpPower * 2.5f;
        while(Input.GetKey(KeyCode.Space) && maxTime >= 0)
        {
            if (Input.GetKey(KeyCode.LeftShift)) yield break;
            maxTime -= Time.deltaTime;
            rigid.velocity = Vector2.Lerp(rigid.velocity, Vector2.right * rigid.velocity.x + Vector2.up * endVelocity, Time.deltaTime);

            yield return null;
        }
    }
    private IEnumerator Dash()
    {
        if (isDashCooldowned) yield break;

        isDash = true;
        isInvincible = true;
        isDashCooldowned = true;

        float h = Input.GetAxisRaw("Horizontal");
        if (h == 0)
            h = sprite.flipX ? -1 : 1;

        float finalDashPower = h * dashPower;
        rigid.velocity = Vector2.zero;

        dashTrail.emitting = true;

        Managers.Sound.Play("P_Dash");
        DOTween.To(() => rigid.velocity, x => rigid.velocity = x, rigid.velocity + Vector2.right * finalDashPower, 0.1f).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(0.1f);
        isInvincible = false;

        yield return new WaitForSeconds(0.1f);
        isDash = false;
        
        yield return new WaitForSeconds(0.2f);
        dashTrail.emitting = false;

        yield return new WaitForSeconds(0.3f);
        isDashCooldowned = false;
    }
    private void GroundCheck()
    {
        if (isGround || rigid.velocity.y > 0) return;

        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, 0.5f, 1<<8);
        if(rayHit.transform != null)
        {
            anim.SetBool(_jump, false);
            isGround = true;
        }
    }
    #endregion
    #region Act
    private IEnumerator Attack()
    {
        DOTween.Kill(rigid);
        isAct = true;

        float xDir = sprite.flipX ? -1 : 1;

        Managers.Sound.Play("P_Slash");
        CinemachineCameraShaking.Instance.CameraShake();

        if (isGround)
        {
            anim.SetTrigger(_attack);
            anim.SetInteger(_attackMove, attackMove);

            Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position + Vector3.right * xDir, new Vector2(2f, 2f), 0, 1 << 7);
            foreach (Collider2D col in cols)
            {
                CinemachineCameraShaking.Instance.CameraShake(7, 0.1f);
                col.GetComponent<IHittable>().OnDamage(10, transform);
            }

            rigid.DOMoveX(transform.position.x + xDir, 0.1f);
            yield return new WaitForSeconds(0.1f);

            attackVfxs[attackMove].transform.localScale = new Vector3(xDir, 1, 1);

            attackVfxs[attackMove].SetActive(false);
            attackVfxs[attackMove].SetActive(true);

            attackMove = (attackMove + 1) % maxAttack;
        }
        else
        {
            anim.SetInteger(_attackMove, 2);
            anim.SetTrigger(_attack);

            yield return new WaitForSeconds(0.1f);
            attackVfxs[2].transform.localScale = new Vector3(xDir, 1, 1);

            Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position + Vector3.right * xDir, new Vector2(2f, 4.5f), 0, 1 << 7);
            foreach (Collider2D col in cols)
            {
                CinemachineCameraShaking.Instance.CameraShake(7, 0.1f);
                col.GetComponent<IHittable>().OnDamage(10, transform);
            }

            attackVfxs[2].SetActive(false);
            attackVfxs[2].SetActive(true);

            attackMove = 0;
        }
        yield return new WaitForSeconds(0.15f);
        isAct = false;
    }
    private IEnumerator Skill()
    {
        if (mp < 50) yield break;
        
        isAct = true;
        isInvincible = true;

        mp -= 50;

        Managers.Pool.PoolManaging("P_Skill", transform);

        anim.SetTrigger(_attack);
        anim.SetInteger(_attackMove, 0);

        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(1f);
        
        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, new Vector2(25, 15), 0, 1 << 7);
        foreach(Collider2D col in cols)
        {
            CinemachineCameraShaking.Instance.CameraShake(10f, 0.1f);
            col.GetComponent<IHittable>().OnDamage(100, transform);
        }

        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;
        isAct = false;
        isInvincible = false;

        yield return null;
    }
    private IEnumerator Parry()
    {
        if (!isGround || mp < 10) yield break;

        mp -= 10;

        useSkillVfx.SetActive(false);
        useSkillVfx.SetActive(true);

        anim.SetTrigger(_parry);
        isAct = true;
        isParry = true;
        Managers.Sound.Play("P_OnAct");

        yield return new WaitForSeconds(0.5f);

        isParry = false;
        isAct = false;
    }

    #region Soul
    private IEnumerator Soul(int num = 0)
    {
        if (mp < SoulManager.Instance.soulMps[num] || isSoul) yield break;

        yield return null;

        isSoul = true;
        anim.SetTrigger(_soul);

        useSkillVfx.SetActive(false);
        useSkillVfx.SetActive(true);

        float dir = sprite.flipX ? -1 : 1;
        soul.transform.localScale = new Vector2(dir, 1);

        soul.GetComponent<SpriteRenderer>().sprite = SoulManager.Instance.soulSprites[num];
        soul.SetActive(true);

        Managers.Sound.Play("P_OnAct");
        float setFloat = soulMat.GetFloat("_ColorRange");
        while(setFloat > 0)
        {
            setFloat -= Time.deltaTime * 7.5f;
            soulMat.SetFloat("_ColorRange", setFloat);

            yield return null;
        }

        soulMat.SetFloat("_ColorRange", 0);

        switch(num)
        {
            case 0:
                StartCoroutine(Soul_Striker());
                break;
            case 1:
                StartCoroutine(Soul_Healer());
                break;
        }
    }

    private IEnumerator Soul_Striker()
    {
        mp -= 30;

        float dir = sprite.flipX ? -1 : 1;

        beamVfx.transform.localScale = new Vector2(dir, 1);
        Managers.Sound.Play("P_Beam");

        beamVfx.SetActive(false);
        beamVfx.SetActive(true);

        CinemachineCameraShaking.Instance.CameraShake(13, 0.2f);

        Collider2D[] cols = Physics2D.OverlapBoxAll(beamVfx.transform.position + Vector3.right * 17f * dir, new Vector2(30f, 3f), 0, 1 << 7);
        foreach(Collider2D col in cols)
        {
            col.GetComponent<IHittable>().OnDamage(50, transform);
        }

        yield return new WaitForSeconds(1f);

        soulAnim.SetTrigger(_soulDisappear);
        float setFloat = 0;
        while (setFloat < 2)
        {
            setFloat += Time.deltaTime * 7.5f;
            soulMat.SetFloat("_ColorRange", setFloat);

            yield return null;
        }
        soulMat.SetFloat("_ColorRange", 2f);
        
        soul.SetActive(false);
        isSoul = false;
    }
    private IEnumerator Soul_Healer()
    {
        Managers.Sound.Play("P_Heal");
        healVfx.SetActive(true);

        while (Input.GetKey(KeyCode.L) && hp < maxHp && mp >= 2)
        {
            mp -= 2;
            hp += 0.5f;
            yield return new WaitForSeconds(0.01f);
        }
        if (hp > maxHp) hp = maxHp;

        Managers.Sound.Play("P_Heal");
        healVfx.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        soulAnim.SetTrigger(_soulDisappear);

        float setFloat = 0;
        while (setFloat < 2)
        {
            setFloat += Time.deltaTime * 7.5f;
            soulMat.SetFloat("_ColorRange", setFloat);

            yield return null;
        }

        soulMat.SetFloat("_ColorRange", 2f);
        
        soul.SetActive(false);
        isSoul = false;
    }
    #endregion
    #endregion
    #region Damage
    private IEnumerator UpdateMP()
    {
        while(!isDead)
        {
            yield return null;
            if (mp >= MaxMp || isAct) continue;
            mp += Time.deltaTime * 5;
        }
    }
    public virtual void OnDamage(float damage, Transform origin = null)
    {
        if (isInvincible) return;
        if (isParry)
        {
            isAct = false;
            mp += 20;

            if (mp > maxMp) mp = maxMp;

            Managers.Sound.Play("P_Parry");
            Managers.Pool.PoolManaging("ParryFx", transform);
            CinemachineCameraShaking.Instance.CameraShake(8);
            
            if(origin != null)
                origin.GetComponent<IHittable>().OnDamage(0, transform);

            return;
        }
        Managers.Sound.Play("P_Damaged");
        hp -= damage;
        StartCoroutine(OnHit());
        
        if(origin != null) OnKnockBack(origin);
        if (hp <= 0) OnDie();
    }
    public virtual void OnKnockBack(Transform origin)
    {
        Vector2 dir = (transform.position - origin.position).normalized;
        rigid.AddForce(dir * 5f, ForceMode2D.Impulse);
    }
    private IEnumerator OnHit()
    {
        globalVolume.profile = hitVolumeProfile;
        yield return new WaitForSeconds(0.2f);
        globalVolume.profile = defaultVolumeProfile;
    }
    public void OnDie()
    {
        isDead = true;
        GameManager.Instance.OnGameOver();
    }
    #endregion
}
