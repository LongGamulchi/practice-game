using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inven;
    public GameObject inventoryPanel;
    bool activeInventory = false;
    public static EquipmentSlot swapItem;
    public ItemSlot[] itemSlots;
    public EquipmentSlot[] equipmentSlots;
    public Transform equipmentSlotHolder;
    public Transform itemSlotHolder;

    private void Start()
    {
        itemSlots = itemSlotHolder.GetComponentsInChildren<ItemSlot>();//�ڽ� ������Ʈ�� �ѹ��� �� ������
        equipmentSlots = equipmentSlotHolder.GetComponentsInChildren<EquipmentSlot>();
        inven.onSlotCountChange += SlotChange;
        inven.onChangeItem += RedrawItemSlotUI;
        inven.onChangeItemStack += StackChange;
        inven.onUseDropDiscrimination += UseOrDropUpdate;
        inven.onChangeEquip += RedrawEquipSlotUI;
        inven.onNearSlotSurch += NearSlot;
        inven.onAllDrop += AllDrop;
        inventoryPanel.SetActive(activeInventory);
        RedrawItemSlotUI();
        RedrawEquipSlotUI();
        for (int i = 0; i < 4; i++)
        {
            equipmentSlots[i].equipSlotnum = i;
        }
    }

    private void SlotChange(int val)
    {        
        for(int i = 0; i< itemSlots.Length; i++)
        {
            itemSlots[i].itemSlotnum = i;
            if (i < inven.SlotCnt)
                itemSlots[i].GetComponent<Button>().interactable = true;
            else
                itemSlots[i].GetComponent<Button>().interactable = false;
        }
    }//slotnum�� ������ ���� ����UI�� Ȱ��ȭ�Ѵ�



    private void Update()
    {
        if (!inven.isDrag)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                activeInventory = !activeInventory;
                inventoryPanel.SetActive(activeInventory);

            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                inven.SlotCnt++;
            }
        }
    }//tabŰ�� �κ��丮�� Ű�� ���� pŰ�� ĭ���� �ø���.

    int StackChange(int itemIndex, int itemStack)
    {
        int leftoverStack = 0;
        int slotItemStack = itemSlots[itemIndex].item.itemStack;
        if (slotItemStack < 3 && itemSlots[itemIndex].item.detailType != DetailType.Gold)
        {//��尡 �ƴҶ��� �Ҹ�ǰ�� 3���� ������ �״´�.
            if (slotItemStack + itemStack > 3)
            {
                leftoverStack = slotItemStack + itemStack - 3;
                itemSlots[itemIndex].UpdateItemStack(itemStack - leftoverStack);
            }
            else
                itemSlots[itemIndex].UpdateItemStack(itemStack);
        }
        else if (slotItemStack < 60 && itemSlots[itemIndex].item.detailType == DetailType.Gold)
        {//����϶��� 60���� �״´�.
            if (slotItemStack + itemStack > 60)
            {
                leftoverStack = slotItemStack + itemStack - 60;
                itemSlots[itemIndex].UpdateItemStack(itemStack - leftoverStack);
                inven.totalGold += itemStack - leftoverStack;
            }
            else
            {
                itemSlots[itemIndex].UpdateItemStack(itemStack);
                inven.totalGold += itemStack;
            }
        }
        else
        {
            leftoverStack = itemStack;
        }
        return leftoverStack;
    }/*���� �ý����� �����ߴ�. ������ �ε����� �����۽����� �޾� ������ �� �ִٸ� �����ϰ�, �������� ���� ��ŭ�� ���� �ٽ� �����Ͽ���.*/

    void UseOrDropUpdate(int useIndex, bool Discrimination, ItemType type)
    {
        if (type != ItemType.Equipment)
        {
            if (!Discrimination)
                itemSlots[useIndex].UseUpdate();
            else if (Discrimination)
            {
                if (itemSlots[useIndex].item.detailType == DetailType.Gold)
                {
                    if (itemSlots[useIndex].item.itemStack < 20 && inven.totalGold > 60)
                    {
                        itemSlots[useIndex - 1].item.itemStack -= (20 - itemSlots[useIndex].item.itemStack);
                        itemSlots[useIndex].item.itemStack = 20;
                    }//�������� ����϶� ���ǿ� ���� ������ ���� ���ǹ��̴�.
                }
                itemSlots[useIndex].DropUpdate();
            }
        }
        else
        {
            //if (!Discrimination) equipmentSlots[useIndex].UseUpdate();
            if (Discrimination)
                equipmentSlots[useIndex].DropUpdate();
        }
    }//��Ŭ�� ��Ŭ�� �� drop���� use���� �Ǻ��� �� �����Ѵ�.


    public void RedrawItemSlotUI()
    {

        for(int i=0; i< itemSlots.Length; i++)
        {
            itemSlots[i].RemoveSlot();
        }
        for(int i = 0; i<inven.items.Count; i++)
        {
            itemSlots[i].item = inven.items[i];
            itemSlots[i].UpdateSlotUI();
        }
    }//������ ������ ������ ������ ĭUI�� ������Ʈ �Ѵ�.

    public void RedrawEquipSlotUI()
    {
        for (int i = 0; i < 4; i++)
        {
            equipmentSlots[i].RemoveSlot();
        }
        for (int i = 0; i < 4; i++)
        {
            equipmentSlots[i].item = inven.equipments[i];
            equipmentSlots[i].UpdateSlotUI();
        }
    }//������ ������ ������ ui ������Ʈ

    public int NearSlot(Vector3 position)
    {
        float min = 10000f;
        int slotIndex = -1;
        for (int i = 0; i < 4; i++)
        {
            Vector2 slotPosition = equipmentSlots[i].transform.position;
            float dis = Vector2.Distance(slotPosition, position);
            if (dis < min)
            {
                min = dis;
                slotIndex = i;
            }
        }
        if (min > 145f)
            return -1;
        else
            return slotIndex;
    }//�巡�� ����� ���콺 �����ǰ� ���� ����� ������ ã�� �����Ѵ�.

    public void AllDrop(int length, bool isDead)
    {
        if (!isDead)
        {
            int loopCount = inven.listLength - inven.SlotCnt;
            for (int i = 0; i< loopCount; i++)
            {
                int index = Random.Range(0, length);
                itemSlots[index].AllDrop();
                length--;

            }
            inven.listLength = inven.SlotCnt;
        }//�κ�â�� �ʰ��ߴٸ� �������� �ʰ� �� ����ŭ�� ���� ������.
        else
        {
            for(int i = 0; i<length; i++)
            {
                itemSlots[0].AllDrop();
            }
            for(int i = 0; i<4; i++)
            {
                equipmentSlots[i].DropUpdate();
            }
        }
    }//�÷��̾ �׾��ٸ� ��� �۰� ��� ������.

}
