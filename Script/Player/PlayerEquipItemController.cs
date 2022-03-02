using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipItemController : MonoBehaviour
{

    public delegate void OnAttackCall();
    public OnAttackCall onAttackCall;
    public delegate void OnAttackCallWithMousePosition(Vector3 mousePosition);
    public OnAttackCallWithMousePosition onAttackCallWithMousePosition;
    Inventory inven;
    public Player player;

    public Transform weaponHolder;
    public Transform armorHolder;
    public Transform bagHolder;
    public Weapon[] weapons;
    public Armor[] armors;
    public Bag[] bags;
    public int index;
    public int totalDeal;
    public bool nowHoldThrow;
    public bool nowSwap;
    public string[] throws = { "Bomb", "Molotov" };
    public int throwsChoice = 0;


    public void Start()
    {
        player = GetComponent<Player>();
        weapons = weaponHolder.GetComponentsInChildren<Weapon>();
        armors = armorHolder.GetComponentsInChildren<Armor>();
        bags = bagHolder.GetComponentsInChildren<Bag>();
        inven = GetComponent<Inventory>();
        inven.onChangeEquip += SetEpuiqment;
        SetEpuiqment();
    }

    
    public void WeaponUse(Vector3 mousePosition)
    {

        if (!nowHoldThrow && !nowSwap)
        {
            if (inven.equipments[0].itemType != ItemType.Null
                && inven.equipments[1].itemType != ItemType.Null)
            {
                if (inven.lastUseWaepon == 1)
                    setWeapon(1);

                if (inven.lastUseWaepon == 0)
                    setWeapon(0);
            }//지금 사용할 무기로 교체한다.
            onAttackCall();
        }
        else
        {
            onAttackCallWithMousePosition(mousePosition);
        }
        player.isStaminaMax = false;
    }

    public bool CanThrowsUse()
    {
        bool isThrow = false;
        bool isFind = false;
        
        if (!nowHoldThrow)
        {
            for (int j = 0; j < weapons.Length; j++)
            {
                    weapons[j].transform.gameObject.SetActive(false);
            }
            if (inven.items != null)
            {
                foreach (Item throws in inven.items)
                {
                    if (throws.detailType == DetailType.Throws)
                    {
                        isThrow = true;
                        break;
                    }
                }//아이템중에 투척물이 있는지 찾은후에
                if (isThrow)
                {
                    for (int i = throwsChoice; i < throws.Length; i++)
                    {
                        foreach (Item findThrows in inven.items)
                        {
                            if (throws[i] == findThrows.itemName)
                            {
                                for (int j = 0; j < weapons.Length; j++)
                                {
                                    if (throws[i] == weapons[j].name)                                    
                                        StartCoroutine(SwapDelay(true, j));                                    
                                    else
                                        weapons[j].transform.gameObject.SetActive(false);                                    
                                }
                                throwsChoice = i;
                                isFind = true;
                                break;
                            }
                        }
                        if (isFind) break;
                        if (i == throws.Length - 1) i = -1;
                    }//투척물이 있다는건 확실했으니, 가지고있는 투척물을 순서대로 조회해서 가장 먼저 들리는 투척물을든다.
                }
                else {
                    StartCoroutine(SwapDelay(false, 0));
                }//투척물이 없다면 무기를 든다.
            }
        }
        else 
            StartCoroutine(SwapDelay(false, 0));
        //투척모드 onoff를 해준다.
        return isFind;
    }

    public void SetEpuiqment()
    {
        if (!nowHoldThrow)
        {
            //무기 입수,드랍 또는 스왑 후의 무기 상황이다.
            if (inven.equipments[0].itemType == ItemType.Null
                && inven.equipments[1].itemType == ItemType.Null)
            {//무기다 둘 다 없음
                setOffWaepon();
                inven.lastUseWaepon = 0;
            }
            else if (inven.equipments[0].itemType != ItemType.Null
                && inven.equipments[1].itemType == ItemType.Null)
            {//0에만 무기가 있음
                setWeapon(0);
                inven.lastUseWaepon = 0;
            }
            else if (inven.equipments[0].itemType == ItemType.Null
                && inven.equipments[1].itemType != ItemType.Null)
            {//1에만 무기가 있음
                setWeapon(1);
                inven.lastUseWaepon = 1;
            }
            else if (inven.equipments[0].itemType != ItemType.Null
                && inven.equipments[1].itemType != ItemType.Null)
            {//둘다 있음
                if (inven.lastUseWaepon == 1)
                    setWeapon(1);

                if (inven.lastUseWaepon == 0)
                    setWeapon(0);
            }//둘다 있는 경우 마지막에 쓴 무기 위치의 무기를 든다.
        }


        if (inven.equipments[2].itemType != ItemType.Null)
        {
            for(int i = 0; i < armors.Length; i++)
            {
                if (inven.equipments[2].itemName == armors[i].name)
                    armors[i].transform.gameObject.SetActive(true);
            }            
        }
        else
        {
            for (int i = 0; i < armors.Length; i++)
            {
                armors[i].transform.gameObject.SetActive(false);
            }
        }//실제 player에게 실드 장착


        if (inven.equipments[3].itemType != ItemType.Null)
        {
            for (int i = 0; i < bags.Length; i++)
            {
                if (inven.equipments[3].itemName == bags[i].name)
                {
                    bags[i].transform.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < bags.Length; i++)
            {
                bags[i].transform.gameObject.SetActive(false);
            }
        }//실제 player에게 가방 장착
    }

    public void setWeapon(int waponNum)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (inven.equipments[waponNum].itemName == weapons[i].name)
                weapons[i].transform.gameObject.SetActive(true);
            else
                weapons[i].transform.gameObject.SetActive(false);
        }
    }

    public void setOffWaepon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].transform.gameObject.SetActive(false);
        }
    }

    IEnumerator SwapDelay(bool isTake, int j)
    {
        nowSwap = true;
        yield return new WaitForSeconds(1);
        if (isTake)
        {
            weapons[j].transform.gameObject.SetActive(true);
            nowHoldThrow = true;
        }
        else
        {
            nowHoldThrow = false;
            SetEpuiqment();
        }
        nowSwap = false;
    }//투척물을 들 때, 넣을떄, 다 떨어졌을때 딜레이를 넣었다.

}
