using UnityEngine;
using System.Collections;

public class LongBow : Weapon
{
    PlayerEquipItemController equipItemController;
    public Transform muzzle;
    public LongBowArrow arrow;
    public float distance = 0;
    
    void  Start()
    {
        equipItemController = GetComponentInParent<PlayerEquipItemController>();
        equipItemController.onAttackCall += Shoot;
        muzzle.gameObject.SetActive(false);
    }//�ѱ��� �ش��ϴ� ������Ʈ�� �ǹ����� �Ͽ� ��Ÿ��� ǥ���� ������Ʈ�� �ٿ��־���.

    public void Shoot()
    {
        if (this.transform.gameObject.activeSelf)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                muzzle.gameObject.SetActive(true);
                distance += 0.1f;
                muzzle.localScale = new Vector3(0.15f, 0.15f, distance);

                if (distance > 10)
                {
                    muzzle.GetComponentInChildren<Range>().UpdateMaterialOn();
                }
                //Ȱ�� �ּ� ��Ÿ� �̻��� �Ǹ� �ð������� �˷��ٰ��̴�.

                if (distance > 25)
                {
                    stamina = 0.05f;
                    StaminaUpdate();
                    distance = 25;
                    muzzle.localScale = new Vector3(0.15f, 0.15f, distance);
                }
                else
                {
                    stamina = 0.1f;
                    StaminaUpdate();
                }

                if (player.stamina <= 0f)
                {
                    Shooting();
                }
            }
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                Shooting();
            }
        }//���콺�� ������ ������ ��Ÿ��� �����ϸ� ���� ��Ÿ� �̻��϶� ���콺�� ���� ���.
    }

    public void Shooting()
    {
        if (distance > 10)
        {
            LongBowArrow newArrow = Instantiate(arrow, muzzle.position, muzzle.rotation) as LongBowArrow;
            newArrow.Shooter(GetComponentInParent<Player>().gameObject);
            newArrow.Crossroad(distance, distance + 8);

        }
        distance = 0;
        muzzle.localScale = new Vector3(0.15f, 0.15f, 0f);
        muzzle.GetComponentInChildren<Range>().UpdateMaterialOff();
        muzzle.gameObject.SetActive(false);
    }
}