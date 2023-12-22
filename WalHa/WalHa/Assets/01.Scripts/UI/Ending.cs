using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ending : MonoBehaviour
{
    [SerializeField] private GameObject playerImg;
    [SerializeField] private GameObject bossImg;

    [SerializeField] private TextMeshProUGUI dialogueTxt;
    [SerializeField] private TMP_FontAsset[] fonts;

    [SerializeField] private GameObject EndImg;

    private int idx = 0;

    public void Clear()
    {
        playerImg.SetActive(false);
        bossImg.SetActive(false);
    }

    private void Start()
    {
        OnClickDialogue();
    }

    public void OnClickDialogue()
    {
        SetEndDialogue(idx);
        idx++;
    }

    public void SetEndDialogue(int num)
    {
        Clear();
        switch (num)
        {
            case 0:
                playerImg.SetActive(true);
                dialogueTxt.font = fonts[0];
                dialogueTxt.text = "�츮 �θ���� �������?";
                break;
            case 1:
                bossImg.SetActive(true);
                dialogueTxt.font = fonts[1];
                dialogueTxt.text = "�� ���� ��򰡿� ������.. ã�� �ʴ� ���� �����Ŵ�...";
                break;
            case 2:
                playerImg.SetActive(true);
                dialogueTxt.font = fonts[0];
                dialogueTxt.text = "���� ������?";
                break;
            case 3:
                bossImg.SetActive(true);
                dialogueTxt.font = fonts[1];
                dialogueTxt.text = "���� �θ�� �׸� ������ �̰� �ƴϴ�.. ã�Ƴ��� ����� �ҷ����� �����̴�..";
                break;
            case 4:
                playerImg.SetActive(true);
                dialogueTxt.font = fonts[0];
                dialogueTxt.text = "�ƴ�.. �׷� �� ����! �� �ݵ�� �θ���� ���س��� ���ž�!";
                break;

            default:
                EndImg.SetActive(true);
                break;
        }
    }
}
