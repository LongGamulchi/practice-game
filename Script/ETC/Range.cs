using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{
    public MeshRenderer rangeColor;
    public Material[] rangeMaterials = new Material[2];

    public void UpdateMaterialOn()
    {
        rangeColor.material = rangeMaterials[1];
    }
    public void UpdateMaterialOff()
    {
        rangeColor.material = rangeMaterials[0];
    }//��Ÿ��� ���� ��Ÿ� ǥ�� ������Ʈ�� ���� �ٲ۴�.
}
