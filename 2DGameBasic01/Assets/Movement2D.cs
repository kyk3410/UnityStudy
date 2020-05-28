using UnityEngine;

public class Movement2D : MonoBehaviour
{
    private float moveSpeed = 5.0f;                 // 이동 속도
    private Vector3 moveDirection = Vector3.zero;   // 이동 방향

    private void Update()
    {
        // Negative left, a : -1
        // Positive right, d : 1
        // Non : 0
        float x = Input.GetAxisRaw("Horizontal");  // 좌우 이동
        // Negative down, s : -1
        // Positive up, w : 1
        // Non : 0
        float y = Input.GetAxisRaw("Vertical");     // 위아래 이동

        // 이동 방향 설정
        moveDirection = new Vector3(x, y, 0);

        // 새로운 위치 = 현재 위치 + (방향 * 속도)
        //transform.position = transform.position + new Vector3(1, 0, 0) * 1;
        //transform.position += Vector3.right * 1 * Time.deltaTime;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }


}
