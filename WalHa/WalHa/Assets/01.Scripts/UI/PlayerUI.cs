using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerController player;
    
    [SerializeField] private Slider hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Slider mpBar;
    [SerializeField] private TextMeshProUGUI mpText;

    private float lerpHp = 0;

    private void Start()
    {
        player = PlayerController.Instance;

        lerpHp = player.MaxHp;
    }
    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (Mathf.Abs(lerpHp - player.hp) >= 0.1f)
            lerpHp = Mathf.Lerp(lerpHp, player.hp, Time.deltaTime * 5f);
        else
            lerpHp = player.hp;

        hpText.text = $"{(int)lerpHp}<size=80%><color=#CCCCCC>/{player.MaxHp}";
        hpBar.value = lerpHp / player.MaxHp;


        mpBar.value = player.mp / player.MaxMp;
        mpText.text = ((int)player.mp).ToString();
    }
}
