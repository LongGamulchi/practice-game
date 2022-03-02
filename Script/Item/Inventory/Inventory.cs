using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{


    PlayerEquipItemController equipItemController;

    public delegate void OnChangeEquip();
    public OnChangeEquip onChangeEquip;
    public delegate void OnSlotCountChange(int val); //대리자 정의
    public OnSlotCountChange onSlotCountChange; //대리자 인스턴스화
    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;
    public delegate void OnUseDropDiscrimination(int useIndex, bool Click, ItemType type);
    public OnUseDropDiscrimination onUseDropDiscrimination;
    public delegate void OnAllDrop(int listLength, bool isDead);
    public OnAllDrop onAllDrop;
    public delegate int OnChangeItemStack(int number, int itemStack);
    public OnChangeItemStack onChangeItemStack;
    public delegate int OnNearSlotSurch(Vector3 position);
    public OnNearSlotSurch onNearSlotSurch;

   
    //대리자를 사용하는 이유 : 대리자를 사용하지 않는다면 update를 활용하여 특정 이벤트가 일어나는지 끊임없이 체크해야한다.
    //하지만 대리자는 이벤트가 일어날 때 Invoke로 신호를 준다. 효율적인 비용
    //마치 카페 알바가 손님들에게 주문벨을 주고 계속해서 커피를 만들고, 주문벨이 울리면 손님이 커피를 가져간다. 손님이 커피 나왔는지 계속 체크할 필요가 없는것
    public List<Item> items = new List<Item>();
    public Item[] equipments = new Item[4];
    //인벤토리에 담긴 아이템 리스트

    public bool isDead = false;
    public bool nowOpneInven = false;
    public bool isDrag;
    public int listLength = 0;
    public int lastUseWaepon = 0;
    public int totalGold = 0;
    public int tempPrice = 0;
    private int slotCnt;

    public int SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            onSlotCountChange.Invoke(slotCnt);
        }
    }//바뀐 인벤토리 크기를 반환한다.(value는 할당받은 크기를 의미)

    private void Start()
    {
        equipItemController = GetComponentInParent<PlayerEquipItemController>();
        SlotCnt = 6;
    }//초기 인벤토리 크기


    public bool AddItem(Item _item)
    {
        int itemIndexNumber = 0;
        int leftoverStack;
        if (_item.itemType != ItemType.Equipment)
        {//장비 아이템이 아닐 경우에만
            if (items != null && _item.detailType != DetailType.Throws)
            {
                foreach (Item tempItem in items)
                {
                    if (_item.itemName == tempItem.itemName)
                    {
                        leftoverStack = onChangeItemStack(itemIndexNumber, _item.itemStack);
                        if (leftoverStack == 0)
                        {
                            items.Sort(Utility.compare);
                            return true;
                        }
                        else
                        {
                            _item.itemStack = leftoverStack;
                        }
                    }
                    itemIndexNumber++;
                }
            }// 이름이 같은 아이템을 스택할 수 있으면 스택한다. 아이템 Enum에 따라 스택하지 않는 아이템도 추가 하자.
            if (items.Count < SlotCnt)
            {
                items.Add(_item);
                items.Sort(Utility.compare);
                if(_item.detailType ==DetailType.Gold)
                    totalGold +=_item.itemStack;
                if (onChangeItem != null)
                {
                    onChangeItem();
                }
                listLength++;
                return true;
            }
        }//빈칸이 있을떄 아이템이 들어온다면 아이템을 추가한다.


        if (_item.detailType == DetailType.Weapon)
        {
            if (equipments[0].itemType == ItemType.Null)
            {
                equipments[0] = _item;
                onChangeEquip();
                return true;
            }
            else if (equipments[1].itemType == ItemType.Null)
            {
                equipments[1] = _item;
                onChangeEquip();
                return true;
            }//나중에 양쪽다 무기 잇으면 방금 사용한 무기와 교체할것이다.
            else
            {
                int tempLastuse = lastUseWaepon;
                onUseDropDiscrimination(lastUseWaepon, true, equipments[lastUseWaepon].itemType);
                equipments[tempLastuse] = _item;
                lastUseWaepon = tempLastuse;
                onChangeEquip();
                return true;
            }
        }

        else if (_item.detailType == DetailType.Armor)
        {
            if (equipments[2].itemType == ItemType.Null)
            {
                equipments[2] = _item;
                onChangeEquip();
                return true;
            }
            else
            {
                onUseDropDiscrimination(2, true, equipments[2].itemType);
                equipments[2] = _item;
                onChangeEquip();
                return true;
            }
        }

        else if (_item.detailType == DetailType.Bag)
        {
            if (equipments[3].itemType == ItemType.Null)
            {
                SlotCnt += _item.itemStack;
                equipments[3] = _item;
                onChangeEquip();
                return true;
            }
            else
            {
                onUseDropDiscrimination(3, true, equipments[3].itemType);
                SlotCnt += _item.itemStack;
                equipments[3] = _item;
                onChangeEquip();
                return true;
            }//이미 템이 있다면 교체한다.
        }//장비가능한 아이템일 경우 할당된 칸이 존재해야 함으로 리스트가 아닌 고정된 크기의 배열을 사용하였다.
        return false;
    }

    
    public void RemoveItem(int _index, ItemType type)
    {
        
        if (type != ItemType.Equipment)
        {
            string tempItem = items[_index].itemName;
            DetailType detailType = items[_index].detailType;

            items.RemoveAt(_index);
            items.Sort(Utility.compare);
            listLength--;
            if (detailType == DetailType.Throws)
            {
                if (!CanBeUse(tempItem))
                {
                    if (equipItemController.nowHoldThrow)
                    {
                        equipItemController.nowHoldThrow = false;
                        equipItemController.CanThrowsUse();
                    }
                }
            }
            onChangeItem.Invoke();


        }
        else
        {
            equipments[_index] = new Item();
            onChangeEquip();
        }
    }//아이템 제거, 배열의 경우 객체이므로 new로 초기화 한다.


    public void PickUp(FieldItems fieldItems)
    {
        if (AddItem(fieldItems.GetItem()))
        {
            fieldItems.DestroyItem();
        }
    }//아이템을 줍고 땅바닥에 있는 아이템을 파괴한다.

    public bool CanBeUse(string findItem)
    {
        int useIndex = 0;
        int loopCount = 0;
        bool isThere = false;
        foreach (Item tempItem in items)
        {
            if (findItem == tempItem.itemName)
            {
                isThere = true;
                useIndex = loopCount;
            }
            loopCount++;
        }
        return isThere;
    }

    public void FastUse(string findItem)
    {
        int useIndex = 0;
        int loopCount = 0;
        bool isThere = false;
        foreach (Item tempItem in items)
        {
            if (findItem == tempItem.itemName)
            {
                isThere = true;
                useIndex = loopCount;
            }
            loopCount++;
        }
        if (isThere)
        {
            onUseDropDiscrimination(useIndex, !isThere, items[useIndex].itemType);
        }
    }//찾는 아이템이 있다면 빠른사용 온클릭 업데이트가 받는 bool값이 false일때만 Use로 이어진다.

    public void ChoiceFarItem(Item item, bool leftRight)
    {
        int itemIndex = 0;
        int loopCount = 0;
        bool isThere = false;
        foreach (Item tempItem in items)
        {
            if (item.itemName == tempItem.itemName)
            {
                isThere = true;
                itemIndex = loopCount;
            }
            loopCount++;
        }
        if (isThere)
            onUseDropDiscrimination(itemIndex, leftRight, item.itemType);
    }//가장 멀리있는 아이템을 고른 후 우클릭과 좌클릭의 여부에 따라 동작이 바뀐다.

    public void SwapItem(int swapItemIndex, Vector3 lastPosition)
    {
        Debug.Log("tringswap");
        Item tempItem;
        int swapPositionIndex = onNearSlotSurch(lastPosition);
        Debug.Log("slotnum:"+swapPositionIndex);
        if (swapPositionIndex > -1)
        {
            if (swapPositionIndex == 0 || swapPositionIndex == 1)
            {
                Debug.Log("swap");
                tempItem = equipments[swapItemIndex];
                equipments[swapItemIndex] = equipments[swapPositionIndex];
                equipments[swapPositionIndex] = tempItem;
                onChangeEquip();

            }
        }//받은 정보를 이용하여 스왑할 두 슬롯의 아이템을 스왑 할 수 있다면 스왑한다.
    }

    public void InventoryOver()
    {
        onAllDrop(listLength, isDead);
    }//인벤토리가 초과하게 템을 담았거나, 플레이어가 죽었을떄 호출한다.
}
