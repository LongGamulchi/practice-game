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
    }//�κ��丮 â�� ���� ��ǥ�� rect.xMax ���� ���Ѱ��� ui���� �κ��丮 ���� ��ǥ���� �ٱ��� ��ǥ���� �Ǻ��� �� �ִ� ������ �ȴ�.

    public void UpdateSlotUI()
    {
        if (item.itemType != ItemType.Null)
        {
            isItem = true;
            itemIcon.sprite = item.itemImage;
            itemIcon.gameObject.SetActive(true);
        }
    }//�κ��丮 ���â ������Ʈ

    public void RemoveSlot()
    {

        isItem = false;
        item = null;
        itemIcon.gameObject.SetActive(false);
    }//�������� ���� �� �� �ش� ������ ��Ȱ��

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
                }//���������� �κ��丮���� ��� �Լ� �� �� �����.
                if (item.detailType == DetailType.Bag && inven.listLength > inven.SlotCnt)
                    inven.InventoryOver();
                //���� ������ ������ ���� ĭ ������ ������ ���� �� ������� ���ܸ� ó�������.
                inven.RemoveItem(equipSlotnum, item.itemType);
            }
        }        
    }
    //�Լ� �� �� ������ �ش� ��ũ��Ʈ���� ���� ���� ������ ���ĭ�� redraw�Լ� �����̴�.
    //�ε����� �޾Ƽ� �ش� ĭ�� ������Ʈ�ϴ°� �� ȿ���������� ������ �����ϱ� �� �����Ƽ� �׳� �̷��� �ߴ�. ������ ���� ���̵� ���� �ȳ����ؼ���


    public void OnPointerUp(PointerEventData eventData)
    {
        if (isItem && !isDrag)
        {
            if (eventData.button == btn2)
            {
                DropUpdate();
            }
        }
    }// ��Ŭ���� ������ ���




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
            bool isDrop = false;
            if (itemIcon.transform.position.x < baseRect.xMin + baseRectPosition.x
           || itemIcon.transform.position.x > baseRect.xMax + baseRectPosition.x
           || itemIcon.transform.position.y < baseRect.yMin + baseRectPosition.y
           || itemIcon.transform.position.y > baseRect.yMax + baseRectPosition.y)
            {
                DropUpdate();
                
            }//�κ��丮 ������ �������� �巡�� �ߴٸ� �������� ���� ������.  

            if (equipSlotnum == 0 || equipSlotnum == 1) {
                if (!isDrop)
                {
                    inven.SwapItem(equipSlotnum, itemIcon.transform.position);
                }      
            }//���⸸ ��ü �����ϰ� �Ұ��̱� ������ ������ �ɾ���. �Ű��� �̹��� ��ġ�� ���� ���Թ�ȣ�� ������.

            itemIcon.transform.SetParent(parentObj);
            itemIcon.transform.position = transform.position;
            isDrag = false;
            inven.isDrag = isDrag;
        }
    }//�������� �̹����� �ߴ� ��ġ�� �� ���·�, �������� �θ� ���� �������� �ٲ۴�.

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
