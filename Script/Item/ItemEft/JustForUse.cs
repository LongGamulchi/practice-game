using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ItemEft/Consumable/JustForUse")]
public class JustForUse : ItemEffect
{
    public override bool ExecuteRole(GameObject usePlayer)
    {
        return true;
    }
}
