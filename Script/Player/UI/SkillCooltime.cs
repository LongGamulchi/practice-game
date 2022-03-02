using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooltime : MonoBehaviour
{
    public Text coolTimeText;
    public Image coolTimeFillAmount;

    public void CoolTimeUpdate(float numerrator, float denominator)
    {
        coolTimeText.gameObject.SetActive(true);
        if (numerrator > 0.9)
            coolTimeText.text = Mathf.CeilToInt(numerrator).ToString();
        else
            coolTimeText.text = string.Format("{0:0.#}", numerrator);
        coolTimeFillAmount.fillAmount = numerrator / denominator;
    }//��ų ��Ÿ�� ǥ�� 1�� ���Ϻ��ʹ� �Ҽ����� ���̰������.
    public void CoolTimeEnd()
    {
        coolTimeFillAmount.fillAmount = 0;
        coolTimeText.gameObject.SetActive(false);
    }//���� �� ���Ҵٸ� �ؽ�Ʈ�� ���� Ȥ�� �������� �ʾ��Ʈ�� 0����


}
