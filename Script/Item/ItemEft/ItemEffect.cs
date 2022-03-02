using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract bool ExecuteRole(GameObject gameObject);
}//추상메소드
