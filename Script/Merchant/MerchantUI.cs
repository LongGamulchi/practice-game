using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{    
    Inventory inven;
    public GameObject fielItemPrefab;
    public bool activeMerchant = false;
    public bool canOpenMerchantUI;
    public GameObject merchantPanel;
    public MerchantSlot[] merchantSlots;
    public List<Item> tempitems = new List<Item>();
    public Transform merchantlotHolder;
    public Text lessGold;
    public Text merchantSpeech;
    public string[] speechs = { "�������. ���� ì�ܿ��̰���?",
        "���ôٽ��� ���� �����̶��ϴ�. ���ϴ� ������ ó��������??", "������ ���ص帮�ڳ׿�.",
        "���� �� ��������. �װ��� �� ���� ���ݾƿ�.", "�� ���� ������ ä���ּ���."};
    Vector3 playerPosition;

    void Start()
    {
        merchantSlots = merchantlotHolder.GetComponentsInChildren<MerchantSlot>();
        merchantPanel.SetActive(activeMerchant);
        for(int i = 0; i<12; i++)
        {
            merchantSlots[i].Slotnum = i;
        }
    }//�ڽ� ������ ��� ������ ���� ��ȣ�� �ű��.
    
    void Update()
    {
        if (canOpenMerchantUI)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                activeMerchant = !activeMerchant;
                merchantPanel.SetActive(activeMerchant);
                SpeechUpdate();
            }
            lessGold.text = "Gold : " + inven.totalGold.ToString();
        }
    }//���ΰ� ������ �������� tabŰ�� ui�� Ű�� �ݴ´�. text�� total ���� update�Ѵ�.


    public void RedrawSlotUI(List<Item> items, Vector3 position, Inventory _inven)
    {
        inven = _inven;
        playerPosition = position;
        tempitems = items;
        canOpenMerchantUI = true;

        if (inven.nowOpneInven)
        {
            activeMerchant = true;
            merchantPanel.SetActive(activeMerchant);
            SpeechUpdate();
        }//�κ��丮�� �����ִٸ� ����ui�� �����̴�.
        for (int i = 0; i < merchantSlots.Length; i++)
        {
            merchantSlots[i].RemoveSlot();
        }
        for (int i = 0; i <items.Count; i++)
        {
            merchantSlots[i].item = items[i];
            merchantSlots[i].UpdateSlotUI(_inven);
        }
    }//ui�� �ֽ�ȭ�ߴ�.


    public void RemoveItem(int index)
    {
        tempitems.RemoveAt(index);
        RedrawSlotUI(tempitems, playerPosition, inven);
    }//���� ���� �������� update�� list�� ���������� �ʾҴµ� �˾Ƽ� �������ְ� �ִ�. �Ŀ� ���װ� ����� �̺κ��� ������ ����.

    public void offMerchantPanel()
    {
        activeMerchant = false;
        canOpenMerchantUI = false;
        merchantPanel.SetActive(activeMerchant);        
    }//ui�� ����.

    public void SpeechUpdate()
    {
        if (inven.totalGold > 1)
            merchantSpeech.text = speechs[Random.Range(0, 5)];
        else
            merchantSpeech.text = "<color=red>�������� ����!!!!!!!</color>";
    }//���� �ִ� ���·� ui�� Ű�� ����� �ϳ��� �ϰ� ���µ� ui�� Ų�ٸ� ȭ�� ����.

    public void AddItem(Item item)
    {
        Vector3 randomPostion = new Vector3(Random.Range(-1f, 1f), 0.8f, Random.Range(-1f, 1f));
        Vector3 dropPosition = playerPosition + randomPostion;
        GameObject go = Instantiate(fielItemPrefab, dropPosition , Quaternion.identity);
        go.GetComponent<FieldItems>().SetItem(item);
        inven.PickUp(go.GetComponent<FieldItems>());
    }//������ �������� �÷��̾� �ֺ��� �����Ͽ� �ٷ� �����ϰ��Ѵ�.
    /*
     �κ��丮�� additem�� �̿��Ͽ� �ٷ� �κ��丮�� �������� �����ϴ� ����� ����߾����� �κ��丮�� �� �������� ��ȭ�� ����
     ������ �ִ� �����ۿ��� �Ȱ��� ��ȭ�� �Ͼ�� ���װ� �߻��ߴ�.
     ��� ���׶�⺸�ٴ� ������ �������� �״�� �κ��丮 ������ ��Ͽ��� �߰��Ѵٴ� ����� �����Ͽ� ���� ��������.
     (�׷��ϱ� �ϳ��� Ŭ������ ���� ��ũ��Ʈ���� �����ϴ� ������ �Ǿ�������̴�.)
     �� ������ �ذ��ϱ� ���� ������ �������� ������ ���� fielditem�� �������ְ�, �� �������� �ٷ� ���� �� �� �ı����־���.
     */
}
