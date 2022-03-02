using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;
    Player player;


    private void Awake()
    {
        instance = this;
    }//생성되는 즉시 작동하는 Awake
    public List<Item> itemDB = new List<Item>();
    //아이템의 총 리스트

    [Space(20)]//인스펙터 여백
    public GameObject fielItemPrefab;
    public Vector3[] pos;
    public int randomMin;
    public int randomMax;

    private void Start()
    {
        for(int i = 0; i<pos.Length; i++)
        {
            GameObject go = Instantiate(fielItemPrefab, pos[i], Quaternion.identity);
            go.GetComponent<FieldItems>().SetItem(itemDB[Random.Range(randomMin, randomMax)]);
        }//i개만큼 랜덤한 아이템을 사전 설정한 위치에 생성.        
    }
    public bool Drop(Item invenItemPrefab, Vector3 dropPosition)
    {
        Vector3 randomPostion = new Vector3(Random.Range(-1.5f, 1.5f), 0.8f, Random.Range(-1.5f, 1.5f));
        if (dropPosition.y > 10f)
        {
            player = GameObject.Find("dummy").GetComponent<Player>();
            dropPosition = player.position + randomPostion;
        }
        else
            dropPosition += randomPostion;
       
        GameObject go = Instantiate(fielItemPrefab, dropPosition, Quaternion.identity);
        go.GetComponent<FieldItems>().SetItem(invenItemPrefab);
        if (go != null)
            return true;
        else
            return false;
    }//플레이어 위치에 아이템을 떨어트렸다면 ture 아니라면 false
}
