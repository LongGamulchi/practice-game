using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ItemEft/Consumable/Health")]
// ScriptableObject는 변수를 일괄적이고 편하게 바꿀수 있는 장점이 있다.
// 추상 메소드의 모든 동작을 구현해야 하며, 인게임 내의 변화가 게임이 끝나고나서도 유지된다.
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
    }//인벤토리 내의 회복 아이템을 사용시 작동한다.
}
