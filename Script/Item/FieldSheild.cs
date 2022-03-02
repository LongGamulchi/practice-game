using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSheild : FieldItems
{
    public Material[] sheildColors = new Material[4];
    public MeshRenderer meshRenderer;

    public void UpdateColor(float maxSheild)
    {
        if (maxSheild == 25)
        {
            meshRenderer.material = sheildColors[0];
        }
        else if (maxSheild == 50)
        {
            meshRenderer.material = sheildColors[1];
        }
        else if (maxSheild == 75)
        {
            meshRenderer.material = sheildColors[2];
        }
        else if (maxSheild == 100)
        {
            meshRenderer.material = sheildColors[3];
        }
    }//최대 실드량에 따라 실드의 색을 정한다.
}
