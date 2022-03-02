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
    }//사거리에 따라 사거리 표시 오브젝트의 색을 바꾼다.
}
