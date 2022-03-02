using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Sheild")]
public class SheildRecovery : ItemEffect
{
    public int RecoveryPoint;
    public override bool ExecuteRole(GameObject usePlayer)
    {
        SheildController sheild = usePlayer.GetComponent<SheildController>();
        if (sheild.sheild < sheild.maxSheild)
        {
            sheild.sheild += RecoveryPoint;
            if (sheild.sheild > sheild.maxSheild)
                sheild.sheild = sheild.maxSheild;
            sheild.UpdateItemInfo();
            return true;
        }
        else return false;
    }//실드량을 채운다.
}
