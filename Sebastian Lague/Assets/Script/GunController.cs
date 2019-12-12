using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun startingGun;
    Gun equippedGun;

    private void Start()
    {
        // 만약 처음 시작 무기를 할당해 줬었다면, 즉 startingGun이 Null이 아니라면
        if(startingGun != null)
        {
            // 하면 EquipGun을 호출할 수 있다.
            EquipGun(startingGun);  
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        // 새로운 총을 가져오고 싶을때, equipedGun에 총의 새로운 인스턴스를 생성해 넣을 수 있다.
        // 하여, Instantiate(gunToEquip)을 할당한다.
        if(equippedGun != null)
        {
            // 현재 장착중인 총이 이미 있는지 체크해야 되므로 이미 있으면, 새 총을 가져오기 전에 파괴해야 한다.
            // 하여 equippedGun이 null이 아니면 Destroy(equippedGun.gameObject)를 해준다.
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        // 이제 총기를 새로 인스턴스화 할때 위치 값으로 weaponHold.position와 weaponHold.rotation을 줄 수 있다.
        // 변수가 Gun타입이라 오브젝트를 Gun으로 형변환 해주어야 된다.
        // 총 오브젝트가 플레이어를 따라 움직이도록 weaponHold의 자식으로 넣어야 한다.
        equippedGun.transform.parent = weaponHold;
    }

    public void Shoot()
    {
        // 장착중인 무기를 먼저 체크해야한다
        if(equippedGun != null)
        {
            equippedGun.Shoot();
        }
    }
}
