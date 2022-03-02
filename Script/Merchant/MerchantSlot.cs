using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MerchantSlot : MonoBehaviour, IPointerUpHandler
{
    private Rect baseRect;
    private Vector3 baseRectPosition;
    public PointerEventData.InputButton btn1 = PointerEventData.InputButton.Left;
    public PointerEventData.InputButton btn2 = PointerEventData.InputButton.Right;


    public int Slotnum;
    public bool isItem;
    public Item item;
    public Image itemIcon;
    public Text itemPrice;
    public Text merchantSpeech;

    Inventory inven;
    MerchantUI ui;



    public void Start()
    {
        ui = GetComponentInParent<MerchantUI>();
    }

    public void Update()
    {
        if (isItem)
        {
            if (item.price > inven.totalGold)
                itemPrice.text = "<color=red>" + item.price.ToString() + "</color>";
            else
                itemPrice.text = "<color=yellow>" + item.price.ToString() + "</color>";
        }//�� �� �ִ� ������ ���, �� ������ ������ ���������� ������ ����.
    }

    public void UpdateSlotUI(Inventory _inven)
    {
        inven = _inven;
        isItem = true;
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);        
    }//������ ���� �ֽ�ȭ
    public void RemoveSlot()
    {
        isItem = false;
        item = null;
        itemIcon.gameObject.SetActive(false);
    }//�������� ���� �� �� �ش� ������ ��Ȱ��

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isItem)
        {
            if (inven.totalGold > 0 && inven.totalGold >= item.price)
            {
                ui.AddItem(item);
                merchantSpeech.text = "���� ���� �ŷ�����.";
                inven.totalGold -= item.price;
                inven.tempPrice = item.price;
                inven.FastUse("Gold");
                RemoveSlot();
                ui.RemoveItem(Slotnum);

            }
            else
                merchantSpeech.text = "Ȥ�� ���� �����Ű���?";
            //0�������� �� �� ����. ���� ��� �ִٸ� ���� ����Ͽ� ���.
        }
        else
            merchantSpeech.text = "�ű� �ƹ��͵� ���ݾ� �����̴�?";
        //�峭���� �־���.
    }
}

