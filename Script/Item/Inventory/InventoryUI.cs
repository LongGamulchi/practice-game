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
        itemSlots = itemSlotHolder.GetComponentsInChildren<ItemSlot>();//자식 컴포넌트를 한번에 다 가져옴
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
    }//slotnum의 갯수에 따라 슬롯UI를 활성화한다



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
    }//tab키로 인벤토리를 키고 끄며 p키로 칸수를 늘린다.

    int StackChange(int itemIndex, int itemStack)
    {
        int leftoverStack = 0;
        int slotItemStack = itemSlots[itemIndex].item.itemStack;
        if (slotItemStack < 3 && itemSlots[itemIndex].item.detailType != DetailType.Gold)
        {//골드가 아닐때만 소모품을 3스택 까지만 쌓는다.
            if (slotItemStack + itemStack > 3)
            {
                leftoverStack = slotItemStack + itemStack - 3;
                itemSlots[itemIndex].UpdateItemStack(itemStack - leftoverStack);
            }
            else
                itemSlots[itemIndex].UpdateItemStack(itemStack);
        }
        else if (slotItemStack < 60 && itemSlots[itemIndex].item.detailType == DetailType.Gold)
        {//골드일때는 60까지 쌓는다.
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
    }/*스택 시스템을 구현했다. 아이템 인덱스와 아이템스택을 받아 스택할 수 있다면 스택하고, 스택하지 못한 만큼의 값을 다시 리턴하였다.*/

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
                    }//아이템이 골드일때 조건에 맞춰 버리기 위한 조건문이다.
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
    }//우클릭 좌클릭 등 drop인지 use인지 판별한 후 동작한다.


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
    }//아이템 변동이 있을시 아이템 칸UI를 업데이트 한다.

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
    }//아이템 변동이 있을시 ui 업데이트

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
    }//드래그 종료시 마우스 포지션과 가장 가까운 슬롯을 찾아 리턴한다.

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
        }//인벤창을 초과했다면 랜덤으로 초과 된 수만큼의 템을 떨군다.
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
    }//플레이어가 죽었다면 모든 템과 장비를 떨군다.

}
