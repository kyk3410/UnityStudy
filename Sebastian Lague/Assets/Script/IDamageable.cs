using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // 얼마나 데미지를 입을지 표현할 float damage를 입력으로 받고, 또한 RaycastHit을 입력받는다, 이것이 충돌 지점이 어디인지 등의 기타 정보를 제공해 줄것이다.
    // 실제로 구현 부분은 여기서 만들 필요가 없기때문에 ; 해준다.
    void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection);
    // TakeDamage를 만들어준다 TakeHit을 간단하게 만든 버전이다, damage는 요구하지만 RaycastHit입력은 요구하지 않을 것이다.
    void TakeDamage (float damage);
}

/* 오브젝트에 어떠한 스크립트에 붙었는지 일일이 체크하고 싶지가 않기때문에 발사체에 맞았다는 것을 스크립트 종류와 상관없이 통보할 수 있어야 된다
 * 하여 IDamageable(타격입을 수 있는)이라는 인터페이스를 만든다.
 * TakeHit 이라는 메소드를 가지는데 발사체와 충돌했다는 것을 통보 받아야 하는 클래스들은 IDamageable 이라는 인터페이스를 구현(Implement)하면 된다.
 * (Implement - 세부구현이 없는 추상 껍데기를 상속 받아 내부 구현을 채우는 것)
 * 인터페이스를 삽입하면 해당 클래스는 TakeHit 메소드가 무조건 포함되며 이것을 구현해야 한다.
 * 장점은 발사체가 충돌했을 때 오브젝트에 어떤 스크립트가 붙어있는지 신경 쓸 필요가 없다는 것이다.
 * 인터페이스는 이름 앞에 대문자 I 를 붙여줘야 한다.
*/
