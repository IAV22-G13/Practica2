using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UCM.IAV.Navegacion
{
    public class FieldOfView : ComportamientoAgente
    {
		public float viewRadius;
		[Range(0, 360)]
		public float viewAngle;
		public float speed;
		public float raySize;

		public LayerMask targetMask;
		public LayerMask obstacleMask;

		[HideInInspector]
		public List<Transform> visibleTargets = new List<Transform>();

		private float timer =1000000;
		private float maxTime = 5;
		private Direccion lastDireccion;

		public override Direccion GetDireccion()
		{
			Direccion result = new Direccion();
			visibleTargets.Clear();
			Collider targetsInViewRadius = null;
			if (Physics.OverlapSphere(transform.position, viewRadius, targetMask).Length > 0)
				targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask)[0];
			Vector3 dirToTarget = new Vector3();
			if (targetsInViewRadius != null)
			{
				Transform target = targetsInViewRadius.transform;
				dirToTarget = (target.position - transform.position);
				if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
				{
					Vector3 fromPt = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
					Vector3 fromPt1 = new Vector3(fromPt.x - raySize, fromPt.y, fromPt.z - raySize);
					Vector3 fromPt2 = new Vector3(fromPt.x + raySize, fromPt.y, fromPt.z - raySize);
					Vector3 fromPt3 = new Vector3(fromPt.x - raySize, fromPt.y, fromPt.z + raySize);
					Vector3 fromPt4 = new Vector3(fromPt.x + raySize, fromPt.y, fromPt.z + raySize);

					Vector3 toPt = new Vector3(target.position.x, target.position.y + 0.5f, target.position.z);
					Vector3 toPt1 = new Vector3(toPt.x - raySize, toPt.y, toPt.z - raySize);
					Vector3 toPt2 = new Vector3(toPt.x + raySize, toPt.y, toPt.z - raySize);
					Vector3 toPt3 = new Vector3(toPt.x - raySize, toPt.y, toPt.z + raySize);
					Vector3 toPt4 = new Vector3(toPt.x + raySize, toPt.y, toPt.z + raySize);

					if (!Physics.Raycast(fromPt1, toPt1- fromPt1, Vector3.Distance(fromPt1, toPt1), obstacleMask) && 
						!Physics.Raycast(fromPt2, toPt2 - fromPt2, Vector3.Distance(fromPt2, toPt2), obstacleMask) &&
						!Physics.Raycast(fromPt3, toPt3 - fromPt3, Vector3.Distance(fromPt3, toPt3), obstacleMask) &&
						!Physics.Raycast(fromPt4, toPt4 - fromPt4, Vector3.Distance(fromPt4, toPt4), obstacleMask))
					{
						visibleTargets.Add(target);
						result.lineal = dirToTarget.normalized * speed;
						agente.transform.rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
						timer = 0;
						lastDireccion = result;
					}
					else if (timer < maxTime)
					{
						timer += Time.deltaTime;
						//agente.transform.rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
						return lastDireccion;
					}
				}
				else if (timer < maxTime)
				{
					timer += Time.deltaTime;
					//agente.transform.rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
					return lastDireccion;
				}
			}
            else if (timer < maxTime)
            {
				timer += Time.deltaTime;
				//agente.transform.rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
				return lastDireccion;
            }
			return result;
		}

		public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
		{
			if (!angleIsGlobal)
			{
				angleInDegrees += transform.eulerAngles.y;
			}
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}
	}
}
