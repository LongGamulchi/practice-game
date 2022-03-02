using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EquipmentUI : MonoBehaviour
{
   
    public PlayerEquipItemController equipItemController;
    public Inventory inven;
    public Image leftMouseWeapon;
    public Image rightMouseWeapon;
    public Image throws;
    public Text throwsCount;

    private void Start()
    {
        inven.onChangeEquip += EquipUIUpdate;
        inven.onChangeItem += ThrowsUIUpdate;
        leftMouseWeapon.gameObject.SetActive(false);
        rightMouseWeapon.gameObject.SetActive(false);
    }


    void EquipUIUpdate()
    {
        if (inven.equipments[0].itemType != ItemType.Null) {
            leftMouseWeapon.sprite = inven.equipments[0].itemImage;
            leftMouseWeapon.gameObject.SetActive(true);
        }
        else
        {
            leftMouseWeapon.gameObject.SetActive(false);
        }


        if (inven.equipments[1].itemType != ItemType.Null) {
            rightMouseWeapon.sprite = inven.equipments[1].itemImage;
            rightMouseWeapon.gameObject.SetActive(true);
        }
        else
        {
            rightMouseWeapon.gameObject.SetActive(false);
        }
    }//인벤의 장비 상황에 따라 ui 최신화


    void ThrowsUIUpdate()
    {
        bool isFind = false;
        bool isUpdate = false;
        int count = 0;
        if (inven.items != null)
        {
            foreach (Item findThrows in inven.items)
            {
                if (findThrows.detailType == DetailType.Throws)
                {
                    throws.gameObject.SetActive(true);
                    throwsCount.gameObject.SetActive(true);
                    isFind = true;
                    break;
                }                
            }//아이템중에 투척물이 있는지 찾은후에
        }
        if (isFind)
        {
            for (int i = equipItemController.throwsChoice; i < equipItemController.throws.Length; i++)
            {
                foreach (Item findThrows in inven.items)
                {
                    if (equipItemController.throws[i] == findThrows.itemName)
                    {
                        isUpdate = true;
                        count++;
                        throws.sprite = findThrows.itemImage;
                    }//이름이 같은 투척물의 갯수와 이미지를 찾아서 업데이트한다.
                }
                if (isUpdate)
                {
                    equipItemController.throwsChoice = i;
                    throwsCount.text = count.ToString();
                    break;
                }//업데이트 했다면 빠져나간다.
                if (i == equipItemController.throws.Length - 1) i = -1;
                //업데이트 못했다면 순회하면서 찾는다.
            }
        }
        if (!isFind)
        {
            throws.gameObject.SetActive(false);
            throwsCount.gameObject.SetActive(false);
        }

    }
    /*
     투척물이 있다면
     현재 번호 투척물이 있다면
        ui 업데이트
     현재번호 투척물이 없다면
        번호 업데이트 후 반복
     */
}
