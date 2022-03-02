using System.Collections;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State {Idle, Chasing, Attacking};
    State currentState;
    //ĳ���� ����
    
    NavMeshAgent pathfinder;//��ã�� �˰��� ���
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;
    Color originalColour;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;
    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;
    public float damage = 1;
    //���ݰ���

    bool hasTarget;

    public override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColour = skinMaterial.color;

        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = GetComponent<CapsuleCollider>().radius;
            StartCoroutine(UpdatePath());
        }//Ÿ���� �����Ҷ��� ����, ���� ���� �����Ѵ�.
        
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;//���ͺ񱳴� ȿ���� ��������.
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + targetCollisionRadius + myCollisionRadius, 2))
                {//�ݶ��̴��� ǥ���� �������� �Ÿ��� ������ �� �������� �߰���
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }//Ÿ�ٰ��� �Ÿ� ���� < ��Ÿ������̶�� �����Ѵ�.
            }
        }//player�� ������쿡�� �۵��ϵ��� �Ѵ�.
        

        IEnumerator Attack()
        {
            currentState = State.Attacking;
            pathfinder.enabled = false;

            Vector3 originalPosition = transform.position;
            Vector3 dirToTarget = (target.position - transform.position).normalized;//Ÿ�ٹ��⺤��
            Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius/3);

            float attackSpeed = 3;
            float percent = 0;
            skinMaterial.color = Color.black;
            bool hasAppliedDamage = false;

            while(percent <= 1)
            {
                if(percent >=0.5f && !hasAppliedDamage)
                {
                    hasAppliedDamage = true;
                    targetEntity.TakeDamagePlayer(damage);
                }

                percent += Time.deltaTime * attackSpeed;
                float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;//������ �������.
                transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation); //���� ���� 0~1~0���� �̵��Կ� ���� ��ġ����
                yield return 0;
            }
            skinMaterial.color = originalColour;
            currentState = State.Chasing;
            pathfinder.enabled = true;
        }//������ �����ߴ�. State�� Ȱ���� ������ɰ� ���ݱ���� �ߺ������� ���Ѵ�.
             
    }
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while (hasTarget)
        {
            if (currentState == State.Chasing) {
                Vector3 dirToTarget = (target.position - transform.position).normalized;//Ÿ�ٹ��⺤��
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                //Ÿ�ٹ��⺤�Ϳ� �� ��ü�� �������� �̿��� ���� �Ÿ����� ���߰� �Ͽ���.
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }//1�ʿ� 4�� �÷��̾� ��ġ�� �ν��Ѵ�.
    }
}
