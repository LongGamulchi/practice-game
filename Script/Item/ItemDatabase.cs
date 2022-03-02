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
    }//�����Ǵ� ��� �۵��ϴ� Awake
    public List<Item> itemDB = new List<Item>();
    //�������� �� ����Ʈ

    [Space(20)]//�ν����� ����
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
        }//i����ŭ ������ �������� ���� ������ ��ġ�� ����.        
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
    }//�÷��̾� ��ġ�� �������� ����Ʈ�ȴٸ� ture �ƴ϶�� false
}
