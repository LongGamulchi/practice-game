using UnityEngine;
using System.Collections;
//�߻�ü
public class Projectile : MonoBehaviour
{
    Vector3 whenShooterPosition;
    GameObject shooter;
    public LayerMask[] collisionMask;
    public float speed = 10;
    public float damage = 1;
    public float range = 15;
    public float speedCorrection = 0.1f;
    //�Ѿ��� ����ϰ� ���� ��ġ�� ���� ���¿��� ���� �̵��ϴ� �����ӿ� �Ѿ��� �� ���η� �������� ��쿡�� �浹�� Ȯ���� �� �������� �������� �ִ´�.

    private void Start()
    {
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask[0]);
        if (initialCollisions.Length > 0)
            OnHitObject(initialCollisions[0]);
        initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask[1]);
        if (initialCollisions.Length > 0)
            OnHitObject(initialCollisions[0]);
        //������ ������Ʈ�� ��� �迭�� ��� �� ���� 0����ũ��(�浹���̸�)
        
    }

    void Update()
    {

        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
        //�����̵�

        float myDistanceToShooter = Vector3.Distance(transform.position, whenShooterPosition);
        if (Mathf.Abs(myDistanceToShooter) >= range)
        {
            GameObject.Destroy(gameObject);
        }//�ִ� ��Ÿ��� �������.
    }
    public void Shooter(GameObject _shooter)
    {
        shooter = _shooter;
        whenShooterPosition = shooter.transform.position;//��Ÿ��� �÷��̾� ��ġ �̵��� ���� ��ȯ���� �ʵ��� �߻��� ��ġ�� ������
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
    }//�߻�ü �浹 ����

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
    }//�浹�� �۵�
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
    }//�����ִ� ���¿��� �۵�
}