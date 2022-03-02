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
    //�� �̺�Ʈ�� ���� ���� �׾��ٴ°��� �˰� �� ���̴�.

    GameObject attackedObj;

    public virtual void Start()
    {
        inven = GetComponent<Inventory>();
        sheild = GetComponent<SheildController>();
        health = maxHealth;
    }//ü�� = ����ü��

    public void TakeHitEnemy(float damage, RaycastHit hit, GameObject _attackedObj)
    {
        attackedObj = _attackedObj;
        TakeDamageEnemy(damage, attackedObj);        
    }//hit �� ��ġ�� ã�ƾ� �� �� �����ϰ� ����� �� �ֱ� ������ hit���� damage�� ���� �ϴ½����� ���������

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
                }//���� ���°� �ƴϸ鼭 �ǰ� 0���ϸ� �״´�.
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
        
    }//�ǵ� ���� ���� ������ ������ �Դ°��� �ΰ��� �������.
    //�Ѵ� �Ȱ��� ������ ������ ���ϱ� ���� �����Ҷ� �� �ǵ尡 ���̴°��� �ƴѰ�. 

    public void TakeDamageEnemy(float damage, GameObject _attackedObj)
    {
        attackedObj = _attackedObj;
        if (damage > health)
            damage = health;//�ʰ��� �������� �����Ѵ�.

        if (!isitem && attackedObj.layer == LayerMask.NameToLayer("Player"))
        {
            if (attackedObj.GetComponent<Inventory>().equipments[2].itemType != ItemType.Null)
                attackedObj.GetComponent<SheildController>().TakeDealing(damage);//���� ��������ŭ �ǵ忡 ����            
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
        GameObject Text = Instantiate(damageText[i]); // ������ �ؽ�Ʈ ������Ʈ
        Vector3 textPosition = new Vector3(Random.Range(-0.5f, 0.5f), 2, Random.Range(-0.5f, 0.5f));
        Text.transform.position = this.transform.position + textPosition; // ǥ�õ� ��ġ
        Text.GetComponent<DamageText>().damage = (int)damage; // ������ ����
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
    }//�״´ٸ� dead ture�̸� ������Ʈ�� ���ش�.

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
