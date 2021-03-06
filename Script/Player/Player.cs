using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(PlayerEquipItemController))]
[RequireComponent(typeof(SheildController))]
[RequireComponent(typeof(FieldOfView))]
public class Player : LivingEntity
{
    public enum State { Idle, Dashing, Moving };
    public State currentState;

    public float downMoveSpeed= 3;
    public float moveSpeed = 6;
    public float dashSpeed = 5;
    public float dashTime = 1;
    public float dashCoolTime = 1f;
    public float basicCoolTime = 1f;
    public float ultimitCoolTime = 1f;
    public float staminaHeal;


    public bool nowPunching;
    public bool drinkChargingPotion;
    public bool staminaInfinity;
    public bool isPushRightMouse;
    public bool isPushLeftMouse;
    public bool isAttack;
    public bool isdash;
    public bool isPickup;
    public bool activeInventory;
    public bool nowHealing;
    public bool isStaminaMax;
    public bool isExhausted;
    public LayerMask collisionMask;
    public Transform targetRange;
    public SkillCooltime dashSkillcool;
    public SkillCooltime basicSkillcool;
    public SkillCooltime ultimitSkillcool;
    public Camera minimapCamera;
    public Vector3 position;
    public BoxCollider melee;

    Animator anim;
    public Collider[] initialCollisions;
    Weapon weapon;
    PlayerController controller;
    PlayerEquipItemController equipController;
    Inventory inventory;
    public Camera viewCamera;
    FieldItems fieldItem;
    FieldItems beforeItem;
    Vector3 moveVelocity;
    Vector3 point;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public override void Start()
    {
        base.Start();
        currentState = State.Idle;
        controller = GetComponent<PlayerController>();
        equipController = GetComponent<PlayerEquipItemController>();
        weapon = GetComponent<Weapon>();
        inventory = GetComponent<Inventory>();
        targetRange.gameObject.SetActive(false);
    }

