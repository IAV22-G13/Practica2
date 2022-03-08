using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    public class Hilo : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                this.gameObject.GetComponent<GoToCass>().enabled = true;
            }
            else if (Input.GetKeyUp("space"))
            {
                this.gameObject.GetComponent<GoToCass>().enabled = false;
            }
        }
    }
}
