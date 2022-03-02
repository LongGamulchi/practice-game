using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public Player player;
    public float stamina;
    //여기에 무기에는 쓰이지 않는 아이템 값들을 이용해서 무기의 데미지와 스테미나 소모량 관리를 무기별로 관리하기 편하게하자.

    public void StaminaUpdate()
    {
        player.stamina -= stamina;
    }
}//무기라면 보통 가지고 있어야 할 정보들을 가지고있다.
