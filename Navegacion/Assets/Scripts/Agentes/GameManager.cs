using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            Application.LoadLevel(Application.loadedLevelName);
        }
        if (Input.GetKeyDown("t"))
        {
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}