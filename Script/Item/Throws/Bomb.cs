using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Throws
{

    public void Ignite()
    {
        StartCoroutine(Explosion());
    }

    public IEnumerator Explosion()
    {
        yield return new WaitForSeconds(StopWatch);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        mesh.gameObject.SetActive(false);        
        effect.gameObject.SetActive(true);
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, rangeRadius);
        if (initialCollisions.Length > 0)
        {
            for(int i = 0; i< initialCollisions.Length; i++)
            {
                if (initialCollisions[i].gameObject.layer== LayerMask.NameToLayer("Enemy") 
                    || initialCollisions[i].gameObject.layer == LayerMask.NameToLayer("Player")) {
                    Transform target = initialCollisions[i].transform;
                    Vector3 dirToTarget = (target.position - transform.position).normalized;
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obtacleMask))
                    {
                        Debug.Log("Boom!" + initialCollisions.Length);
                        OnHitObject(initialCollisions[i]);
                    }
                }
            }
        }
        yield return new WaitForSeconds(3.2f);
        GameObject.Destroy(gameObject);
    }//지정한 위치에 도달하면 범위 내 피격 대상들에게 대미지를 주고 사라진다.
}
