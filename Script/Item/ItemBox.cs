using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : LivingEntity
{
    public List<Item> items = new List<Item>();
    public ItemDatabase itemDB;
    public int randomMin;
    public int randomMax;
    public override void Start()
    {
        isitem = true;
        OnDeath += DropItems;
        for (int i = 0; i < Random.Range(2,5); i++)
        {
            items.Add(itemDB.itemDB[Random.Range(randomMin, randomMax)]);
        }
    }

    void DropItems()
    {
        foreach (Item item in items)
        {
            ItemDatabase.instance.Drop(item, this.transform.position);
        }
    }

}
