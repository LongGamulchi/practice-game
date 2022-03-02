using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public MerchantUI ui;
    public ItemDatabase itemDB;
    Inventory inven;
    //itemDB�� ����Ʈ�� ��ӹ޾Ƽ� ������ �ǸŹ�ǰ�� �����Ұ��̴�.

    public int randomMin;
    public int randomMax;

    private void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            items.Add(itemDB.itemDB[Random.Range(randomMin, randomMax)]);
        }
        items.Sort(Utility.compare);
    }//������ ��ǰ����Ʈ�� ���� �� ���������.

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inven = other.GetComponent<Inventory>();
            ui.RedrawSlotUI(items, other.transform.position, inven);
        }
    }//������ Ʈ���� ������ �� �� ui�� �ش� ������ ui�� �ֽ�ȭ�ȴ�.

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ui.offMerchantPanel();
            items.Sort(Utility.compare);
        }
    }//���ΰ� �־����� ������ ui�� ����.
}
