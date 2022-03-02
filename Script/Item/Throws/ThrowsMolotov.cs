using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowsMolotov : Weapon
{
    PlayerEquipItemController equipItemController;
    Inventory inven;

    public Transform range;
    public Transform throwsPosition;
    public Molotov molotov;

    void Start()
    {
        inven = GetComponentInParent<Inventory>();
        equipItemController = GetComponentInParent<PlayerEquipItemController>();
        equipItemController.onAttackCallWithMousePosition += Throw;
    }

    private void OnEnable()
    {
        range.gameObject.SetActive(true);
        range.localScale = new Vector3(46, 0.05f, 46);

    }
    private void OnDisable()
    {
        range.gameObject.SetActive(false);
    }


    public void Throw(Vector3 mousePosition)
    {
        if (this.transform.gameObject.activeSelf)
        {
            
            if (Input.GetMouseButtonDown(0) && inven.CanBeUse("Molotov"))
            {                
                Vector3 targetPosition = mousePosition - throwsPosition.position;
                if (targetPosition.magnitude < 23)
                {
                    stamina = targetPosition.magnitude;
                    StaminaUpdate();
                    Molotov newMolotov = Instantiate(molotov, throwsPosition.position, throwsPosition.rotation) as Molotov;
                    newMolotov.Throw(targetPosition, GetComponentInParent<Player>().gameObject);
                    newMolotov.Ignite();
                    inven.FastUse("Molotov");
                }
                if (!inven.CanBeUse("Molotov"))
                {
                    equipItemController.nowHoldThrow = false;
                    this.transform.gameObject.SetActive(false);
                    equipItemController.CanThrowsUse();
                }
                /*던져질때마다 인벤토리의 remove를 하고 아직 남아있는지 체크 남아있지 않다면 비활*/
            }
        }
    }

}
