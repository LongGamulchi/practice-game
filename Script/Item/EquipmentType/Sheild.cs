using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheild : MonoBehaviour
{

    public Material[] sheildColors = new Material[4];
    public Sprite[] itemImage = new Sprite[4];
    public MeshRenderer meshRenderer;
    SheildController sheildController;
    Inventory inven;

    public void Start()
    {
        inven = GetComponentInParent<Inventory>();
        sheildController = GetComponentInParent<SheildController>();
        sheildController.onSheildColor += UpdateMaterial;
    }
    public void UpdateMaterial(float maxSheild, float sheild)
    { 
        if (maxSheild == 25)
        {
            meshRenderer.material = sheildColors[0];
            sheildController.item.itemImage = itemImage[0];
            if(sheild == 0)
            {

            }
        }
        else if (maxSheild == 50)
        {
            meshRenderer.material = sheildColors[1];
            sheildController.item.itemImage = itemImage[1];
            if (sheild == 0)
            {

            }
        }
        else if (maxSheild == 75)
        {
            meshRenderer.material = sheildColors[2];
            sheildController.item.itemImage = itemImage[2];
            if (sheild == 0)
            {

            }
        }
        else if (maxSheild == 100)
        {
            meshRenderer.material = sheildColors[3];
            sheildController.item.itemImage = itemImage[3];
            if (sheild == 0)
            {

            }
        }
    }//실드 레벨별로 색과 아이템 이미지를 바꿔준다. 후에는 깨진 상태일 때 모습도 추가해주자.
}

