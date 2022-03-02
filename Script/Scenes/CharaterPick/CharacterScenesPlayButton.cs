using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharacterScenesPlayButton : MonoBehaviour, IPointerUpHandler
{
    public PointerEventData.InputButton btn1 = PointerEventData.InputButton.Left;
    public void OnPointerUp(PointerEventData eventData)
    {
        if(CharacterPickManager.instance.currentCharactor != Character.Null)
            SceneManager.LoadScene("GameScenes");
    }
}
