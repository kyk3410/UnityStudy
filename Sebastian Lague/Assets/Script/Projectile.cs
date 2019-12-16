using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask; // LayerMask를 정의 해준다, 이것으로 어떤 오브젝트, 어떤 레이어가 발사체와 충돌할지 결정할 수 있다.
    public Color trailColor;
    float speed = 10f;
    float damage = 1f;

    float lifetime = 3;
    float skinWidth = 0.1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
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
        if(Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
           // 하여 만약 무언가와 충돌했다면 여기에 OnHitObject를 호출하고 hit변수를 사용한다.
            OnHitObject(hit.collider, hit.point);
        }
    } 

    // 충돌한 오브젝트 정보를 가져올 Raycast hit을 입력으로 한다.
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

 