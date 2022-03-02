using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    PlayerEquipItemController equipItemController;
    Animator anim;
    public BoxCollider melee;
    public bool attackready;

    void Start()
    {
        equipItemController = GetComponentInParent<PlayerEquipItemController>();
        equipItemController.onAttackCall += Swing;
        anim = GetComponentInParent<Animator>();
        damage = 25;
        stamina = 20;
    }

    void Swing()
    {
        if (this.transform.gameObject.activeSelf)
        {
            if (attackready)
            {
                StaminaUpdate();
                attackready = false;
                anim.SetTrigger("doAttack");
                Invoke("OnAttack", 0.28f);
                Invoke("OffAttack", 0.66f);
                Invoke("AttackDelay", 1);
            }
        }//invoke�� ���� �����̸��������.
    }

    public void OnTriggerEnter(Collider other)
    {

            IDamageble damagebleObject = other.GetComponent<IDamageble>();
            if (damagebleObject != null)
            {
                damagebleObject.TakeDamageEnemy(damage, GetComponentInParent<Player>().gameObject);
            }   
    }



    void OnAttack()
    {
        melee.GetComponent<BoxCollider>().enabled = true;
    }
    void OffAttack()
    {
        melee.GetComponent<BoxCollider>().enabled = false;
    }
    void AttackDelay()
    {
        attackready = true;
    }//invoke�� ����Ͽ� ���� ������ ���� ������ ������ �����ϵ��� ��������, ���� �����̸� �߰������.
}//��Ȱ��ȭ �Ǵ� ������Ʈ���� �ڷ�ƾ�� ������� �ʴ°��� ����. �ڷ�ƾ�� �߰��� ���������.
