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
                dialogueTxt.text = "우리 부모님은 어디있지?";
                break;
            case 1:
                bossImg.SetActive(true);
                dialogueTxt.font = fonts[1];
                dialogueTxt.text = "이 나라 어딘가에 있지만.. 찾지 않는 것이 좋을거다...";
                break;
            case 2:
                playerImg.SetActive(true);
                dialogueTxt.font = fonts[0];
                dialogueTxt.text = "무슨 뜻이지?";
                break;
            case 3:
                bossImg.SetActive(true);
                dialogueTxt.font = fonts[1];
                dialogueTxt.text = "너의 부모는 그리 선량한 이가 아니다.. 찾아내면 재앙을 불러오고 말것이다..";
                break;
            case 4:
                playerImg.SetActive(true);
                dialogueTxt.font = fonts[0];
                dialogueTxt.text = "아니.. 그럴 리 없어! 난 반드시 부모님을 구해내고 말거야!";
                break;

            default:
                EndImg.SetActive(true);
                break;
        }
    }
}
