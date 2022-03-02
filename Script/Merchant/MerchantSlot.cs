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
        }//살 수 있는 물건은 노랑, 살 수없는 물건은 빨간색으로 가격을 띄운다.
    }

    public void UpdateSlotUI(Inventory _inven)
    {
        inven = _inven;
        isItem = true;
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);        
    }//아이템 슬롯 최신화
    public void RemoveSlot()
    {
        isItem = false;
        item = null;
        itemIcon.gameObject.SetActive(false);
    }//아이템이 제거 될 떄 해당 슬롯을 비활성

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isItem)
        {
            if (inven.totalGold > 0 && inven.totalGold >= item.price)
            {
                ui.AddItem(item);
                merchantSpeech.text = "정말 좋은 거래에요.";
                inven.totalGold -= item.price;
                inven.tempPrice = item.price;
                inven.FastUse("Gold");
                RemoveSlot();
                ui.RemoveItem(Slotnum);

            }
            else
                merchantSpeech.text = "혹시 돈이 없으신가요?";
            //0원에서는 살 수 없다. 만약 살수 있다면 돈을 사용하여 산다.
        }
        else
            merchantSpeech.text = "거긴 아무것도 없잖아 병신이니?";
        //장난으로 넣었다.
    }
}

