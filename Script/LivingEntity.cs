using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageble
{
    public Inventory inven;
    public SheildController sheild;
    public GameObject[] damageText;
    public float maxHealth;
    public float health;
    public float downHealth;
    public float maxStamina;
    public float stamina;
    public bool outsideRing;
    public bool isRingDamageWait;
    public bool isitem;
    protected bool dead;
    protected bool down;
    public float totalDeal;

    public event System.Action OnDeath;
    //이 이벤트를 통해 적이 죽었다는것을 알게 할 것이다.

    GameObject attackedObj;

    public virtual void Start()
    {
        inven = GetComponent<Inventory>();
        sheild = GetComponent<SheildController>();
        health = maxHealth;
    }//체력 = 시작체력

    public void TakeHitEnemy(float damage, RaycastHit hit, GameObject _attackedObj)
    {
        attackedObj = _attackedObj;
        TakeDamageEnemy(damage, attackedObj);        
    }//hit 한 위치를 찾아야 할 때 유용하게 사용할 수 있기 때문에 hit에서 damage를 실행 하는식으로 간략해줬다

    public void TakeDamagePlayer(float damage)
    {
        if (!down)
        {
            if (inven.equipments[2].itemType != ItemType.Null && sheild.sheild>0)
            {
                if (sheild.sheild >= damage )
                    DamageText(damage, 1);
                else
                    DamageText(sheild.sheild , 1);
                damage = sheild.TakeDamaged(damage);
                
            }
            if (inven.equipments[2].itemType == ItemType.Null || damage != 0)
            {
                if (damage > health)
                    damage = health;
                health -= damage;
                DamageText(damage,0);
                if (health <= 0 && !dead)
                {
                    health = 0;
                    if (gameObject.CompareTag("Player"))
                    {
                        Down();
                    }
                    else
                        Die();
                }//죽은 상태가 아니면서 피가 0이하면 죽는다.
            }
            
        }
        else if (down)
        {
            if (damage > downHealth)
                damage = downHealth;
            downHealth -= damage;
            DamageText(damage,0);
            if (downHealth <= 0 && !dead)
            {
                inven.isDead = true;
                inven.InventoryOver();
                downHealth = 0;
                Die();
            }
        }
        
    }//실드 관련 버그 때문에 데미지 입는곳을 두개로 나눠줬다.
    //둘다 똑같은 데미지 계산식을 쓰니까 적을 공격할때 내 실드가 까이는것이 아닌가. 

    public void TakeDamageEnemy(float damage, GameObject _attackedObj)
    {
        attackedObj = _attackedObj;
        if (damage > health)
            damage = health;//초과된 데미지는 무시한다.

        if (!isitem && attackedObj.layer == LayerMask.NameToLayer("Player"))
        {
            if (attackedObj.GetComponent<Inventory>().equipments[2].itemType != ItemType.Null)
                attackedObj.GetComponent<SheildController>().TakeDealing(damage);//넣은 데미지만큼 실드에 전달            
        }
        DamageText(damage,0);

        health -= damage;
        if (health <= 0 && !dead)
        {
            health = 0;
            Die();
        }
    }

    public void DamageText(float damage, int i)
    {
        GameObject Text = Instantiate(damageText[i]); // 생성할 텍스트 오브젝트
        Vector3 textPosition = new Vector3(Random.Range(-0.5f, 0.5f), 2, Random.Range(-0.5f, 0.5f));
        Text.transform.position = this.transform.position + textPosition; // 표시될 위치
        Text.GetComponent<DamageText>().damage = (int)damage; // 데미지 전달
    }

    protected void Down()
    {
        down = true;
    }

    protected void Die()
    {
        dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }//죽는다면 dead ture이며 오브젝트를 없앤다.

    public IEnumerator RingDamage()
    {
        Debug.Log("hi");
        float startTime = Time.time;
        isRingDamageWait = true;
        while(Time.time < startTime + 1)
        {
            if (!outsideRing)
            {
                isRingDamageWait = false;
                yield break;
            }
            yield return null;
        }
        isRingDamageWait = false;
        TakeDamagePlayer(Ring.ringDamage);
    }    
}
