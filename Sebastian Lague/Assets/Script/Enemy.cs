using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))] // NavMeshAgent를 불러주어 Enemy 스크립트를 할당하는 오브젝트는 전부 생긴다
public class Enemy : LivingEntity
{
    public enum State {Idle, Chasing, Attacking};
    State currentState;

    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder; // 길찾기를 관리 할것이다
    Transform target; // targer은 플레이어이다
    Material skinMaterial;
    LivingEntity targetEntity;

    Color originalColor;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1f;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>(); // 할당!


        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform; // Player라는 태그를 가진 오브젝트를 target에 할당한다.
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        //Enemy 스크립트에는 public override void Start()로 만들어주는데 여기 있는 Start 메소드를 오버라이드를 한다
        // 하여 여기서 base.Start()로 베이스(부모)클래스의 Start메소드를 부른다.
        base.Start ();
        pathfinder = GetComponent<NavMeshAgent>(); // 할당!
        

        if(hasTarget)
        {
            currentState = State.Chasing;
            //hasTarget = true;

            /*target = GameObject.FindGameObjectWithTag("Player").transform; // Player라는 태그를 가진 오브젝트를 target에 할당한다.
            targetEntity = target.GetComponent<LivingEntity>();*/
            targetEntity.OnDeath += OnTargetDeath;

            /*myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;*/
            StartCoroutine(UpdatePath()); // 호출하게 해주면 refreshRate한 시간마다 계속 목적지를 갱신해준다.
        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer,float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;
        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;

        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if(damage >= health)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update()
    {
        // 1. pathfinder.SetDestination(target.position); target.position을 넣어 목적지를 플레이어의 위치로 지정해 준다.
        // Update 안에 SetDestinantion을 호출하는 것은 매 프레임 마다 새롭게 경로를 계산한다는 거다,적과 장애물이 많을 경우 매우 많은 처리를 요구하게 된다.
        // 하여 매 프레임마다 업데이트를 하지 말고, 타이머로 고정된 간격으로 하게 하자.
        // -> IEnumerator UpdatePath()를 만들어준다 
        // 1은 IEnumerator UpdatePath()로 이동해준다.
        if (hasTarget)
        {
            if(Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold+myCollisionRadius+targetCollisionRadius,2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirTotarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirTotarget * (myCollisionRadius);
        //Vector3 attackPosition = target.position;

        float attackSpeed = 3; 
        float percent = 0;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while(percent <= 1)
        {
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f; // Path를 업데이트할 때 사용할 갱신간격을 선언
        while (hasTarget)
        {
            if(currentState == State.Chasing)
            {
                Vector3 dirTotarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirTotarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                // targetPosition을 선언해서 새로운 벡터3를 할당하는데 이것이 바닥에 있도록 한다, 하여 x값에는 target.position.x가 들어가지만 y 값에는 0이 들어가고
                // z값에는 target.position.z가 들어간다
                //Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
                if (!dead)
                {
                    // target.position대친 targetPosition을 넣어준다
                    pathfinder.SetDestination(targetPosition); 
                    // UpdatePath 코루틴이 있기 때문에 오브젝트가 파괴된 이후에도 이 코루틴을 돌리면서 pathfinder의 SetDestination을 호출하며 도착지점을 계산하려 할것이다
                    // 하여 에러를 반환한다, 하여 호출하기전에 if(!dead)죽지 않았음을 조건으로 준다.
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    } 
    // UpdatePath 코루틴을 한번 호출하게 되면 while 루프를 실행하는데, 우리가 refreshRate를 정의한 대로 x초 마다 계속 루프를 반복한다
}
