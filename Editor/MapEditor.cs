using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (MapGenerator))]//사용할 클래스 명시 target은 자동으로 그 오브젝트가 된다.
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;
        if (DrawDefaultInspector())
        {
            map.GenerateMap();//구성물이 새로 그려지는 메 프레임마다 실행해 그리게 한다.
        }// 인스펙터에서 값이 변환 될 때만 실행
        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }//스크립트 상에서 변화를 줬을떄 반영 할 수 있도록 버튼을 만들었다.
    }
}
