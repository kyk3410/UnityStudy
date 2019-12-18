using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab; // 인스턴스화할 타일
    public Transform obstaclePrefab; // 장애물 프리팹
    public Transform mapFloor;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Vector2 maxMapSize; // 맵의 크기

    [Range(0, 1)]
    public float outlinePercent; // 테두리 퍼센트 [Range(0,1)]은 0에서 1 사이 범위로 한정 한다는 뜻이다

    public float tileSize;
    List<Coord> allTileCooards; // 모든 타일 좌표에 대한 리스트로 List<Coord> allTileCooards를 선언한다.
    Queue<Coord> shuffledTileCoords; // 셔플된 좌표들을 저장할 변수
    Queue<Coord> shuffledOpenTileCoords;
    Transform[,] tileMap;

    Map currentMap;

    void Start()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }
    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);
        //GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, .05f, currentMap.mapSize.y * tileSize);

        // GenerateMap에 allTileCooards에 new List<Coord>()를 할당하고, 그리고 모든 타일을 거쳐 루프하고 싶으니 맵 사이즈 루프를 가져온다.
        // Generating coords
        allTileCooards = new List<Coord>();

        for (int x = 0; x < currentMap.mapSize.x; x ++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y ++)
            {
                // allTileCooards.Add를 호출해와 x와 y값을 받은 new Coord(x,y)를 추가해준다.
                allTileCooards.Add(new Coord(x, y));
            }
        }
        // 새 좌표 큐를 할당하는데,이것은 IEnumerable<Coords>collection을 입력 받을 수 있기 때문에 Utility.ShuffleArray(~)를 할당한다,
        // 그리고 저 안에는 좌표 리스트인 allTileCoords 를 ToArray를 호출해 배열로 입력해준다. ShuffleArray가 배열을 받기 때문이다.
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCooards.ToArray(), currentMap.seed));

        // Create map holder object
        string holderName = "Generated Map"; // Generated Map 밑에 자식으로 가지고 있을 오브젝트를 생성해야 된다.
        if(transform.Find(holderName))
        {
            // holderName이란 오브젝트 아래에 자식들이 존재한다면 그것들을 파괴한다.
            // DestroyImmediate는 게임 오브젝트만 파괴가능하다, Destroy대신에 DestroyImmediate를 사용하는 이유는 에디터에서 호출한 것이라,DestroyImmediate를 사용한다.
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        // 타일들이 존재한 경우 파괴한 다음에, 여기에 새로운 맵 홀더를 만들어 준다.
        Transform mapHolder = new GameObject(holderName).transform;
        //
        mapHolder.parent = transform;

        // Spawning tiles
        /// 맵 사이즈 루프
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++){
                // 타일을 World에 소환할 위치를 계산, x에 -mapSize.x/2를 할당한다 
                // * -mapSize.x/2를 하면 x좌표 0을 중심으로 맵의 가로 길이의 절반 만큼 왼쪽으로 이동한 점에서 부터 타일 생성을 시작한다.
                // 0.5를 옴기면 타일의 중심이 아닌 모서리가 위치 하게된다. 그리고 루프마다 이동해 넘어갈 x값을 넣어준다.
                //Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2  + 0.5f + y);
                // tilePosition에 CoordToPosition(x,y)를 할당해준다.
                Vector3 tilePosition = CoordToPosition(x, y);
                // 위치는 tilePosition 게임 월드에서 바닥에 눕혀 놓기 위해 90도 회전 Quaternion 메소드 호출하는데 오일러 각을 기준으로 회전시킨다
                // * Euler angle - 이전 좌표축에서 이후 좌표축이 얼마나 회전했는지 기준으로 회전을 표현하는 좌표계, x축을 나타내는 Vector3.right * 90도, Transform으로 형변환
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize; // Vector3.one * (1 - outlinePercent)로 테두리 영역만큼 타일의 크기를 줄여 할당해 준다.
                // 새 타일을 생성할때 마다, newTile.parent = mapHolder로 새타일의 부모를 맵 홀더로 해준다.
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }


        // Spawning obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCooards);

        for (int i=0; i< obstacleCount; i++)
        {
            // 여기서 타일을 생성한 이후에, 생성할 장애물들 수를 특정해보자 obstacleCount가 10이라고 했을때 for루프를 해주고 Coord randomCoord = GetRandomCoord();해준다
            // 이제 이 좌표 Coord를 게임 월드 상의 실제 Vector3로 변환해보자
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if(randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount)){
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight,(float)prng.NextDouble());
                // tilePosition과 동일하게 CoordToPosition을 할당해준다
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                // Instantiate(obstaclePrefab,으로 프리팹을 인스턴스화 하여 할당해주는데 위치로는 당연이 obstaclePosition을 주고, 회전은 필요없으니 Quaternion.identity를 할당 그리고 Transform으로 형변환
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1-outlinePercent) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));

        // Creating navmesh mask
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab,Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab,Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab,Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;
        
        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap,int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCentre);
        mapFlags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for(int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if(x == 0 || y == 0)
                    {
                        if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if(!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }
    // 메소드로 만들어 쓴다
    Vector3 CoordToPosition (int x, int y)
    {
        // tilePosition에 있던 변환식으로 반환한다
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        int x =  Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y =  Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) -1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) -1);
        return tileMap[x, y];
    }
    // 이 메소드는 그저 큐로 부터 다음 아이템을 얻어 랜덤 좌표를 반환해준다.
    public Coord GetRandomCoord()
    {
        // 셔플된 타일 좌표 큐의 첫 아이템을 가지도록, shuffleTileCoords.Dequeue()를 호출해서 할당한다, 
        // 그리고 그렇게 얻은 랜덤 좌표 randomCoord를 큐의 마지막으로 다시 되돌려 놓고 싶은데
        Coord randomCoord = shuffledTileCoords.Dequeue();
        // 이때 shuffledTileCoords.Enqueue(randomCoord)를 해준다.
        shuffledTileCoords.Enqueue(randomCoord);
        // 그리고 randomCoord를 반환해준다.
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;
        // 생성자
        // 리스트에 맵의 모든 타일을 표현한 좌표 Coord를 리스트에 저장하고 싶다.
        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1==c2);
        }
    }

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCentre
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }

    }
}

/* Editor폴더를 만들어 주는데 단순이 체계적으로 정리하려고 그러는게 아닐,. 에디터 스크립트들은 반드시 Editor 폴더 아래 있어야 한다.
 
 */