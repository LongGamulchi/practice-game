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
        }//invoke로 공격 딜레이를만들었다.
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
    }//invoke를 사용하여 베는 동작이 나올 때에만 공격이 가능하도록 해줬으며, 공격 딜레이를 추가해줬다.
}//비활성화 되는 오브젝트에는 코루틴을 사용하지 않는것이 좋다. 코루틴이 중간에 멈춰버린다.
