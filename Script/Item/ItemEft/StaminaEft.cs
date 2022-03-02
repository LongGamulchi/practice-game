using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Stamina")]
// ScriptableObject�� ������ �ϰ����̰� ���ϰ� �ٲܼ� �ִ� ������ �ִ�.
// �߻� �޼ҵ��� ��� ������ �����ؾ� �ϸ�, �ΰ��� ���� ��ȭ�� ������ ���������� �����ȴ�.
public class StaminaEft : ItemEffect
{
    public override bool ExecuteRole(GameObject usePlayer)
    {
        Player player = usePlayer.GetComponent<Player>();
        if (!player.staminaInfinity)
        {
            player.staminaInfinity = true;
            return true;
        }
        return false;
    }//�κ��丮 ���� ȸ�� �������� ���� �۵��Ѵ�.
}
