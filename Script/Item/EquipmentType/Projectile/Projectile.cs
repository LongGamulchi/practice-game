using UnityEngine;
using System.Collections;
//발사체
public class Projectile : MonoBehaviour
{
    Vector3 whenShooterPosition;
    GameObject shooter;
    public LayerMask[] collisionMask;
    public float speed = 10;
    public float damage = 1;
    public float range = 15;
    public float speedCorrection = 0.1f;
    //총알이 출발하고 적이 겹치지 않은 상태에서 적이 이동하는 프레임에 총알이 적 내부로 겹쳐지는 경우에는 충돌을 확인할 수 없음으로 보정값을 넣는다.

    private void Start()
    {
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask[0]);
        if (initialCollisions.Length > 0)
            OnHitObject(initialCollisions[0]);
        initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask[1]);
        if (initialCollisions.Length > 0)
            OnHitObject(initialCollisions[0]);
        //겹쳐진 오브젝트의 모든 배열을 얻어 그 값이 0보다크면(충돌중이면)
        
    }

    void Update()
    {

        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
        //직선이동

        float myDistanceToShooter = Vector3.Distance(transform.position, whenShooterPosition);
        if (Mathf.Abs(myDistanceToShooter) >= range)
        {
            GameObject.Destroy(gameObject);
        }//최대 사거리를 정해줬다.
    }
    public void Shooter(GameObject _shooter)
    {
        shooter = _shooter;
        whenShooterPosition = shooter.transform.position;//사거리가 플레이어 위치 이동에 따라 변환되지 않도록 발사한 위치를 저장함
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance+speedCorrection, collisionMask[0], QueryTriggerInteraction.Collide)
            || Physics.Raycast(ray, out hit, moveDistance + speedCorrection, collisionMask[1], QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }//발사체 충돌 감지

    void OnHitObject(RaycastHit hit)
    {
        if (hit.collider.gameObject.layer == 6)
        {
            IDamageble damagebleObject = hit.collider.GetComponent<IDamageble>();
            if (damagebleObject != null)
            {
                damagebleObject.TakeHitEnemy(damage, hit, shooter);
            }
            GameObject.Destroy(gameObject);
        }
        else
            GameObject.Destroy(gameObject);
    }//충돌시 작동
    void OnHitObject(Collider c)
    {
        if (c.gameObject.layer == 6)
        {
            IDamageble damagebleObject = c.GetComponent<IDamageble>();
            if (damagebleObject != null)
            {
                damagebleObject.TakeDamageEnemy(damage, shooter);
            }
            GameObject.Destroy(gameObject);
        }
        else
            GameObject.Destroy(gameObject);
    }//겹쳐있는 상태에서 작동
}