using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// UnityEditor을 해주어야 Editor기능을 하용 할 수 있다.
using UnityEditor;

// 35. 버튼을 표시하려면 CustomEditor을 해주어야된다. 
[CustomEditor (typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 32. 사용자 정의 편집기를 검사하는 객체인 target과 같다, 그 객체를 MapGenerator에 캐스트 하고 싶다.
        MapGenerator mapGen = (MapGenerator)target;

        // 37. DrawDefaultInspector와 같이 값이 변경되었음을 의미한다.
        if (DrawDefaultInspector())
        { 
            // 38.autoUpdate가 true가 되면 업데이트가 되도록 한다.
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        // 33. 버튼을 추가해준다. 
        if (GUILayout.Button("Generate"))
        {
            // 34. 버튼을 누르면 mapGen.GenerateMap()을 한다.
            mapGen.GenerateMap();
        }
    }
}
