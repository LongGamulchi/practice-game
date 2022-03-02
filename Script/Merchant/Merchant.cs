using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public MerchantUI ui;
    public ItemDatabase itemDB;
    Inventory inven;
    //itemDB의 리스트를 상속받아서 상인의 판매물품을 구성할것이다.

    public int randomMin;
    public int randomMax;

    private void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            items.Add(itemDB.itemDB[Random.Range(randomMin, randomMax)]);
        }
        items.Sort(Utility.compare);
    }//상인의 물품리스트를 생성 후 정렬해줬다.

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inven = other.GetComponent<Inventory>();
            ui.RedrawSlotUI(items, other.transform.position, inven);
        }
    }//상인의 트리거 안으로 들어갈 때 ui가 해당 상인의 ui로 최신화된다.

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ui.offMerchantPanel();
            items.Sort(Utility.compare);
        }
    }//상인과 멀어지면 강제로 ui를 끈다.
}
