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
        public Material actualCass, lowerCass, normal;
        public void Update()
        {
            if (lastCass != null)
            {
                id = gP.IdToGrid(lastCass.id);
                gP.mapVertices[(int)id.x, (int)id.y] = true;
                lastCass.GetComponent<MeshRenderer>().material = normal;
            }
            for (int i = 0; i < vList.Length; i++)
            {
                gP.costs[vList[i].id] = clearCost;
                vList[i].GetComponent<MeshRenderer>().material = normal;
            }
            vList = gP.poison(this.gameObject);
            lastCass = gP.GetNearestVertex(this.transform.position);
            id = gP.IdToGrid(lastCass.id);
            lastCass.GetComponent<MeshRenderer>().material = actualCass;
            gP.mapVertices[(int)id.x, (int)id.y] = false;

            for (int i = 0; i < vList.Length; i++)
            {
                gP.costs[vList[i].id] = poisonedCost;
                vList[i].GetComponent<MeshRenderer>().material = lowerCass;
            }
        }
    }
}
