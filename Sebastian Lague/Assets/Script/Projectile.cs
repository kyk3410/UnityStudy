using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask; // LayerMask를 정의 해준다, 이것으로 어떤 오브젝트, 어떤 레이어가 발사체와 충돌할지 결정할 수 있다.
    public Color trailColor;
    float speed = 10f;
    float damage = 1f;
    // 총알에 생명주기를 주어 시간이 지나면 없어지도록 만들어 준다.
    float lifetime = 3;
    float skinWidth = 0.1f;

    private void Start()
    {
        // lifetime이 지낫을때 오브젝트를 없어지도록 만들어준다.
        Destroy(gameObject, lifetime);
        // 콜라이더 내부에서 출발하고 있는지 체크, 이는 OverlapSphere메소드를 사용해 알 수 있다. 
        // Collider의 배열 initialCollisions을 선언하고 Physics.OverlapSphere메소드를 호출해 할당한다.
        // 이 메소드는 위치를 요구하기 때문에, 이 발사체의 위치를 입력한다. transform.position을 주고, 그리고 반지름을 요구하는데, 0.1정도의 작은 값을 준다.
        // 그리고 collisionMask를 입력한다, 이로서 우리는 발사체와 겹쳐있는 모든 충돌체들의 배열을 얻었다. 
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        // initialCollisions.Lenght가 0보다 클 때(총알이 생성됬을 때 어떤 충돌체 오브젝트와 이미 겹친(충돌한)상태일때)이는 뭐든지 하나라도 충돌된 상태를 뜻하는데
        // 충돌한 첫번째 녀석에게 데미지를 가하고, 발사체를 파괴하려 한다.
        if(initialCollisions.Length > 0)
        {
            // OnHitObject 메소드를 호출하고, 충돌한 오브젝트 중 첫번째 녀석을 주면 된다.
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor); 
    }

    // 무기마다 발사체가 서로 다른 속력을 가지게 하고 싶을 수도 있으므로
    public void SetSpeed(float newSpeed)
    {
        // newSpeed를 speed에 할당해준다.
        speed = newSpeed;
    }
    void Update()
    {
        // 2. 레이캐스트를 사용해준다 반드시 transform.Translate를 호출하기 전에 레이의 이동할 거리와 충돌에 대한 결과를 가져와야 한다
        float moveDistance = speed * Time.deltaTime;
        // 3. CheckCollisions 메소드를 호출해준다 
        CheckCollisions(moveDistance);
        // 1. 발사체에 우리가 원하는건 앞으로 날라가는 것 뿐이라
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        // Ray를 지정해주고 새로운 Ray를 할당하고, 시작 지점과 방향을 주어야 한다, 발사체의 위치와, 발사체의 정면 방향을 넣어준다.
        Ray ray = new Ray(transform.position, transform.forward);
        // 그리고 충돌 오브젝트에 대해서 반환한 정보도 가져와야 한다.
        RaycastHit hit;

        // 조건문에 Physics.Raycast()에 ray를 넣고 hit을 입력해 가져(out 키워드)온다, 거리는 moveDistance를 입력, 레이아웃 마스크로 collisionMask를 넣는다
        // QueryTriggerInteraction으로서 우리가 트리거 콜라이더들과 충돌할지 안할지 정할수 있다, 우리는 트리거 콜라이더와 충돌하고 싶으니 Collide로 지정한다.
        // 충돌을 감지할때 레이가 움직일 거리를 moveDistance + skinWidth로 지정한다.
        if(Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
           // 하여 만약 무언가와 충돌했다면 여기에 OnHitObject를 호출하고 hit변수를 사용한다.
            OnHitObject(hit.collider, hit.point);
        }
    } 

    // 충돌한 오브젝트 정보를 가져올 Raycast hit을 입력으로 한다.
    // 겹치는 발사체를 해결하기 위해 OnHitObject 메소드를 사용하려 하나 이는 RaycastHit을 요구하므로, 이 메소드를 대체할 다른 메소드를 만든다.
    // Collider를 입력받게 한다 OnHitObject와 동일한 일을 하므로 가져와 c의 컴포넌트를 가져오게 지정하고, TakeHit메소드 대신에 전에 만든 TakeDamage 메소드를 사용한다
    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        // IDamageable damageableObject 변수를 선언해주고 hit.collider를 호출해서 충돌한 오브젝트를 가져온다.
        // 그리고 GetComponent<IDamageable>()로 해당 오브제그의 컴포넌트를 가져와 할당한다.
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if(damageableObject != null)
        {
            // null 이 아닐때 damageableObject.TakeHit을 호출한다, damage라는 변수와 RaycastHit 변수를 입력한다
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        // 오브젝트에 충돌했을 때 발사체를 파괴한다.
        GameObject.Destroy(gameObject);
    }
}

/*Enemy 오브젝트에서 IsTrigger를 설정하지 않으면 오브젝트가 플레이어 오브젝트를 충돌하는 현상을 볼수가 있다 하여 설정을 해주면 된다.
하여 레이캐스트가 QueryTriggerInteraction.Collide를 쓰는 이유인데 이렇게 하면 트리거 콜라이더 와의 충돌도 가져올 수 있기 때문이다.
play해보면 총알이 여전히 오브젝트를 통과하는걸 볼수있는데 우리가 총알에 콜리전 마스크를 정의했는데 Nothing으로 지정되어있기 때문이다
Enemy 레이어를 Enemy로 지정하고, Bullet 으로 돌아가서 Enemy를 충돌하고 싶다고 지정해준다.
*/

/* Projectile의 Raycast충돌감지에 있는 한가지의 문제는 만약 발사체가 어떤 오브젝트의 내부에서 출발한다면 그 충돌들을 기록하지 않을 것이다.원래 레이케스트가 그렇게 동작을 한다.
   예를 들면 적이 근접한 상태에서 내가 총을 쏘게 되면 총알이 적의 몸 속에서 생성되어, 어떤 데미지도 가하지 못한체 그냥 뚫고 뒤로 날라갈것이다.
 */

/* 마지막으로 충돌 감지에서 개선할 점은 적과 총알이 있다고 모든것이 한 프레임에서 이루어 질때의 문제를 말하는 건데, 그 한 프레임의 순간에 적이 살짝 앞으로 다가오면서
 * 이때 레이캐스트가 다시한번 적 내부에서 시작되면서 적과 총알이 겹칠때 시작되면 충돌을 감지 할 수가 없다
 */
