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
        Vertex lastCass = null;
        Vector2 id;
        public void Update()
        {
            if (lastCass != null)
            {
                id = gP.IdToGrid(lastCass.id);
                gP.mapVertices[(int)id.x, (int)id.y] = true;
            }
            for (int i = 0; i < vList.Length; i++)
            {
                gP.costs[vList[i].id] = clearCost;
            }
            vList = gP.poison(this.gameObject);
            id = gP.IdToGrid(lastCass.id);
            lastCass = gP.GetNearestVertex(this.transform.position);
            gP.mapVertices[(int)id.x, (int)id.y] = false;
            for (int i = 0; i < vList.Length; i++)
            {
                gP.costs[vList[i].id] = poisonedCost;
            }
        }
    }
}
