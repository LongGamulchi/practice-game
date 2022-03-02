using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscUI : MonoBehaviour
{
    public GameObject escPanel;
    bool activeEsc;
    private void Start()
    {
        escPanel.SetActive(activeEsc);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            activeEsc = !activeEsc;
            escPanel.SetActive(activeEsc);
        }
    }
}
