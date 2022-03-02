using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Rect baseRect;
    private Vector3 baseRectPosition;
    public PointerEventData.InputButton btn1 = PointerEventData.InputButton.Left;
    public PointerEventData.InputButton btn2 = PointerEventData.InputButton.Right;
    public int itemSlotnum;
    public bool isDrag;
    public bool leftRight;
    public bool isItem;
    public Item item;
    public Image itemIcon;
    public Text itemStackText;
    public int tempItemStack;
    public Transform parentObj;
    Inventory inven;



    public void Start()
    {
        inven = GetComponentInParent<InventoryUI>().inven;
        baseRect = transform.parent.parent.parent.GetComponent<RectTransform>().rect;
        baseRectPosition = transform.parent.parent.parent.GetComponent<RectTransform>().position;
        isDrag = false;
    }//�κ��丮 â�� ���� ��ǥ�� rect.xMax ���� ���Ѱ��� ui���� �κ��丮 ���� ��ǥ���� �ٱ��� ��ǥ���� �Ǻ��� �� �ִ� ������ �ȴ�.

    public void UpdateSlotUI()
    {
        isItem = true;
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);
        itemStackText.text = item.itemStack.ToString();
    }
    public void RemoveSlot()
    {
        isItem = false;
        item = null;
        itemIcon.gameObject.SetActive(false);
    }//�������� ���� �� �� �ش� ������ ��Ȱ��

    public void UpdateItemStack(int fieldItemStack)
    {
        item.itemStack += fieldItemStack;
        itemStackText.text = item.itemStack.ToString();
    }//������ �� ���� �ؽ�Ʈ�� �����Ѵ�.

    public void UseUpdate()
    {         

        bool isUse = item.Use(inven.transform.gameObject);
        if (isUse)
        {
            if (item.detailType != DetailType.Gold)
            {
                if (item.itemStack > 1)
                {
                    item.itemStack--;
                    itemStackText.text = item.itemStack.ToString();
                }
                else
                    inven.RemoveItem(itemSlotnum, item.itemType);
            }//�������� ��尡 �ƴҶ��� �׳� ����Ѵ�.
            else
            {
                if(item.itemStack < inven.tempPrice)
                {
                    inven.tempPrice -= item.itemStack;
                    inven.RemoveItem(itemSlotnum, item.itemType);
                    inven.FastUse("Gold");
                }
                else if (item.itemStack == inven.tempPrice)
                {
                    inven.RemoveItem(itemSlotnum, item.itemType);
                    inven.tempPrice = 0;
                }
                else if (item.itemStack > inven.tempPrice)
                {
                    item.itemStack -= inven.tempPrice;
                    itemStackText.text = item.itemStack.ToString();
                    inven.tempPrice = 0;
                }
            }//���� ����ϴ� ��Ȳ�� 3���� �з��� ������ ���ݿ� �˸°� ���� ����ϰ� �����.
        }
    }//����� ������ ��Ȳ�� �����ؾ��Ѵٸ� ����Ѵ�.

    public void DropUpdate()
    {
        tempItemStack = item.itemStack;
        if (item.detailType != DetailType.Gold)
        {
            item.itemStack = 1;
            bool isDrop = item.Drop();
            item.itemStack = tempItemStack;
            if (isDrop)
            {
                if (item.itemStack > 1)
                {
                    item.itemStack--;
                    itemStackText.text = item.itemStack.ToString();
                }
                else
                    inven.RemoveItem(itemSlotnum, item.itemType);
            }//��Ŭ�� ������δ� �������� 1���� ����Ѵ�.
        }
        else
        {//�������� ����ϰ��
            if (item.itemStack > 20)
            {
                item.itemStack = 20;
                bool isDrop = item.Drop();
                item.itemStack = tempItemStack;
                if (isDrop)
                {
                    item.itemStack -= 20;
                    inven.totalGold -= 20;
                    itemStackText.text = item.itemStack.ToString();
                }//20���� ������ ������ ������Ʈ���ش�.
            }
            else
            {
                bool isDrop = item.Drop();
                if (isDrop)
                {
                    inven.totalGold -= item.itemStack;
                    inven.RemoveItem(itemSlotnum, item.itemType);

                }
            }//20�����϶�� �׳� ������.
        }

    }

    public void AllDrop()
    {
        bool isDrop = item.Drop();
        if (isDrop)
        {
            if (item.detailType == DetailType.Gold)
                inven.totalGold -= item.itemStack;
            inven.RemoveItem(itemSlotnum, item.itemType);
        }
    }//�ѹ��� ��� ������ ������ ���


    public void OnPointerUp(PointerEventData eventData)
    {
        if (isItem && !isDrag)
        {// isDrag�� �̿��Ͽ� �巡�� �߿��� �������� ���ǰų� �������� �ʵ��� �ߴ�.
           /* if (eventData.button == btn1)
            {
                leftRight = false;
                inven.ChoiceFarItem(item, leftRight);
            } Ŭ������ ������ ����Ҷ� ä�θ� ��ų ������ ���� ���׿�;;*/

            if (eventData.button == btn2)
            {
                leftRight = true;
                inven.ChoiceFarItem(item, leftRight);
            }
            leftRight = false;
        }
    }/*�������� ����ϰų� ������ ������ �� �������� ���� ������ ���� ����ϰ� �ʹ�.
     �׷��� ��Ŭ���ϋ��� false ��Ŭ���϶� true�� ��ȯ�ϴ� bool�� �����.
     ���� �κ��丮�� ������ ����Ʈ�� Ž���� ���� �̸��� �������� ���� �������� ���� ��������ã�´�.
     ��Ŭ���� ��Ŭ���� �����Ͽ� ������ �����Ѵ�.
     */







    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isItem)
        {
            isDrag = true;
            inven.isDrag = isDrag;
            parentObj = transform;
            itemIcon.transform.SetParent(GameObject.FindGameObjectWithTag("InventoryUI").transform);
        }
    }//�巡���� �� �巡�� �� �������� �� ���� ���̵��� �ϱ� ���ؼ� �θ� ���� ������ ĵ������ �Ű��.
    public void OnDrag(PointerEventData eventData)
    {
        if (isItem)
        {
            itemIcon.transform.position = eventData.position;
        }
    }//�巡�� �߿� �ش� �������� ���콺�� ���󰣴�.
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isItem)
        {
            if (itemIcon.transform.position.x < baseRect.xMin + baseRectPosition.x
           || itemIcon.transform.position.x > baseRect.xMax + baseRectPosition.x
           || itemIcon.transform.position.y < baseRect.yMin + baseRectPosition.y
           || itemIcon.transform.position.y > baseRect.yMax + baseRectPosition.y)
            {
                AllDrop();
            }//�κ��丮 ������ �������� �巡�� �ߴٸ� �������� ���� ������.
            itemIcon.transform.SetParent(parentObj);
            itemIcon.transform.position = transform.position;
            isDrag = false;
            inven.isDrag = isDrag;
        }
    }//�������� �̹����� �ߴ� ��ġ�� �� ���·�, �������� �θ� ���� �������� �ٲ۴�.


}
