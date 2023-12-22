using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private TextMeshProUGUI hpTxt;

    private float lerpHp = 0;

    private void OnEnable()
    {
        lerpHp = BossBase.NowBoss.Hp;
        UpdateUI();
    }
    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (BossBase.NowBoss == null) return;

        if (Mathf.Abs(lerpHp - BossBase.NowBoss.Hp) >= 0.1f)
            lerpHp = Mathf.Lerp(lerpHp, BossBase.NowBoss.Hp, Time.deltaTime * 5f);
        else
            lerpHp = BossBase.NowBoss.Hp;

        hpBar.value = lerpHp / BossBase.NowBoss.MaxHp;
        hpTxt.text = $"{(int)lerpHp}/{BossBase.NowBoss.MaxHp}";
    }
}
