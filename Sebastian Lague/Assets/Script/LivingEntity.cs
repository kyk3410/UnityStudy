using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IDamageable을 상속한다
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth; // health값을 직접 지정하지 않고, 시작체력을 만든다.
    protected float health; // 체력변수 proteceted를 해줌으로써 상속 관계 없는 클래스에서 사용할 수 없고, 인스펙터에서 볼 수 없다
    // Player나 Enemy 스크립트는 사용가능하다
    protected bool dead; // true, false를 나타내기 위해 bool로 해준다.

    // LivingEntity가 Enemy클래스의 Spawner까지 짜는건 난잡하고 죽는 처리를 할 때 Die 메소드안에 Spawner래퍼런스를 찾아 계속 확인하는건 좋지가 않다
    // Spawner가 event를 구독하게 하여 적이 죽었을때 알림을 받게 한다.
    public event System.Action OnDeath; // System.Action을 선언하는데, 이것은 델리케이트 메소드로써
    // * 델리케이트 - 다른 메소드의 위치를 가르키고 불러올 수 있는 타입. c++에서의 함수 포인터와 유사한 역할

    protected virtual void Start()
    {
        health = startingHealth; // 체력을 할당해준다. -> Player와 Enemy에 MonoBehavior 대신 LivingEntity을 상속한다
        // public 대신 protected로 사용한다
    }
    // IDamageable을 상속 받았기 때문에 강제로 구현해주어야된다
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        // health -= damage;를 해주고 health가 0보다 작거나 같으면 Die메소드를 호출해준다
        // 또 아직 죽지 않았을때라는 조건을 붙여준다 && !dead
        // Do some stuff here with hit var 나중에 hit변수와 함께 어떤 처리들을 여기서 할것이다. 예를 들면 나중에 RaycastHit변수를 사용해서 발사체가 적을 맞춘 지점을 감지할 수 있고,
        // 그리고 파티클 오브젝트를 그 지점에서 생성할 수 있다.
        TakeDamage(damage);
    }

    // IDamageable에서 TakeDamage를 만들어주었기 때문에 LivingEntity에도 만들어주어야 된다
    public virtual void TakeDamage(float damage)
    {
        // TakeHit 코드를 가져오고 TakeHit메소드는 단순이 TakeDamage를 damage를 넣어 재사용하면 된다.
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }

    }

    [ContextMenu("Self Destruct")]
    protected void Die() // public 대신 protected 로 해준다
    {
        dead = true; // 죽었으면 dead에 true를 할당
        if (OnDeath != null) // OnDeath 이벤트가 null이 아니라면
        {
            OnDeath(); // OnDeath를 일반적인 함수처럼 호출 할수가 있다
        }
        GameObject.Destroy(gameObject);// 게임오브젝트를 파괴해준다.
    }
}

/* 플레이어와 적 모두 공통점을 가지고 있다 양쪽다 데미지를 입고 HP가 존재하고 죽을 수도 있다 하여 그런기능을 각각 구현하는것보다
 * 공유하는 클래스를 만들어 양쪽다 상속받아 쓸 LivingEntity 라는 클래스를 만든다.
 * IDamageable을 구현할수 있는데, 우선 LivingEntity가 MonoBehavior을 상속(extends)하는데
 * Player와 Enemy에 LivingEntity를 상속해주었는데 이때 주의해야될 점은 중복된 메소드가 있는 경우이다,
 * 상속받은 스크립트의 Start와 LivingEntity의 Start메소드는 완전이 덮어 쓰게 될것이다 하여 상속받은 스크립트의 코드가 절대 실행되지 못한다.
 * 하여 virtual 키워드를 더해서 public virtual void Start()로 만들어준다.
 * Player와 Enemy에도 protected로 해준다
*/