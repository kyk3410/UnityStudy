using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidbody;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Vector3 velocity를 입력으로 받는다.
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    public void LookAt (Vector3 lookPoint)
    {
        Vector3 heigthCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);// y 값은 플레이어 자신의 높이 값을 넣어주어야 한다.
        transform.LookAt(heigthCorrectedPoint); // 플레이어가 마우스 커서를 향해 기울여서 바라볼거시다, y값이 유지하기위해서 위와 같이 해준다, 
        //후에 lookPoint대신 heightCorrectedPoint를 바라게한다
    }
    void FixedUpdate() // public으로 해줄필요 없고 privte으로 해준다.
    {
        // FixedUpdate를 사용해야 하는 이유는, 이 부분은 정기적이고 짧게 반복적으로 실행되야 하기 때문이다.
        // 이것을 쓰면 프레임 저하가 발생해도 프레임에 시가의 가중치를 곱해 실행되어 이동속도를 유지해준다.
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime); 
        // AddForce메소드를 사용하지 않고 간단히 MovePosition 메소드로 위치를 옮긴다
        // myRigidbody.position은 현재 위치 velocity는 이동할 속력이다.
        // fixedDeltaTime은 두 FixedUpdate 메소드가 호출된 시간 간격을 뜻한다.
    }
}
