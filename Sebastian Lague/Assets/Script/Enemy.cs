using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))] // NavMeshAgent를 불러주어 Enemy 스크립트를 할당하는 오브젝트는 전부 생긴다
public class Enemy : LivingEntity
{
    // pathfinder.SetDestination 메소드가 공격 Attack()메소드 도중에 실행되면 pathfinder가 false가 되었기 때문에 에러를 리턴할 것이다
    // 그래서 해야할일은, 현재 적의 상태를 저장하고 이를 통해 만약 공격 중인 상태라면 경로를 설정하는 작업을 안하는 것이다
    // 하여 enum 을 선언해준다, 대문자 S를 사용한다. Idle은 아무것도 하지 않는 상태, Chasing은 플레이어를 추격하는 상태, 그리고 Attacking은 공격하는 도중임을 나타낸다.
    public enum State {Idle, Chasing, Attacking};
    // 그리고 현재 상태를 나타내는 State currentState를 선언한다.
    State currentState;

    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder; // 길찾기를 관리 할것이다
    Transform target; // targer은 플레이어이다
    Material skinMaterial; // 적의 색깔을 가져온다.
    LivingEntity targetEntity; // target이 죽었는지 감지하고 추격을 멈추도록 하기위해 

    Color originalColor; // 공격이 끝난후에 원래 색으로 돌아와야되니

    float attackDistanceThreshold = 0.5f; //(공격 거리 임계값, 공격할 수 있는 한계 거리)
    float timeBetweenAttacks = 1f;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius; // 적의 자신의 반지름
    float targetCollisionRadius; // 목표의 충돌범위를 위한

    bool hasTarget;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>(); // 할당!


        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform; // Player라는 태그를 가진 오브젝트를 target에 할당한다.
            targetEntity = target.GetComponent<LivingEntity>(); // 컴포넌트 가져오기

            myCollisionRadius = GetComponent<CapsuleCollider>().radius; // 자신의 캡슐 콜라이더 컴포넌트를 가져와 radius값을 할당
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius; // targetCollisionRadius도 같은 것을 목표에서 가져오니, 위와 같이 할당한다.
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
            // Start 메소드에서 currentState = State.Chasing 으로 기본 상태를 지정해준다. 기본 상태는 추격이다
            currentState = State.Chasing;
            //hasTarget = true; 목표가 존재함

            /*target = GameObject.FindGameObjectWithTag("Player").transform; // Player라는 태그를 가진 오브젝트를 target에 할당한다.
            targetEntity = target.GetComponent<LivingEntity>();*/
            // LivingEntity의 OnDeath 이벤트가 구독하게 한다 
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

