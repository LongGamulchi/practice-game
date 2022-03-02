using System.Collections;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State {Idle, Chasing, Attacking};
    State currentState;
    //캐릭터 상태
    
    NavMeshAgent pathfinder;//길찾기 알고리즘 사용
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
    //공격관련

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
        }//타겟이 존재할때만 공격, 추적 등을 시작한다.
        
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
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;//벡터비교는 효율이 떨어진다.
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + targetCollisionRadius + myCollisionRadius, 2))
                {//콜라이더의 표면을 기준으로 거리를 제도록 각 반지름을 추가함
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }//타겟과의 거리 제곱 < 사거리제곱이라면 공격한다.
            }
        }//player가 있을경우에만 작동하도록 한다.
        

        IEnumerator Attack()
        {
            currentState = State.Attacking;
            pathfinder.enabled = false;

            Vector3 originalPosition = transform.position;
            Vector3 dirToTarget = (target.position - transform.position).normalized;//타겟방향벡터
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
                float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;//보간을 만들었다.
                transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation); //보간 값이 0~1~0으로 이동함에 따라 위치변경
                yield return 0;
            }
            skinMaterial.color = originalColour;
            currentState = State.Chasing;
            pathfinder.enabled = true;
        }//공격을 구현했다. State를 활용해 추적기능과 공격기능의 중복실행을 막앗다.
             
    }
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while (hasTarget)
        {
            if (currentState == State.Chasing) {
                Vector3 dirToTarget = (target.position - transform.position).normalized;//타겟방향벡터
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                //타겟방향벡터와 각 개체의 반지름을 이용해 적당 거리에서 멈추게 하였다.
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }//1초에 4번 플레이어 위치를 인식한다.
    }
}
