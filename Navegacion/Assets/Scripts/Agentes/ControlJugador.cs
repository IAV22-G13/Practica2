/*    
   Copyright (C) 2020 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Navegacion
{

    using UnityEngine;

    /// <summary>
    /// Clara para el comportamiento de agente que consiste en ser el jugador
    /// </summary>
    public class ControlJugador : ComportamientoAgente
    {
        /// <summary>
        /// Obtiene la dirección
        /// </summary>
        /// <returns></returns>

        public override Direccion GetDireccion()
        {
            if (Input.GetKeyDown("space"))
            {
                this.GetComponent<AudioSource>().Play();
                //suena musica
            }
            if (Input.GetKeyUp("space"))
            {
                this.GetComponent<AudioSource>().Pause();
                //suena musica
            }

            Direccion direccion = new Direccion();
            direccion.lineal.x = Input.GetAxis("Horizontal");
            direccion.lineal.z = Input.GetAxis("Vertical");
            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;

            //this.transform.rotation = new Quaternion(0, Mathf.Atan2(-agente.velocidad.x, agente.velocidad.z), 0, 0);
            agente.transform.rotation = Quaternion.LookRotation(direccion.lineal, Vector3.up);

            return direccion;
        }
    }
}