    void Update()
    {
        position = this.gameObject.transform.position;
        Cursor.lockState = CursorLockMode.Confined;
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);
        if (moveVelocity != Vector3.zero)
        {
            currentState = State.Moving;
        }
        else currentState = State.Idle;
        //???????? ????
        
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        //?????? ?????? ???? ?????? ?????? ?????? ?????? ???? ????

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        //?????? ???????? ???????? ?????? ????

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            point = ray.GetPoint(rayDistance);
            controller.LookAt(point);
            float distance = Vector3.Distance(position,point);
            targetRange.position = point;
        }
        //???????????? ?????? ?????? ???? ????

        if (Cursor.lockState == CursorLockMode.Confined)
            controller.CameraMove(position, point);
        minimapCamera.transform.position = position + Vector3.up * 40;
        //?????? ???????? ????

        anim.SetBool("isRun", moveVelocity != Vector3.zero);
        anim.SetBool("isWalk", moveSpeed < 4);
        //??????????

        if (equipController.nowHoldThrow && !down)
            targetRange.gameObject.SetActive(true);
        else
            targetRange.gameObject.SetActive(false);
        //???????? onoff
        if(!isAttack && !isStaminaMax && !staminaInfinity)
        {
            
            if (isExhausted)//???????? ?????????? ?????? ????????.
                stamina += staminaHeal/2;
            else
                stamina += staminaHeal;
            if (stamina > maxStamina)
            {
                isStaminaMax = true;
                stamina = maxStamina;
            }
        }//???????? ????

        if (staminaInfinity)
        {
            stamina = maxStamina;
            StartCoroutine(StaminaInfinity());

        }

        if(stamina <= 0)
        {
            stamina = 0.001f;
            isExhausted = true;
            StartCoroutine(StopAttackWait(2));
            StartCoroutine(Exhausted());
            
        }//????????.

        if(outsideRing && !isRingDamageWait)
        {
            StartCoroutine(RingDamage());
        }


        if (!down && !isExhausted)
        {
            if (!activeInventory && !nowHealing)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (currentState == State.Moving && !isdash)
                    {
                        isdash = true;
                        currentState = State.Dashing;
                        StartCoroutine(Dash());
                        StartCoroutine(DashCool());
                    }
                    else
                    {
                        Debug.Log("CoolTime or IdelState");
                    }
                }//???????? ???????????? ???????? ????????.

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    StartCoroutine(BasicSkillCool());
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    StartCoroutine(UltimitSkillCool());
                }


                if (Input.GetMouseButton(0) && !isPushRightMouse )
                {
                    isPushLeftMouse = true;
                    isAttack = true;
                    if (inventory.equipments[0].itemType != ItemType.Null || equipController.nowHoldThrow)
                    {
                        inventory.lastUseWaepon = 0;
                        equipController.WeaponUse(point);
                    }
                    else if(!nowPunching && !equipController.nowSwap)
                    {
                        anim.SetTrigger("doPunch");
                        StartCoroutine(Punch());
                    }

                }
                if (Input.GetMouseButtonUp(0) && !isPushRightMouse)
                {
                    if (inventory.equipments[0].itemType != ItemType.Null && !isPushRightMouse
                        && !equipController.nowHoldThrow)
                        equipController.WeaponUse(point);
                    StartCoroutine(StopAttackWait(0.4f));
                    isPushLeftMouse = false;

                }//?????????? ???? ???? ?????? ????????.



 
                if (Input.GetMouseButton(1) && !isPushLeftMouse
                        && !equipController.nowHoldThrow)
                {
                    isPushRightMouse = true;
                    isAttack = true;
                    if (inventory.equipments[1].itemType != ItemType.Null)
                    {
                        inventory.lastUseWaepon = 1;
                        equipController.WeaponUse(point);
                    }
                    else if (!nowPunching)
                    {
                        anim.SetTrigger("doPunch");
                        StartCoroutine(Punch());
                    }
                }
                if (Input.GetMouseButtonUp(1) && !isPushLeftMouse
                        && !equipController.nowHoldThrow)
                {
                    if (inventory.equipments[1].itemType != ItemType.Null && !isPushLeftMouse
                        && !equipController.nowHoldThrow)
                        equipController.WeaponUse(point);
                    StartCoroutine(StopAttackWait(0.4f));
                    isPushRightMouse = false;

                }//?????????? ???? ???? ?????? ????????.
                




                if (Input.GetKeyDown(KeyCode.F))
                {
                    equipController.CanThrowsUse();
                   
                }//f?? ?????? ?????????? on off????.

                if (Input.GetKeyDown(KeyCode.G))
                {
                    equipController.throwsChoice += 1;
                    if (equipController.throwsChoice >= equipController.throws.Length)
                        equipController.throwsChoice = 0;

                    if (equipController.nowHoldThrow)
                    {
                        equipController.nowHoldThrow = false;
                        equipController.CanThrowsUse();
                    }
                    inven.onChangeItem();
                }//???? ?????? ?????? ??????





                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (sheild.sheild < sheild.maxSheild)
                    {
                        if (inven.CanBeUse("SheildPotion"))
                            StartCoroutine(HealingTime(2));
                    }
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (health < maxHealth)
                    {
                        if (inven.CanBeUse("SmallPotion"))
                            StartCoroutine(HealingTime(1));
                    }
                }//1???? 2???? ?????? ?????? ???????? ??????????.
            }
            


            if (isPickup && Input.GetKeyDown(KeyCode.E))
            {
                if (fieldItem != null)
                {
                    inventory.PickUp(fieldItem);
                    fieldItem = null;
                    isPickup = false;
                }
            }//???????? ???? ?? ???? ?? e?? ?????? ??????.

            
            if (Input.GetMouseButton(0) && nowHealing)
            {
                nowHealing = false;
            }//???? ?????? ?????? ?????????? ???? ????????.
        }//???????????? ???????????? ???????? ??????.


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventory.isDrag)
            {
                inven.nowOpneInven = !inven.nowOpneInven;
                activeInventory = !activeInventory;
            }
        }//?????????? ?????? ???? ???? ???? ???? ?? ?? ????.

        if (down)
        {
            nowHealing = false;
            moveSpeed = downMoveSpeed;
        }
    }


    IEnumerator Dash()
    {
        isAttack = true;
        stamina -= 5;
        isStaminaMax = false;
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            controller.Move(moveVelocity * dashSpeed);
            yield return null;
            currentState = State.Idle;
        }
        StartCoroutine(StopAttackWait(0.4f));
    }
    IEnumerator DashCool()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashCoolTime)
        {
            dashSkillcool.CoolTimeUpdate((startTime + dashCoolTime - Time.time), (dashCoolTime));
            yield return null;
        }
        dashSkillcool.CoolTimeEnd();
        isdash = false;
    }//???? ???????? ???????? ???? ?? ???????? ?????????? ???? ???????????? ??????????????. ???? ?????????? ?????? ?? ?? ????.
    IEnumerator BasicSkillCool()
    {
        float startTime = Time.time;
        while (Time.time < startTime + basicCoolTime)
        {
            basicSkillcool.CoolTimeUpdate((startTime + basicCoolTime - Time.time), (basicCoolTime));
            yield return null;
        }
        basicSkillcool.CoolTimeEnd();
    }
    IEnumerator UltimitSkillCool()
    {
        float startTime = Time.time;
        float endTime = startTime + ultimitCoolTime;
        while (Time.time < endTime)
        {
            if (drinkChargingPotion)
            {
                endTime -= ultimitCoolTime / 3;
                drinkChargingPotion = false;
            }
            ultimitSkillcool.CoolTimeUpdate((endTime - Time.time), (ultimitCoolTime));
            yield return null;
        }
        ultimitSkillcool.CoolTimeEnd();

    }


        IEnumerator HealingTime(int keypadNumber)
    {//?????????? ?????????? ????????.
        float startTime = Time.time;
        nowHealing = true;
        moveSpeed = moveSpeed / 3;
        while (Time.time < startTime + 20f)
        {
            if (!nowHealing)
            {
                moveSpeed = moveSpeed * 3;
                break;
            }
            if (Time.time > startTime + 5)
            {
                if (keypadNumber == 1)
                    inventory.FastUse("SmallPotion");
                if (keypadNumber == 2)
                    inventory.FastUse("SheildPotion");
                moveSpeed = moveSpeed * 3;
                nowHealing = false;
                Debug.Log("good");
                break;
            }
            Debug.Log(Time.time);
            if(!nowHealing)
                Debug.Log("false");
            yield return null;
        }
        Debug.Log("break");
        yield return null;
        
    }//???? ?????? ???????? ?????????? ????????, ?????? ?????? ????????, ?????????? ?????????? break????.

    IEnumerator Exhausted()
    {
        float tempSpeed = moveSpeed;
        moveSpeed = 3;
        yield return new WaitForSeconds(4f);
        moveSpeed = tempSpeed;
        isExhausted = false;
    }//?????????? ???? ???????? ???? ???????? ????????.
    IEnumerator StopAttackWait(float time)
    {
        yield return new WaitForSeconds(time);
        isAttack = false;
    }
    IEnumerator StaminaInfinity()
    {
        yield return new WaitForSeconds(7);
        staminaInfinity = false;
    }
    IEnumerator Punch()
    {
        stamina -= 10;
        isStaminaMax = false;
        nowPunching = true;
        yield return new WaitForSeconds(0.1f);
        melee.GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(0.4f);
        melee.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        nowPunching = false;
    }
        public void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("FieldItem"))
        {

            initialCollisions = Physics.OverlapSphere(transform.position, 2f, collisionMask);
            isPickup = true;
            if (initialCollisions.Length > 0)
            {
                fieldItem = controller.ItemDstance(point, initialCollisions);
                if (fieldItem != beforeItem && beforeItem != null)
                {
                    beforeItem.transform.localScale = new Vector3(1, 1, 1);
                }
                beforeItem = fieldItem;
                if (fieldItem != null)
                    fieldItem.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            }//?????? fieldItem ?????????? ???? ?????? ???? ?? ???? 1????????(???????? ???????? ??????????)
            
        }        
    }
    public void OnTriggerExit(Collider collider)
    {
        if (fieldItem != null)
            fieldItem.transform.localScale = new Vector3(1, 1, 1);
        initialCollisions = null;
        fieldItem = null;
        isPickup = false;
    }//???????? ???????? ?????? ???? ???? ?????? ?????? return????.
    //???? ?????? ???????? ???????? ???????? ???????? ???? ????????.
}
