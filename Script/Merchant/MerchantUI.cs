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
    public string[] speechs = { "어서오세요. 돈은 챙겨오셨겠죠?",
        "보시다시피 저는 가방이랍니다. 말하는 가방은 처음보나요??", "할인은 못해드리겠네요.",
        "돈을 다 쓰고가세요. 죽고나선 쓸 일이 없잖아요.", "제 안을 돈으로 채워주세요."};
    Vector3 playerPosition;

    void Start()
    {
        merchantSlots = merchantlotHolder.GetComponentsInChildren<MerchantSlot>();
        merchantPanel.SetActive(activeMerchant);
        for(int i = 0; i<12; i++)
        {
            merchantSlots[i].Slotnum = i;
        }
    }//자식 슬롯을 모두 가져와 슬롯 번호를 매긴다.
    
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
    }//상인과 가까이 있을때만 tab키로 ui를 키고 닫는다. text는 total 골드로 update한다.


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
        }//인벤토리가 열려있다면 상인ui도 열것이다.
        for (int i = 0; i < merchantSlots.Length; i++)
        {
            merchantSlots[i].RemoveSlot();
        }
        for (int i = 0; i <items.Count; i++)
        {
            merchantSlots[i].item = items[i];
            merchantSlots[i].UpdateSlotUI(_inven);
        }
    }//ui를 최신화했다.


    public void RemoveItem(int index)
    {
        tempitems.RemoveAt(index);
        RedrawSlotUI(tempitems, playerPosition, inven);
    }//지금 아직 상인한테 update된 list를 리턴해주지 않았는데 알아서 리턴해주고 있다. 후에 버그가 생기면 이부분을 유심히 보자.

    public void offMerchantPanel()
    {
        activeMerchant = false;
        canOpenMerchantUI = false;
        merchantPanel.SetActive(activeMerchant);        
    }//ui를 끈다.

    public void SpeechUpdate()
    {
        if (inven.totalGold > 1)
            merchantSpeech.text = speechs[Random.Range(0, 5)];
        else
            merchantSpeech.text = "<color=red>돈없으면 나가!!!!!!!</color>";
    }//돈이 있는 상태로 ui를 키면 대사중 하나를 하고 없는데 ui를 킨다면 화를 낸다.

    public void AddItem(Item item)
    {
        Vector3 randomPostion = new Vector3(Random.Range(-1f, 1f), 0.8f, Random.Range(-1f, 1f));
        Vector3 dropPosition = playerPosition + randomPostion;
        GameObject go = Instantiate(fielItemPrefab, dropPosition , Quaternion.identity);
        go.GetComponent<FieldItems>().SetItem(item);
        inven.PickUp(go.GetComponent<FieldItems>());
    }//구매한 아이템을 플레이어 주변에 생성하여 바로 습득하게한다.
    /*
     인벤토리의 additem을 이용하여 바로 인벤토리에 아이템을 습득하는 방식을 사용했었으나 인벤토리에 들어간 아이템의 변화에 따라
     상점에 있는 아이템에도 똑같은 변화가 일어나는 버그가 발생했다.
     사실 버그라기보다는 상인의 아이템을 그대로 인벤토리 아이템 목록에도 추가한다는 사실을 간과하여 생긴 문제였다.
     (그러니까 하나의 클래스를 여러 스크립트에서 참조하는 형식이 되어버린것이다.)
     이 문제를 해결하기 위해 상인의 아이템의 정보를 가진 fielditem을 생성해주고, 그 아이템을 바로 습득 한 후 파괴해주었다.
     */
}
