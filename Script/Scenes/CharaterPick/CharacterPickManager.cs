using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Character
{
  Null,  OldMan,Red,Blue ,Yellow
}
public class CharacterPickManager : MonoBehaviour
{
    public static CharacterPickManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null) return;
        DontDestroyOnLoad(gameObject);
    }

    public Character currentCharactor;
}
