using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Rect baseRect;
    private Rect slotRect;
    private Vector3 baseRectPosition;
    public PointerEventData.InputButton btn1 = PointerEventData.InputButton.Left;
    public PointerEventData.InputButton btn2 = PointerEventData.InputButton.Right;
    public int equipSlotnum;
    public bool isItem;
    public bool isDrag;
    public Item item;
    public Image itemIcon;
    public Transform parentObj;
    Inventory inven;



    public void Start()
    {
        inven = GetComponentInParent<EquipmentUI>().inven;
        slotRect = transform.GetComponent<RectTransform>().rect;
        baseRect = transform.parent.parent.parent.GetComponent<RectTransform>().rect;
        baseRectPosition = transform.parent.parent.parent.GetComponent<RectTransform>().position;
    }//인벤토리 창의 원래 좌표에 rect.xMax 등을 더한값이 ui상의 인벤토리 안쪽 좌표인지 바깥쪽 좌표인지 판별할 수 있는 기준이 된다.

    public void UpdateSlotUI()
    {
        if (item.itemType != ItemType.Null)
        {
            isItem = true;
            itemIcon.sprite = item.itemImage;
            itemIcon.gameObject.SetActive(true);
        }
    }//인벤토리 장비창 업데이트

    public void RemoveSlot()
    {

        isItem = false;
        item = null;
        itemIcon.gameObject.SetActive(false);
    }//아이템이 제거 될 떄 해당 슬롯을 비활성

    public void DropUpdate()
    {
        if (isItem)
        {
            bool isDrop = item.Drop();
            if (isDrop)
            {
                if (item.detailType == DetailType.Bag)
                {
                    inven.SlotCnt -= item.itemStack;
                }//슬롯증가는 인벤토리에서 장비 입수 할 때 해줬따.
                if (item.detailType == DetailType.Bag && inven.listLength > inven.SlotCnt)
                    inven.InventoryOver();
                //만약 가방이 버려져 가방 칸 수보다 아이템 수가 더 길어지는 예외를 처리해줬다.
                inven.RemoveItem(equipSlotnum, item.itemType);
            }
        }        
    }
    //입수 할 때 증가를 해당 스크립트에서 하지 않은 이유는 장비칸의 redraw함수 때문이다.
    //인덱스를 받아서 해당 칸만 업데이트하는게 더 효율적이지만 솔직히 수정하기 좀 귀찮아서 그냥 이렇게 했다. 어차피 성능 차이도 많이 안나고해서링


    public void OnPointerUp(PointerEventData eventData)
    {
        if (isItem && !isDrag)
        {
            if (eventData.button == btn2)
            {
                DropUpdate();
            }
        }
    }// 우클릭시 아이템 드랍




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
            bool isDrop = false;
            if (itemIcon.transform.position.x < baseRect.xMin + baseRectPosition.x
           || itemIcon.transform.position.x > baseRect.xMax + baseRectPosition.x
           || itemIcon.transform.position.y < baseRect.yMin + baseRectPosition.y
           || itemIcon.transform.position.y > baseRect.yMax + baseRectPosition.y)
            {
                DropUpdate();
                
            }//인벤토리 밖으로 아이템을 드래그 했다면 아이템을 전부 버린다.  

            if (equipSlotnum == 0 || equipSlotnum == 1) {
                if (!isDrop)
                {
                    inven.SwapItem(equipSlotnum, itemIcon.transform.position);
                }      
            }//무기만 교체 가능하게 할것이기 때문에 조건을 걸었다. 옮겨진 이미지 위치와 현재 슬롯번호를 보낸다.

            itemIcon.transform.SetParent(parentObj);
            itemIcon.transform.position = transform.position;
            isDrag = false;
            inven.isDrag = isDrag;
        }
    }//아이템의 이미지가 뜨는 위치를 원 상태로, 아이템의 부모를 원래 슬롯으로 바꾼다.

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
