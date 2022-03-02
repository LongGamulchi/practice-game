using UnityEngine;
using System.Collections;

public class CrossBow : Weapon
{
    PlayerEquipItemController equipItemController;
    public Transform muzzle;
    public Transform range;
    public CrossBowArrow crossBowArrow;
    public float msBetweenShots = 1000;
    public float muzzleVelocity = 35;
    float nextShotTime;

    void Start()
    {
        equipItemController = GetComponentInParent<PlayerEquipItemController>();
        equipItemController.onAttackCall += Shoot;
    }

    private void OnEnable()
    {
        range.gameObject.SetActive(true);
        range.localScale = new Vector3(30f, 0.15f, 30f);

    }
    private void OnDisable()
    {
        range.localScale = new Vector3(0.15f, 0.15f, 0f);
        range.gameObject.SetActive(false);        
    }

    public void Shoot()
    {
        if (this.transform.gameObject.activeSelf)
        {
            if (Time.time > nextShotTime)
            {
                StaminaUpdate();
                nextShotTime = Time.time + msBetweenShots / 800;
                CrossBowArrow newArrow = Instantiate(crossBowArrow, muzzle.position, muzzle.rotation) as CrossBowArrow;
                newArrow.Shooter(GetComponentInParent<Player>().gameObject);
                newArrow.SetSpeed(muzzleVelocity);
            }
        }
    }//발사딜레이가 아닐때 shoot한다.

}