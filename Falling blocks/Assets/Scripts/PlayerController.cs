using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 7;
    public event System.Action OnPlayerDeath;

    float screenHalfWidthInWorldUnits;
    void Start()
    {
        float halfPlayerWidth = transform.localScale.x / 2f;
        screenHalfWidthInWorldUnits = Camera.main.aspect * Camera.main.orthographicSize + halfPlayerWidth;
        // halfPlayerWidth를 - 하고 아래 첫번째 if문에 screenHalfWidthInWorldUnits을 -로 두번째 if를 +로 하면 화면을 벗어나지를 못한다
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float velocity = inputX * speed;
        transform.Translate(Vector2.right * velocity * Time.deltaTime);

        if(transform.position.x < -screenHalfWidthInWorldUnits)
        {
            transform.position = new Vector2(screenHalfWidthInWorldUnits, transform.position.y);
        }
        if(transform.position.x > screenHalfWidthInWorldUnits)
        {
            transform.position = new Vector2(-screenHalfWidthInWorldUnits, transform.position.y);
        }
    }

    void OnTriggerEnter2D(Collider2D triggerCollider)
    {
        if(triggerCollider.tag == "Falling Block")
        {
            if(OnPlayerDeath != null)
            {
                OnPlayerDeath();
            }
            //FindObjectOfType<GameOver>().OnGameOver();
            Destroy(gameObject);
        }
    }
}
