using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))] // GunController가 있다는 가정하에 가져오는것이므로 할당해준다
public class Player : LivingEntity // <- LivingEntity에는 IDamageable과 MonoBehaviour를 이미 상속중이다
{
    public float moveSpeed = 5f;

    public Crosshairs crosshairs;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    protected override void Start()
    {
        base.Start();
        /*controller = GetComponent<PlayerController>();
        // WeaponController에 대한 레퍼런스를 가져온다
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;*/
    }

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        // WeaponController에 대한 레퍼런스를 가져온다
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }
    void Update()
    {
        // 이동을 입력 받는 곳
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        // 바라보는 방향
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);
            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);

            }
        }

        // 무기 조작 입력
        // 마우스 왼쪽을 뜻하는 0을 집어넣어서 마우스 왼쪽을 누르고 있는 상태인걸 체크
        if (Input.GetMouseButton(0))
        {
            // gunController의 Shoot을 호출할수있다.
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelase();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

    }
}
