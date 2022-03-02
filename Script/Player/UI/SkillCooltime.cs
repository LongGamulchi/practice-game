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
    }//스킬 쿨타임 표시 1초 이하부터는 소숫점도 보이게해줬다.
    public void CoolTimeEnd()
    {
        coolTimeFillAmount.fillAmount = 0;
        coolTimeText.gameObject.SetActive(false);
    }//쿨이 다 돌았다면 텍스트를 끄고 혹시 남아있을 필어마운트를 0으로


}
