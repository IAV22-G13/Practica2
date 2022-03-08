using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    public class WanderMinotaur : ComportamientoAgente
    {
        public Graph grafo;
        public float velocity;
        private int i = 0;
        private List<Vertex> path = null;

        public void Start()
        {
            GameObject dest = grafo.randCass();   
            path = grafo.GetPathBFS(this.gameObject, dest);
            i = 0;
        }

        public override Direccion GetDireccion()
        {
            Direccion result = new Direccion();

            GameObject dest = grafo.randCass();

            objetivo = path[i].gameObject;

            Vector3 direccion = objetivo.transform.position - this.transform.position;
            float distance = direccion.magnitude;

            if (distance < 0.1 && distance > -0.1)
            {
                path = grafo.GetPathBFS(this.gameObject, dest);
                i = 0;
            }
            else i++;

            Vector3 targetVelocity = direccion;
            targetVelocity.Normalize();
            targetVelocity *= velocity;

            result.lineal = targetVelocity;
            agente.transform.rotation = Quaternion.LookRotation(result.lineal, Vector3.up);

            return result;
        }
    }
}
