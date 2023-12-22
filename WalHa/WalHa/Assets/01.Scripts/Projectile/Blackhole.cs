using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Blackhole : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rigid;
    private Vector2 dir;

    private bool isCollided = false;
    
    private void Start()
    {
        dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Debug.Log(dir);
        rigid = GetComponent<Rigidbody2D>();

        StartCoroutine(OnDragProjectile());
    }

    private void Update()
    {
        OnMove();
    }

    private void OnMove()
    {
        rigid.velocity = dir * moveSpeed;
        transform.Rotate(Vector3.one * Time.deltaTime * 50f);
    }
    private IEnumerator OnDragProjectile()
    {
        while (true)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 7.5f, 1 << 9);
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].transform.DOMove(transform.position, 0.5f);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator ColCooldown()
    {
        isCollided = true;
        yield return new WaitForSeconds(0.02f);
        isCollided = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
            PlayerController.Instance.OnDamage(10);
        if(collision.gameObject.layer == 8 || collision.gameObject.layer == 13)
        {
            if (isCollided) return;

            StartCoroutine(ColCooldown());
            Vector2 closestPoint = collision.ClosestPoint(transform.position) - (Vector2)transform.position;
            bool isTriggeredH = Mathf.Abs(closestPoint.x) < Mathf.Abs(closestPoint.y);

            dir += isTriggeredH ? Vector2.down * dir.y * 2 : Vector2.left * dir.x * 2;
        }
    }
}
