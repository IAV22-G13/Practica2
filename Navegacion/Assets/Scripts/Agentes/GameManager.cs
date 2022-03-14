using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    public class GameManager : MonoBehaviour
    {
        static public GameManager instance;
        public GraphGrid gP;
        public bool suavizado = true;
        // Update is called once per frame
        public void Start()
        {
            instance = this;
        }

        public void Update()
        {
            if (Input.GetKeyDown("r"))
            {
                Application.LoadLevel(Application.loadedLevelName);
            }
            if (Input.GetKeyDown("k"))
            {
                suavizado = !suavizado;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public bool getSuavizado()
        {
            return suavizado;
        }
    }
}