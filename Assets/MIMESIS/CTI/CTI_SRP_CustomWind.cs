using UnityEngine;

namespace CTI
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(WindZone))]
	public class CTI_SRP_CustomWind : MonoBehaviour
	{
		private WindZone m_WindZone;

		private Vector3 WindDirection;

		private float WindStrength;

		private float WindTurbulence;

		public float WindMultiplier = 1f;

		private bool init;

		private int CTIWindPID;

		private int CTITurbulencedPID;

		private Transform trans;

		private void Init()
		{
			m_WindZone = GetComponent<WindZone>();
			CTIWindPID = Shader.PropertyToID("_CTI_SRP_Wind");
			CTITurbulencedPID = Shader.PropertyToID("_CTI_SRP_Turbulence");
			trans = base.transform;
		}

		private void OnValidate()
		{
			Update();
		}

		private void Update()
		{
			if (!init)
			{
				Init();
			}
			WindDirection = trans.forward;
			WindStrength = m_WindZone.windMain;
			WindStrength += m_WindZone.windPulseMagnitude * (1f + Mathf.Sin(Time.time * m_WindZone.windPulseFrequency) + 1f + Mathf.Sin(Time.time * m_WindZone.windPulseFrequency * 3f)) * 0.5f;
			WindStrength *= WindMultiplier;
			WindTurbulence = m_WindZone.windTurbulence * m_WindZone.windMain * WindMultiplier;
			Shader.SetGlobalVector(CTIWindPID, new Vector4(WindDirection.x, WindDirection.y, WindDirection.z, WindStrength));
			Shader.SetGlobalFloat(CTITurbulencedPID, WindTurbulence);
		}
	}
}
