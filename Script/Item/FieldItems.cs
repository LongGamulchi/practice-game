using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItems : MonoBehaviour
{
    public Item item;



    public void SetItem(Item _item)
    {

        item.itemName = _item.itemName;
        item.itemType = _item.itemType;
        item.detailType = _item.detailType;
        item.itemImage = _item.itemImage;
        item.itemStack = _item.itemStack;
        item.efts = _item.efts;
        item.sortValue = _item.sortValue;
        item.evolve = _item.evolve;
        item.price = _item.price;
        //Item의 기본정보를 가지고 왔다

        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (item.itemName == this.transform.GetChild(i).name)
                this.transform.GetChild(i).gameObject.SetActive(true);
            else
                this.transform.GetChild(i).gameObject.SetActive(false);
        }//아이템의 이름을 모델과 비교하여 이름이 같다면 활성화

        if(item.detailType == DetailType.Armor)
        {            
            this.GetComponentInChildren<FieldSheild>().UpdateColor(item.itemStack);
        }//아이템이 아머일경우 아머의 상태를 확인하고 떨어진 아이템의 외관을 업데이트한다.

    }
    public Item GetItem()
    {
        return item;
    }//아이템 습득

    public void DestroyItem()
    {
        Destroy(gameObject);
    }//주운 아이템 파괴
}
