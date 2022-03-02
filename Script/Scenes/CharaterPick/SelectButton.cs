using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour, IPointerUpHandler
{
    public Character character;
    public PointerEventData.InputButton btn1 = PointerEventData.InputButton.Left;
    public Image UnSelect;

    void Update()
    {
        if(CharacterPickManager.instance.currentCharactor != character)
            UnSelect.transform.gameObject.SetActive(true);
    }

    void OnSelect()
    {
        UnSelect.transform.gameObject.SetActive(false);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        CharacterPickManager.instance.currentCharactor = character;
        OnSelect();
        

    }
}
