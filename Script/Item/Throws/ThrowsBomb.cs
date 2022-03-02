using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowsBomb : Weapon
{
    PlayerEquipItemController equipItemController;
    Inventory inven;

    public Transform range;
    public Transform throwsPosition;
    public Bomb bomb;

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
            
            if (Input.GetMouseButtonDown(0) && inven.CanBeUse("Bomb"))
            {               
                Vector3 targetPosition = mousePosition - throwsPosition.position;


                if (targetPosition.magnitude < 23)
                {
                    stamina = targetPosition.magnitude;
                    StaminaUpdate();
                    Bomb newBomb = Instantiate(bomb, throwsPosition.position, throwsPosition.rotation) as Bomb;
                    newBomb.Throw(targetPosition, GetComponentInParent<Player>().gameObject);
                    newBomb.Ignite();
                    inven.FastUse("Bomb");
                }                
                if (!inven.CanBeUse("Bomb"))
                {
                    equipItemController.nowHoldThrow = false;     
                    this.transform.gameObject.SetActive(false);
                    equipItemController.CanThrowsUse();
                }                
            }
        }
    }
}
