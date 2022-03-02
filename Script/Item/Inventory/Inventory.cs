using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{


    PlayerEquipItemController equipItemController;

    public delegate void OnChangeEquip();
    public OnChangeEquip onChangeEquip;
    public delegate void OnSlotCountChange(int val); //�븮�� ����
    public OnSlotCountChange onSlotCountChange; //�븮�� �ν��Ͻ�ȭ
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

   
    //�븮�ڸ� ����ϴ� ���� : �븮�ڸ� ������� �ʴ´ٸ� update�� Ȱ���Ͽ� Ư�� �̺�Ʈ�� �Ͼ���� ���Ӿ��� üũ�ؾ��Ѵ�.
    //������ �븮�ڴ� �̺�Ʈ�� �Ͼ �� Invoke�� ��ȣ�� �ش�. ȿ������ ���
    //��ġ ī�� �˹ٰ� �մԵ鿡�� �ֹ����� �ְ� ����ؼ� Ŀ�Ǹ� �����, �ֹ����� �︮�� �մ��� Ŀ�Ǹ� ��������. �մ��� Ŀ�� ���Դ��� ��� üũ�� �ʿ䰡 ���°�
    public List<Item> items = new List<Item>();
    public Item[] equipments = new Item[4];
    //�κ��丮�� ��� ������ ����Ʈ

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
    }//�ٲ� �κ��丮 ũ�⸦ ��ȯ�Ѵ�.(value�� �Ҵ���� ũ�⸦ �ǹ�)

    private void Start()
    {
        equipItemController = GetComponentInParent<PlayerEquipItemController>();
        SlotCnt = 6;
    }//�ʱ� �κ��丮 ũ��


    public bool AddItem(Item _item)
    {
        int itemIndexNumber = 0;
        int leftoverStack;
        if (_item.itemType != ItemType.Equipment)
        {//��� �������� �ƴ� ��쿡��
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
            }// �̸��� ���� �������� ������ �� ������ �����Ѵ�. ������ Enum�� ���� �������� �ʴ� �����۵� �߰� ����.
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
        }//��ĭ�� ������ �������� ���´ٸ� �������� �߰��Ѵ�.


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
            }//���߿� ���ʴ� ���� ������ ��� ����� ����� ��ü�Ұ��̴�.
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
            }//�̹� ���� �ִٸ� ��ü�Ѵ�.
        }//��񰡴��� �������� ��� �Ҵ�� ĭ�� �����ؾ� ������ ����Ʈ�� �ƴ� ������ ũ���� �迭�� ����Ͽ���.
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
    }//������ ����, �迭�� ��� ��ü�̹Ƿ� new�� �ʱ�ȭ �Ѵ�.


    public void PickUp(FieldItems fieldItems)
    {
        if (AddItem(fieldItems.GetItem()))
        {
            fieldItems.DestroyItem();
        }
    }//�������� �ݰ� ���ٴڿ� �ִ� �������� �ı��Ѵ�.

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
    }//ã�� �������� �ִٸ� ������� ��Ŭ�� ������Ʈ�� �޴� bool���� false�϶��� Use�� �̾�����.

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
    }//���� �ָ��ִ� �������� �� �� ��Ŭ���� ��Ŭ���� ���ο� ���� ������ �ٲ��.

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
        }//���� ������ �̿��Ͽ� ������ �� ������ �������� ���� �� �� �ִٸ� �����Ѵ�.
    }

    public void InventoryOver()
    {
        onAllDrop(listLength, isDead);
    }//�κ��丮�� �ʰ��ϰ� ���� ��Ұų�, �÷��̾ �׾����� ȣ���Ѵ�.
}
