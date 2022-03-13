using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    public class GoToCass : ComportamientoAgente
    {
        [SerializeField]
        GraphGrid grafo;
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
            this.GetComponent<LineRenderer>().enabled = true;
            if (path != null && endOfPath == null)
            {
                path = grafo.GetPathBFS(this.gameObject, endOfPath);
                if (GameManager.instance.getSuavizado())
                    path = grafo.Smooth(path);
                DrawPath(material);
            }
        }

        private void OnDisable()
        {
            this.GetComponent<LineRenderer>().enabled = false;
            if(path != null) 
                DrawPath(materialOld);
        }

        public override Direccion GetDireccion()
        {
            if (path == null)
            {
                if (this.gameObject.GetComponent<ControlJugador>() != null && grafo.getEndCass() != null)
                    endOfPath = grafo.getEndCass();
                else
                    endOfPath = grafo.randCass();

                path = grafo.GetPathBFS(this.gameObject, endOfPath);
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
                    path = grafo.GetPathBFS(act.gameObject, grafo.randCass());
                    if (GameManager.instance.getSuavizado())
                        path = grafo.Smooth(path);
                }
                else
                {
                    //path = grafo.GetPathBFS(act.gameObject, path[0].gameObject);
                    //path = grafo.Smooth(path);
                    //DrawPath(material);
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
                if(material != null)
                    path[i].GetComponent<MeshRenderer>().material = m;
                Vector3 pos = path[i].gameObject.transform.position;
                pos.y = 1;                this.GetComponent<LineRenderer>().SetPosition(i, pos);
            }
            Vector3 p = this.gameObject.transform.position;
            p.y = 1;
            this.GetComponent<LineRenderer>().SetPosition(path.Count, p);
        }
    }
}
