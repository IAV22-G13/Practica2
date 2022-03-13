using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    public class GoToCass : ComportamientoAgente
    {
        public GraphGrid grafo;
        [SerializeField]
        float velocity;
        [SerializeField]
        Material material;
        [SerializeField]
        Material materialOld;

        private List<Vertex> path = null;
        private GameObject endOfPath = null;

        private void OnEnable()
        {

            if (this.gameObject.GetComponent<ControlJugador>() != null && grafo.getEndCass() != null)
                endOfPath = grafo.getEndCass();
            else
                endOfPath = grafo.randCass();

            if (this.GetComponent<LineRenderer>() != null)
                this.GetComponent<LineRenderer>().enabled = true;

            path = grafo.GetPathAstar(grafo.GetNearestVertex(this.transform.position).gameObject, endOfPath, grafo.EuclidDist);
            if (GameManager.instance.getSuavizado())
                path = grafo.Smooth(path);
            DrawPath(material);
        }

        private void OnDisable()
        {
            if (this.GetComponent<LineRenderer>() != null)
                this.GetComponent<LineRenderer>().enabled = false;
            if (path != null)
                DrawPath(materialOld);
        }

        public override Direccion GetDireccion()
        {
            if (path == null)
            {
                path = grafo.GetPathAstar(grafo.GetNearestVertex(this.transform.position).gameObject, endOfPath, grafo.EuclidDist);
                Debug.Log(path.Count);
                if (GameManager.instance.getSuavizado())
                    path = grafo.Smooth(path);
                DrawPath(material);
            }

            Direccion result = new Direccion();

            objetivo = path[path.Count - 1].gameObject;

            Vector3 direccion = objetivo.transform.position - this.transform.position;
            float distance = direccion.magnitude;

            if (distance < 0.3 && distance > -0.3)
            {
                Vertex act = path[path.Count - 1];
                path[path.Count - 1].GetComponent<MeshRenderer>().material = materialOld;
                path.RemoveAt(path.Count - 1);
                if (path.Count == 0)
                {
                    if (this.gameObject.GetComponent<ControlJugador>() != null)  //Final laberinto
                    {
                        path = null;
                        return new Direccion();
                    }
                    path = grafo.GetPathAstar(act.gameObject, grafo.randCass(), grafo.EuclidDist);
                    path = grafo.Smooth(path);
                }
            }
            DrawPath(material);

            Vector3 targetVelocity = direccion;
            targetVelocity.Normalize();
            targetVelocity *= velocity;

            result.lineal = targetVelocity;
            agente.transform.rotation = Quaternion.LookRotation(result.lineal, Vector3.up);

            return result;
        }

        public void DrawPath(Material m)
        {
            if (this.GetComponent<LineRenderer>() != null && this.GetComponent<LineRenderer>().enabled)
                this.GetComponent<LineRenderer>().positionCount = path.Count + 1;
            for (int i = 0; i < path.Count; i++)
            {
                if (material != null)
                    path[i].GetComponent<MeshRenderer>().material = m;
                Vector3 pos = path[i].gameObject.transform.position;
                pos.y = 1;                if (this.GetComponent<LineRenderer>() != null)
                    this.GetComponent<LineRenderer>().SetPosition(i, pos);
            }
            Vector3 p = this.gameObject.transform.position;
            p.y = 1;
            if (this.GetComponent<LineRenderer>() != null)
                this.GetComponent<LineRenderer>().SetPosition(path.Count, p);
        }
    }
}
