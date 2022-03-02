using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheildController : MonoBehaviour
{
    public enum ArmorLevel { None, One, Two, Three, Four }
    public delegate void OnSheildColor(float i, float j);
    public OnSheildColor onSheildColor;
    Inventory inven;
    public Item item;
    public ArmorLevel armorLevel;
    public float maxSheild;
    public float sheild;
    public float evolve;
    public float evolveDamage;


    void Start()
    {
        inven = GetComponent<Inventory>();
        inven.onChangeEquip += GetSheild;
    }

    void GetSheild()
    {//item의 소모품에만 쓰이는 변수들을 재활용 할 것이다.
        item = inven.equipments[2];
        maxSheild = item.itemStack;
        sheild = item.sortValue;
        evolve = item.evolve;
        if (maxSheild == 1)
        {
            maxSheild = 0;
            armorLevel = ArmorLevel.None;
            evolveDamage = 0;
        }
        else if (maxSheild == 25)
        {
            armorLevel = ArmorLevel.One;
            evolveDamage = 125;

        }
        else if (maxSheild == 50)
        {
            armorLevel = ArmorLevel.Two;
            evolveDamage = 200;
        }
        else if (maxSheild == 75)
        {
            armorLevel = ArmorLevel.Three;
            evolveDamage = 500;
        }
        else if (maxSheild == 100)
        {
            armorLevel = ArmorLevel.Four;
            evolveDamage = 10000;
        }
        onSheildColor(maxSheild, sheild);
    }//아머의 최대 실드량에 따라 현재 아머의 상태와 다음 진화까지 필요한 딜량을 설정한다.

    public float TakeDamaged(float damage)
    {
        float overDamage;
        Debug.Log(sheild);
        if (sheild > 0)
        {            
            if (sheild < damage)
            {
                overDamage = damage - sheild;
                sheild = 0;
                UpdateItemInfo();
                return overDamage;
            }
            sheild -= damage;
            UpdateItemInfo();
            return 0;
        }
        else
            return damage;
    }//데미지만큼 실드량을 깐다, 실드가 부셔졌다면 초과한 데미지만큼 리턴해준다.

    public void TakeDealing(float damage)
    {
        evolve += damage;
        UpdateItemInfo();
        if (evolve >= evolveDamage && armorLevel != ArmorLevel.Four)
        {
            float evolveTemp = evolve - evolveDamage;
            EvolveArmor(); 
            evolve = evolveTemp;
            UpdateItemInfo();
        }
    }//일정 딜량을 넘으면 갑옷의 최대 실드량이 늘어난다.

    public void UpdateItemInfo()
    {
        item = inven.equipments[2];
        item.itemStack = (int)maxSheild;
        item.sortValue = (int)sheild;
        item.evolve = (int)evolve;
    }//갑옷의 정보가 변함에 따라 인벤토리속 해당 아이템의 정보를 최신화한다.

    public void EvolveArmor()
    {
        evolve = 0;
        maxSheild += 25;
        UpdateItemInfo();
        inven.onChangeEquip();
    }//갑옷이 강해진다.

}