using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    public float damage;
    public void OnTriggerEnter(Collider collider)
    {
        IDamageble damagebleObject = collider.GetComponent<IDamageble>();
        if (damagebleObject != null)
        {
            damagebleObject.TakeDamageEnemy(damage, GetComponentInParent<Player>().gameObject);
        }
    }
}
