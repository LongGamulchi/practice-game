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
    {//item�� �Ҹ�ǰ���� ���̴� �������� ��Ȱ�� �� ���̴�.
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
    }//�Ƹ��� �ִ� �ǵ差�� ���� ���� �Ƹ��� ���¿� ���� ��ȭ���� �ʿ��� ������ �����Ѵ�.

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
    }//��������ŭ �ǵ差�� ���, �ǵ尡 �μ����ٸ� �ʰ��� ��������ŭ �������ش�.

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
    }//���� ������ ������ ������ �ִ� �ǵ差�� �þ��.

    public void UpdateItemInfo()
    {
        item = inven.equipments[2];
        item.itemStack = (int)maxSheild;
        item.sortValue = (int)sheild;
        item.evolve = (int)evolve;
    }//������ ������ ���Կ� ���� �κ��丮�� �ش� �������� ������ �ֽ�ȭ�Ѵ�.

    public void EvolveArmor()
    {
        evolve = 0;
        maxSheild += 25;
        UpdateItemInfo();
        inven.onChangeEquip();
    }//������ ��������.

}