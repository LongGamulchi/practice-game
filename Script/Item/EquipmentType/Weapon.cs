using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public Player player;
    public float stamina;
    //���⿡ ���⿡�� ������ �ʴ� ������ ������ �̿��ؼ� ������ �������� ���׹̳� �Ҹ� ������ ���⺰�� �����ϱ� ���ϰ�����.

    public void StaminaUpdate()
    {
        player.stamina -= stamina;
    }
}//������ ���� ������ �־�� �� �������� �������ִ�.
