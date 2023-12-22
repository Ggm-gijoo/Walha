using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private Transform[] spawnPoints;
    public static int nowStage = 0;

    private void Start()
    {
        Spawn(0, PlayerController.Instance.transform);
        nowStage = 0;
    }
    public void Spawn(int num, Transform target = null)
    {
        Managers.Sound.Play(bgms[num], Define.Sound.Bgm);
        DOTween.Kill(PlayerController.Instance.rigid);
        target.position = spawnPoints[num].position;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            nowStage++;
            Spawn(nowStage, other.transform);
        }
    }
}
