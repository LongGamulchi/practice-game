using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject map;
    public bool activeMap;
    public Text evolveText;
    public Text Watch;
    public Image hpMeter;
    public Image spMeter;
    public Image sdMeter;
    public Image sdBarMeter;
    public Player player;
    SheildController sheild;
    private void Start()
    {
        sheild = player.sheild;
    }

    void Update()
    {
        Watch.text = ((int)(Time.time/60)).ToString() +" : "+ ((int)(Time.time%60)).ToString();
        evolveText.text = "Next Armor : " + (sheild.evolveDamage - sheild.evolve).ToString();
        hpMeter.fillAmount = player.health / player.maxHealth;
        sdBarMeter.fillAmount = sheild.maxSheild / 100;
        sdMeter.fillAmount = sheild.sheild / 100;
        spMeter.fillAmount = player.stamina / player.maxStamina;
        if (Input.GetKeyDown(KeyCode.M)){
            activeMap = !activeMap;
            map.SetActive(activeMap);
        }

    }
}
