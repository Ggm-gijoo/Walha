using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    PlatformEffector2D platform;
    private bool isPlayerOn = false;

    private void Awake()
    {
        platform = GetComponent<PlatformEffector2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || !isPlayerOn) platform.rotationalOffset = 0f;
        if (Input.GetKey(KeyCode.S) && isPlayerOn && PlayerController.Instance.isGround) platform.rotationalOffset = 180f; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
            isPlayerOn = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
            isPlayerOn = false;
    }
}
