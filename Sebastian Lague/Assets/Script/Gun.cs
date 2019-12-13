using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode {Auto, Burst, Single};
    public FireMode firemode;

    // 발사체가 발사되는 위치를 알아야되며 그래야 발사체들을 인스턴스화 할 수 있다
    public Transform[] projectileSpawn;
    // 어떤 발사체를 쏠지도 알아야 하므로 아래와 같이 선언해준다
    public Projectile projectile;
    // 발사 간격
    public float msBetweenShots = 100f;
    // 총알이 발사되는 순간의 총알 속력
    public float muzzleVelocity = 35f;
    public int burstCount;

    public Transform shell;
    public Transform shellEjection;
    MuzzleFlash muzzleflash;
    float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;

    void Start()
    {
        muzzleflash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
    }

    void Shoot()
    {
        // 할당한 연사력만큼 간격을 두어야 되기때문에 nextShotTime 변수를 만들고, 현재 시간이 nextShotTime보다 클때만 총을 쏘게 한다
        if (Time.time > nextShotTime)
        {
            if(firemode == FireMode.Burst)
            {
                if(shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (firemode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            for(int i = 0; i < projectileSpawn.Length; i++)
            {
            // 총을 쐈을 때 직후 nextShotTime에 현재시간(Time.time)과 연사간격을 더해준다, 밀리초를 초로 바꿔야 하니 1000으로 나눠 더해준다.
            nextShotTime = Time.time + msBetweenShots / 1000;
            // 총을 쏠때 새 발사체를 인스턴스화 생성해야 된다, 선언후 projectile을 할당해주고 위치값으로 muzzle의 위치와 회전값을 준다 projectile로 형변환해준다
         
            Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);

            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();

        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}

