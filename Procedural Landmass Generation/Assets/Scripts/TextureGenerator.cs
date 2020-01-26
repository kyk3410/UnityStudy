using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        // 17. 먼저 우리가 받은 노이즈 맵의 너비와 높이가 무엇인지 알아내는 것이다.
        // 18. 우리 할수 있는 것은 int Width = noiseMap.GetLength()와 같다.
        // 19. 높이는 noiseMap.GetLength(1)이다.
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // 20. Texture2라고 말하며 2D텍스처를 만들 수 있다.
        //Texture2D texture = new Texture2D(width, height);

        // 23. 모든 픽셀에서 색상 배열을 먼저 생성하는 것이 더 빠르다.
        Color[] colourMap = new Color[width * height];
        // 24. noiseMap의 모든 값을 반복한다.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 25. y에 지도 너비를 곱하여 색을 얻을 수 있다 행 유형의 색을 제공한다, 열을 얻으려면 X 값을 추가한다
                // 26. 흑백 사이의 색과 동일하게 하고 싶을 때 시점에서 noiseMap의 값에 따라 Color.Lerp를 사용할수 있다, a는 black b는 white다 그다음 0과 1사이의 백분율을 제공한다. 노이즈 맵과 동일하다 noiseMap[x,y];
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        // 21. 모든 픽셀이 텍스처에서 얻을 수 있는 색상을 계속 수정하고 싶다.
        // 22. 이를 수행하는 한 가지 방법은 texture.SetPixel을 사용하여 x와 y마다 색상을 지정하는 것이다.
        // 27 텍스처에 이 색상을 적용하여  texture.SetPixels라고 말하고 colorMap을 제공하려고 한다. 
        //texture.SetPixels(colourMap);
        //texture.Apply();

        return TextureFromColourMap(colourMap, width, height);
    }
}
