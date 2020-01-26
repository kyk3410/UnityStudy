using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    // 16. 텍스처를 설정할 수 있도록 표면의 Renderer에 대한 참조가 필요하다.
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;


    public void DrawTexture(Texture2D texture)
    {

        // 28. 텍스처를 텍스처렌더러에 적용하려고 할때, 또한 맵을 볼 때마다 재생 모드로 가고 싶지 않다는 것이다, 에디터에서 그것들을 생성 할 수 있으면 더 편리 할 것이다. 
        // 29. textureRender.material을 사용할 수 없다는 것을 의미한다, 런타임시에만 인스턴스화되기 때문이다.
        // 다른 한편으로, sharedMaterial을 사용 할 수 있다, 그래서 sharedMaterial의 mainTexture를 설정한다
        textureRender.sharedMaterial.mainTexture = texture;
        // 30. 바닥의 크기가 지도와 일치하면 좋으므로 textureRender.transform.localScale은 새로운 Vector3에 해당한다. x 치수는 너비와 같고 y값은 1이고 z값은 높이이다.  
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
