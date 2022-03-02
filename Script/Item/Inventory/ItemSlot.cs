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
    }//인벤토리 창의 원래 좌표에 rect.xMax 등을 더한값이 ui상의 인벤토리 안쪽 좌표인지 바깥쪽 좌표인지 판별할 수 있는 기준이 된다.

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
    }//아이템이 제거 될 떄 해당 슬롯을 비활성

    public void UpdateItemStack(int fieldItemStack)
    {
        item.itemStack += fieldItemStack;
        itemStackText.text = item.itemStack.ToString();
    }//스택할 때 마다 텍스트를 수정한다.

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
            }//아이템이 골드가 아닐때는 그냥 사용한다.
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
            }//돈을 사용하는 상황에 3가지 분류로 나눠서 가격에 알맞게 돈을 사용하게 해줬다.
        }
    }//사용한 아이템 상황을 갱신해야한다면 사용한다.

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
            }//우클릭 드랍으로는 아이템을 1개씩 드랍한다.
        }
        else
        {//아이템이 골드일경우
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
                }//20원씩 떨구고 스택을 업데이트해준다.
            }
            else
            {
                bool isDrop = item.Drop();
                if (isDrop)
                {
                    inven.totalGold -= item.itemStack;
                    inven.RemoveItem(itemSlotnum, item.itemType);

                }
            }//20원이하라면 그냥 떨군다.
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
    }//한번에 모든 스택을 버리는 경우


    public void OnPointerUp(PointerEventData eventData)
    {
        if (isItem && !isDrag)
        {// isDrag를 이용하여 드래그 중에는 아이템이 사용되거나 버려지지 않도록 했다.
           /* if (eventData.button == btn1)
            {
                leftRight = false;
                inven.ChoiceFarItem(item, leftRight);
            } 클릭으로 아이템 사용할때 채널링 시킬 뾰족한 수가 없네여;;*/

            if (eventData.button == btn2)
            {
                leftRight = true;
                inven.ChoiceFarItem(item, leftRight);
            }
            leftRight = false;
        }
    }/*아이템을 사용하거나 떨굴때 무조건 맨 마지막에 들어온 아이템 부터 사용하고 싶다.
     그래서 좌클릭일떄는 false 우클릭일땐 true를 반환하는 bool을 만든다.
     그후 인벤토리의 아이템 리스트를 탐색해 같은 이름의 아이템중 가장 마지막에 들어온 아이템을찾는다.
     좌클릭과 우클릭을 구분하여 동작을 구현한다.
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
    }//드래그할 때 드래그 된 아이템을 맨 위에 보이도록 하기 위해서 부모를 가장 상위인 캔버스로 옮겼다.
    public void OnDrag(PointerEventData eventData)
    {
        if (isItem)
        {
            itemIcon.transform.position = eventData.position;
        }
    }//드래그 중에 해당 아이템이 마우스를 따라간다.
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
            }//인벤토리 밖으로 아이템을 드래그 했다면 아이템을 전부 버린다.
            itemIcon.transform.SetParent(parentObj);
            itemIcon.transform.position = transform.position;
            isDrag = false;
            inven.isDrag = isDrag;
        }
    }//아이템의 이미지가 뜨는 위치를 원 상태로, 아이템의 부모를 원래 슬롯으로 바꾼다.


}
