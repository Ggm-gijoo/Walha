using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcons : MonoSingleton<PlayerIcons>
{
    [SerializeField] private Image soulIcon;
    [SerializeField] private TextMeshProUGUI soulManaText;

    private void Start()
    {
        SetSoulIcon(0);
    }
    public void SetSoulIcon(int num)
    {
        soulIcon.sprite = SoulManager.Instance.soulSprites[num];
        //soulManaText.text = SoulManager.Instance.soulMps[num].ToString();
    }
}
