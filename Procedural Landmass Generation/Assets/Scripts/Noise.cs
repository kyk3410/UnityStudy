using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트를 장면의 오브젝트에 적용하지 않기 때문에 MonoBehaviour를 상속할 이유가 없다.
// 스크립트의 사본을 여러 개 만들지 않기를 원하기 때문에 static으로 만든다

public static  class Noise
{
// 1. 노이즈 맵을 생성하는 기능을 갖기를 원하기때문에 우리는 그 함수가 0과 1 사이의 숫자 그리드를 반환하기를 원한다
// 2. 함수는 lacunarity및 persistence를 포함하여 많은 논증을? 얻는다
// 7. NoiseMap에 scale인수를 추가해준다.
// 39. octaves와 persistance,lacunarity를 추가해준다
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight,int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        // 3. mapWidth와 mapHeight가 새로운 2차원 float 배열로 정의한다.
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i<octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // 10. scale이 0보다 작거나 같은 경우 최소 0.0001f로 제한한다
        if( scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        // 4. noiseMap을 반복한다
        for(int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // 41. amplitude(진폭)과 frequency(진동)을 만들고 현재 높이를 추적하기 위해 noiseHeight를 0으로 초기화해준다.
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;


                // 40. octaves의 루프를 만들어 준다
                for(int i = 0; i< octaves; i++)
                {
                // 5. 어디에서 높이 값을 얻는 지 결정하고 싶다.
                // 6. 이 값을 정수로 사용하고, 항상 동일한 값을 얻는다.
                // 8. scale을 나누어준다, 정수가 아니기 때문에 깔끔하게 된다.
                // 9. scale이 0과 같을 수 있으므로 0으로 나누기 때문에 주의해야된다.
                // 43. 주파수를 적용해주고 샘플 좌표를 곱해준다.
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x; 
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;
                // 11. Mathf.PerlinNoise(sampleX,sampleY)를 해줄수있다        
                // 45. 범위가 -1에서 1 사이이기때문엥 2를 곱한후 -1을 해준다.
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 -1;
                // 12. noiseMap에 적용하기만 하면 된다,
                //noiseMap[x, y] = perlinValue;
                // 42. perlinValue값과 amplitude값을 곱한 값을 현재 진폭에 더해준다.
                    noiseHeight += perlinValue * amplitude;

                    // 44. 진폭이 얻는 값을 지속성 값을 곱한 값
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if( noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                // 46. 좌표 x,y가있는 noiseMap은 noiseHeight값과 같다.
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); 
            }
        }
        // 13. noiseMap을 반환해준다.
         return noiseMap;
    }
}
