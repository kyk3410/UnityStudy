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
    public int projectilesPerMag;
    public float reloadTime = .3f;


    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f,.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3,5);
    public float recoilMoveSettleSpeedTime = .1f;
    public float recoilRotationSettleSpeedTime = .1f;

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjection;
    MuzzleFlash muzzleflash;
    float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;
    int projectilesRemainingInMag;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    void Start()
    {
        muzzleflash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }
    void LateUpdate()
    {
        // animate recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleSpeedTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity,  recoilRotationSettleSpeedTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.right * -recoilAngle;

        if(!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }
    void Shoot()
    {
        // 할당한 연사력만큼 간격을 두어야 되기때문에 nextShotTime 변수를 만들고, 현재 시간이 nextShotTime보다 클때만 총을 쏘게 한다
        if (!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0)
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
                if(projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                // 총을 쐈을 때 직후 nextShotTime에 현재시간(Time.time)과 연사간격을 더해준다, 밀리초를 초로 바꿔야 하니 1000으로 나눠 더해준다.
                nextShotTime = Time.time + msBetweenShots / 1000;
                // 총을 쏠때 새 발사체를 인스턴스화 생성해야 된다, 선언후 projectile을 할당해주고 위치값으로 muzzle의 위치와 회전값을 준다 projectile로 형변환해준다
         
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

        }
    }

    public void Reload()
    {
        if(!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
        }
    }

        float reloadSpeed = 0;
    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;
            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
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

