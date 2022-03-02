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
    }//�κ��� ��� ��Ȳ�� ���� ui �ֽ�ȭ


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
            }//�������߿� ��ô���� �ִ��� ã���Ŀ�
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
                    }//�̸��� ���� ��ô���� ������ �̹����� ã�Ƽ� ������Ʈ�Ѵ�.
                }
                if (isUpdate)
                {
                    equipItemController.throwsChoice = i;
                    throwsCount.text = count.ToString();
                    break;
                }//������Ʈ �ߴٸ� ����������.
                if (i == equipItemController.throws.Length - 1) i = -1;
                //������Ʈ ���ߴٸ� ��ȸ�ϸ鼭 ã�´�.
            }
        }
        if (!isFind)
        {
            throws.gameObject.SetActive(false);
            throwsCount.gameObject.SetActive(false);
        }

    }
    /*
     ��ô���� �ִٸ�
     ���� ��ȣ ��ô���� �ִٸ�
        ui ������Ʈ
     �����ȣ ��ô���� ���ٸ�
        ��ȣ ������Ʈ �� �ݺ�
     */
}