        skinMaterial = GetComponent<Renderer>().material; // material 할당
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
        // 목표가 죽는 순간에 false를 해 쫒을 목표가 없음을 지정한다
        hasTarget = false;
        // 적이 할일이 없어졌으므로, currentState = State.Idle로 지정한다.
        currentState = State.Idle;
    }

    void Update()
    {
        // 1. pathfinder.SetDestination(target.position); target.position을 넣어 목적지를 플레이어의 위치로 지정해 준다.
        // Update 안에 SetDestinantion을 호출하는 것은 매 프레임 마다 새롭게 경로를 계산한다는 거다,적과 장애물이 많을 경우 매우 많은 처리를 요구하게 된다.
        // 하여 매 프레임마다 업데이트를 하지 말고, 타이머로 고정된 간격으로 하게 하자.
        // -> IEnumerator UpdatePath()를 만들어준다 
        // 1은 IEnumerator UpdatePath()로 이동해준다.
        // hasTarget이 true일때만 공격하게 한다.
        if (hasTarget)
        {
            // 참으로 공격 가능 시간 이후일 경우, 이제 거리를 체크할 수 있다
            if(Time.time > nextAttackTime)
            {
                // (target.position - transform.position).sqrMagnitude로 목표의 위치와 자신의 위치의 차에 제곱을 한 수를 가져온다, 목표까지 거리의 제곱
                // 여기서 적의 중심과 플레이어의 중심으로 부터 잰 거리의 제곱을 가져왔는데, 
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                // 그리고 목표까지 거리의 제곱인 sqrDstToTarget 이 attackDistanceThreshold 제곱 보다 작은지 비교
                //if(sqrDstToTarget < Mathf.Pow(attackDistanceThreshold,2))
                // 우리는 공격 한계거리를 두 콜라이더의 표면으로 부터 젤것인데 그것을 콜라이더의 중심에서 유추하려면, 거기에 두 콜라이더의 반지름을 더해야 한다
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold+myCollisionRadius+targetCollisionRadius,2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks; // 다음 공격 가능시간 현재 시간에 공격 간격을 더한 값으로 지정
                    StartCoroutine(Attack()); // 코루틴 호출
                }
            }
        }
    }

    // 공격을 위한 코루틴 작성
    // 목표가 죽었을때를 처리한후에 플레이어가 적의 공격 등으로 대미지를 입게하는 것을 구현한다.
    IEnumerator Attack()
    {
        // 공격하는 동안은 currentState = State.Attacking 으로 지정하면 UpdatePath가 실행되지 않는다.
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position; // 현재 위치
        Vector3 dirTotarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirTotarget * (myCollisionRadius); //공격 목표지점
        //Vector3 attackPosition = target.position;
        float attackSpeed = 3; // 공격 속도
        float percent = 0; // 찌르기 애니메이션이 얼마나 멀리 갈것인가

        skinMaterial.color = Color.red; // 공격할때 red로 지정
        // 적이 플레이어를 계속 경로계산으로 추적해 쫓아가는 것을 원치 않는다, 왜냐하면 우리가 만든 애니메이션을 방해하기때문이다. 하여 false로 해준다.
        bool hasAppliedDamage = false; // 대미지를 적용하는 도중인가를 선언하고 계속 추적, false로 초기화

        while(percent <= 1)
        {
            // 만약 percent가 절반보다 크거나 같으면서 동시에 hasAppliedDamage가 false로 아직 대미지를 입히지 않았다면
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                // true로 대미지를 가하는 중임을 표현
                hasAppliedDamage = true;
                // targetEntity의 TakeHit을 호출하여 플레이어가 적으로 부터 타격을 입게 하고 싶다, 문제는 TakeHit메소드는 우리가 지정해줄 수 있는 damage값도 요구하지만
                // 동시에 RaycastHit 값도 요구하는데, 아직 레이캐스팅을 하지 않기 때문에 값을 줄 수 없다. 하여 IDamageable로 간다.
                targetEntity.TakeDamage(damage);
            }
            // originalPosition에서 시작해서 attackPosition으로 이동하고 다시 originalPosition으로 돌아가기 때문에,
            // 값이 0에서 1로, 그리고 다시 1에서 0으로 돌아가야만 한다. 하여 대칭함수를 이용한다
            // y = 4(-x^2+x) 같은 함수
            percent += Time.deltaTime * attackSpeed;
            // 하여 이것을 interpolation(보간)값 이라 부를 건데, 보간은 알려진 점들의 위치를 참조하여, 집합의 일정 범위의 점들(선)을 새롭게 그리는 방법을 말한다.
            // 여기서는 원지점->공격지점으로 이동할때 참조할 위 그래프의 대칭 곡선을 만드는 참조점을 의미한다.
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; // <- 볼록 대칭함수를 보면percent가 절반일때라는걸 알 수 있다.
            // transform.position 값이 Vector3.Lerp로 originalPosition에서 출발하여(*Lerp메소드는 두 벡터 사이에 비례 값(0에서 1사이)으로 내분점 지점을 반환.
            // 그래서 보간값이 0일때, 우리는 원지점 originalPosition에 있고, 1일 때는 attackPosition에 있을 것이다, 그리고 다시 0이 됬을 때 원지점으로 돌아간다.
            // 적이 attackPosition에 도달했을 때, interpolation값이 1일때 플레이어가 대미지를 입게 하려한다.
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            // 코루틴이기 때문에, yield return null 을 사용한다. 이는 while 루프의 각 처리 사이에서 프레임을 스킵한다
            // * yield return null 지점에서 작업이 멈추고, Update 메소드의 작업이 완전 수행된 이후, 
            // 다음 프레임으로 넘어갔을 때 yield키워드 아래에 있는 코드나 다음번 루프가 실행된다는 말이다
            yield return null;
        }

        skinMaterial.color = originalColor; // 공격이 끝나면 원래 색으로 돌아온다.
        // 공격이 끝나면 currentState = State.Chasing 으로 추격상태로 다시 돌아간다,
        currentState = State.Chasing;
        pathfinder.enabled = true; // 공격이 끝났을 때 true라고 해준다.
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f; // Path를 업데이트할 때 사용할 갱신간격을 선언
        // target != null 보다 hasTarget으로 하여 목표가 존재하는 동안이라는 표현으로 바꾼다.
        while (hasTarget)
        {
            // if(currentState == State.Chasing) 인 경우에만 UpdatePath로 경로를 갱신한다.
            if (currentState == State.Chasing)
            {
                // 목표로의 방향 벡터를 가져온다. 대상의 위치에서 현재 위치를 뺀다음 정규화 하면 된다.
                Vector3 dirTotarget = (target.position - transform.position).normalized;
                // 경로를 갱신할때 targetPosition에 실제 목표의 위치를 할당하지 말고, 목표 위치에서 일종의 적과 목표 사이의 방향(방향벡터)에
                // 적과 목표의 충돌 범위의 반지름을 곱하여 뺀 값을 할당한다
                // target.position에 목표로의 방향벡터에 적과 자신의 충돌 반지름을 곱한 값인 dirTotarget * (myCollisionRadius + targetCollisionRadius)를 뺀다
                // 찌르기 모션을 주기 위해서 사이 공간을 넣어주기위해 충돌 범위 밖에다 공격 한계 거리인 attackDistanceThreshold의 절반 만큼 더한 거리에서 멈추게 한다.
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
