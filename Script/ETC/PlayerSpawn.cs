using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject player;
    bool isDrop;

    void Start()
    {
        if (CharacterPickManager.instance.currentCharactor != Character.Null)
            player = Instantiate(characters[(int)(CharacterPickManager.instance.currentCharactor) - 1]);
    }

    private void Update()
    {
        if (!isDrop)
        {
            player.GetComponentInChildren<Player>().transform.position = transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDrop = true;
        }
    }
}
