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
    //아이템들의 기본 변수 종류,이름,인벤토리저장시 이미지,변수(효과);


    public bool Use(GameObject usePlayer)
    {
        bool isUsed = false;
        foreach (ItemEffect eft in efts)
        {
            isUsed = eft.ExecuteRole(usePlayer);
        }
        return isUsed;
    }//아이템 사용에 성공했다면 true 실패라면 false;

    public bool Drop()
    {
        bool isDroped = false;
        isDroped = ItemDatabase.instance.Drop(this, Vector3.up*100);

        return isDroped;
    }//아이템 드랍에 성공했으면 true 실패라면 false;
}


