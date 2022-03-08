using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    public class WanderMinotaur : ComportamientoAgente
    {
        public Graph grafo;
        public float velocity;
        private List<Vertex> path = null;

        public void Start()
        {
        }

        public override Direccion GetDireccion()
        {
            if (path == null)
            {
                path = grafo.GetPathBFS(this.gameObject, grafo.randCass());
                path = grafo.Smooth(path);
            }

            Direccion result = new Direccion();

            objetivo = path[path.Count - 1].gameObject;

            Vector3 direccion = objetivo.transform.position - this.transform.position;
            float distance = direccion.magnitude;

            if (distance < 0.5 && distance > -0.5)
            {
                Vertex act = path[path.Count - 1];
                path.RemoveAt(path.Count - 1);
                if (path.Count == 0)
                {
                    path = grafo.GetPathBFS(act.gameObject, grafo.randCass());
                    path = grafo.Smooth(path);
                }
                else
                {
                    path = grafo.GetPathBFS(act.gameObject, path[0].gameObject);
                    path = grafo.Smooth(path);
                }
            }

            Vector3 targetVelocity = direccion;
            targetVelocity.Normalize();
            targetVelocity *= velocity;

            result.lineal = targetVelocity;
            agente.transform.rotation = Quaternion.LookRotation(result.lineal, Vector3.up);

            return result;
        }
    }
}
