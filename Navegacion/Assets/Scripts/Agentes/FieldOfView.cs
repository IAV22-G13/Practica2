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

		public LayerMask targetMask;

		[HideInInspector]
		public List<Transform> visibleTargets = new List<Transform>();

		public override Direccion GetDireccion()
		{
			Direccion result = new Direccion();
			visibleTargets.Clear();
			Collider targetsInViewRadius = null;
			if (Physics.OverlapSphere(transform.position, viewRadius, targetMask).Length > 0)
				targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask)[0];

			if(targetsInViewRadius != null)
			{
				Transform target = targetsInViewRadius.transform;
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
				{
					float dstToTarget = Vector3.Distance(transform.position, target.position);

					if (!Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), dirToTarget, dstToTarget))
					{
						visibleTargets.Add(target);
						result.lineal = dirToTarget * speed;
						agente.transform.rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
					}
                    else
                    {
						return result;
                    }
				}
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
