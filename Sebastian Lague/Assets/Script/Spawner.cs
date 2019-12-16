using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;
    public Wave[] waves; 
    public Enemy enemy; // 프리팹에 Enemy를 할당할수 있게 해준다.

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave; // 현재 웨이브의 레퍼런스를 가져오도록 선언
    int currentWaveNumber; 

    int enemiesRemainingToSpawn; // 스폰에 얼마나 스폰할 적이 남아있는지를 알기위해서 만들어준다.
    int enemiesRemainingAlive; // 살아 있는 적의 수를 나타낼 정수
    float nextSpawnTime; // 다음 스폰시간

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave(); // 첫번째 웨이브가 실행할수있게 호출해준다.
    }

    //
    void Update()
    {
        if (!isDisabled)
        {
            if(Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }
            // enemiesRemainingToSpawn이 0보다 작고, 현재 시간이 다음 스폰시간보다 크면
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--; // 첫번째 적을 불러야 되므로 -- 해준다
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");    
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position +  Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath; // 적을 스폰할 때 마다, spawnedEnemy.OnDeath 에 OnEnemyDeath를 추가해준다
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    // OnDeath에 의해 호출된 OnEnemyDeath 메소드를 통해 알림을 받게 된다.
    void OnEnemyDeath()
    {
        enemiesRemainingAlive--; // 적일 죽을 때마다 1씩 감소해준다.
        // 살아남은 적이 없을 때 NextWave메소드로 다음 웨이브를 시작할수 있다.
        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave()
    {
        currentWaveNumber++; // 현재 웨이브 숫자인 currentWaveNumber를 증가 시키는 것으로 시작, 웨이브는 1로 시작한다
        // currentWaveNumber-1 이 웨이브 배열 길이인 waves.Lenght보다 작도록 해야 한다.
        // 그리고 나서 안전하게 다음 웨이브의 정보를 가져올 수 있다.
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1]; // currentWave에 waves[현재웨이브 -1]을 해준다 *현재웨이브 = currentWaveNumver

            enemiesRemainingToSpawn = currentWave.enemyCount; // 스폰해주어야될 적의 수를 
            enemiesRemainingAlive = enemiesRemainingToSpawn; // 아직 살아있는 적의 수와 스폰할 적의 수를 할당해 준다.

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }
    // 내부에 웨이브의 정보를 저장할 클래스를 만들어준다.
    [System.Serializable] // System.Serializable을 해줌으로 써 인스펙터에 보이게 해준다.
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }
}
/*  Spawner은 웨이브에 따라 이루어질것이다 
 * 
 * 
 */