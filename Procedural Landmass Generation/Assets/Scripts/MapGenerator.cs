 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColourMap, Mesh};
    public DrawMode drawMode;

    const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    public float noiseScale;

    public int octaves;
    // 59. 범위를 0~1사이로 지정해준다.
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultipler;
    public AnimationCurve meshHeightCurve;

    // 36. 자동으로 업데이트 해주기위해 bool을 선언해준다.
    public bool autoUpdate;
    // 배열로 선언해준다.
    public TerrainType[] regions;

    public void DrawMaInEditor()
    {
        MapData mapData = GenerateMapData();
        // 31. MapDisplay를 호출
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultipler, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
    }

    MapData GenerateMapData()
    {
        // 14. Noise 클래스의 2D 노이즈 맵을 검색한다
        // 49. Noise클래스에서 추가해준 octaves,persistance,lacunarity를 추가해준다.
        // 54. seed와 offset을 추가해준다.
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity,offset);
        // 15. 이 함수의 뒷부분에는 noiseMap을 처리하고 지형으로 바꾸는 모든 방법이 있다. 먼저 MapDisplay 클래스에만 전달 하고 있다.
        // 화면에 노이즈 맵 그리기를 시작할 수 있다.
        // 62. 컬러 맵을 만들려면 값을 동일하게 설정한다, 크기가 새로운 색상 배열로 지도 너비 x 지도 높이
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        // 61. 맵을 받는 루프를 만들어준다
        for(int y = 0; y<mapChunkSize; y++)
        {
            for(int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for(int i =0; i< regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);
      
    }
    // 58. 이 함수는 스크립트가로드되거나 Inspector에서 값이 변경 될 때 호출됩니다, 이하로 줄지않게 된다.
    void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }
}

// 60. 지형의 이름 높이 색깔을 public으로 선언해주고 System.Serializable을 해준다.
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public float[,] heightMap;
    public Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}
