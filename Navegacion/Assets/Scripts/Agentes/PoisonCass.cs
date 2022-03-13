using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    public class PoisonCass : MonoBehaviour
    {
        public GraphGrid gP;
        public int poisonedCost = 8;
        public int clearCost = 1;
        Vertex[] vList = new Vertex[0];
        public void Update()
        {
            for (int i = 0; i < vList.Length; i++)
            {
                vList[i].cost = clearCost;
            }
            vList = gP.poison(this.gameObject);
            for (int i = 0; i < vList.Length; i++)
            {
                vList[i].cost = poisonedCost;
            }
        }
    }
}
