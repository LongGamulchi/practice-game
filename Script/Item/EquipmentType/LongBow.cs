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
    }//총구에 해당하는 오브젝트를 피벗으로 하여 사거리를 표시할 오브젝트를 붙여주었다.

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
                //활의 최소 사거리 이상이 되면 시각적으로 알려줄것이다.

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
        }//마우스를 누르고 있으면 사거리가 증가하며 일정 사거리 이상일때 마우스를 때면 쏜다.
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