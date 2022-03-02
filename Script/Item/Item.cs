using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType {Null, Equipment, Concumables, Etc };
public enum DetailType {NoneWeapon, Weapon, Armor, Bag, Throws, Gold };


[System.Serializable]
public class Item
{
    public ItemType itemType;
    public DetailType detailType;
    public string itemName;
    public Sprite itemImage;
    public int itemStack = 1;
    public List<ItemEffect> efts;
    public int sortValue;
    public int evolve;
    public int price;
    //�����۵��� �⺻ ���� ����,�̸�,�κ��丮����� �̹���,����(ȿ��);


    public bool Use(GameObject usePlayer)
    {
        bool isUsed = false;
        foreach (ItemEffect eft in efts)
        {
            isUsed = eft.ExecuteRole(usePlayer);
        }
        return isUsed;
    }//������ ��뿡 �����ߴٸ� true ���ж�� false;

    public bool Drop()
    {
        bool isDroped = false;
        isDroped = ItemDatabase.instance.Drop(this, Vector3.up*100);

        return isDroped;
    }//������ ����� ���������� true ���ж�� false;
}


