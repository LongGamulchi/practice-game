using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throws : MonoBehaviour
{
    public Rigidbody rigid;
    public GameObject mesh;
    GameObject throwedObj;
    public GameObject effect;
    public LayerMask obtacleMask;
    public int damage;
    public float rangeRadius;
    public float yForce;
    public float StopWatch;

    public void Throw(Vector3 targetPosition, GameObject _throwedObj)
    {
        throwedObj = _throwedObj;
        targetPosition.y += yForce;
        rigid.AddForce(targetPosition, ForceMode.Impulse);
        rigid.AddTorque(Vector3.back * 10, ForceMode.Impulse);
    }//지정한 위치까지의 벡터를 받아 해당 위치까지 포물선 운동을 하게 해준다.

    public void OnHitObject(Collider c)
    {
        IDamageble damagebleObject = c.GetComponent<IDamageble>();
        if (damagebleObject != null)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
                damagebleObject.TakeDamagePlayer(damage);
            else
                damagebleObject.TakeDamageEnemy(damage, throwedObj);
        }
    }//데미지 계산
}
