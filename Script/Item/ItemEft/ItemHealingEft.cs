using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ItemEft/Consumable/Health")]
// ScriptableObject�� ������ �ϰ����̰� ���ϰ� �ٲܼ� �ִ� ������ �ִ�.
// �߻� �޼ҵ��� ��� ������ �����ؾ� �ϸ�, �ΰ��� ���� ��ȭ�� ������ ���������� �����ȴ�.
public class ItemHealingEft : ItemEffect
{
    public int healingPoint;
    public override bool ExecuteRole(GameObject usePlayer)
    {
        Player player = usePlayer.GetComponent<Player>();        
        if (player.health < player.maxHealth)
        {
            player.health += healingPoint;
            if (player.health > player.maxHealth)
                player.health = player.maxHealth;
            return true;
        }
        else return false;
    }//�κ��丮 ���� ȸ�� �������� ���� �۵��Ѵ�.
}
