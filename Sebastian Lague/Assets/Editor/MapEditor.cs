using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // 새로 선언해주고


// CustomEditor 키우드를 사용해 우리가 사용할 클래스를 명시한다.
[CustomEditor (typeof (MapGenerator))]
public class MapEditor : Editor // MonoBehaviour대신에 Editor를 상속한다.
{

    public override void OnInspectorGUI()
    {
        // 우리의 맵 생성기의 래퍼런스를 받아야한다. MapGenerator map 을 선언하고 맵 에디터가 다루는 대상 오브젝트를 가리키는 target을 할당해준다.
        // *CustomEditor 키워드로 이 에디터 스크립트가 다룰것이라 선언한 오브젝트는 target으로 접근할 수 있게 자동으로 설정된다.
        // 그리고 그것을 MapGenerator로 형변환 해서 할당한다.
        MapGenerator map = target as MapGenerator;
        // 베이스(부모)클래스의 OnInspectorGUI 메소드를 불러, 인스펙터의 기본 구성물들을 그린 다음 시작합니다.
        //  base.OnInspectorGUI();
        if (DrawDefaultInspector())
        {
            // GUI가 그려지는 매 프레임에 map.GenerateMap 메소드를 호출해서 맵을 그리게 한다.
            map.GenerateMap();
        }

        if(GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
    
}
