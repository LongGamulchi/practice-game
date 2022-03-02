using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (MapGenerator))]//����� Ŭ���� ��� target�� �ڵ����� �� ������Ʈ�� �ȴ�.
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;
        if (DrawDefaultInspector())
        {
            map.GenerateMap();//�������� ���� �׷����� �� �����Ӹ��� ������ �׸��� �Ѵ�.
        }// �ν����Ϳ��� ���� ��ȯ �� ���� ����
        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }//��ũ��Ʈ �󿡼� ��ȭ�� ������ �ݿ� �� �� �ֵ��� ��ư�� �������.
    }
}
