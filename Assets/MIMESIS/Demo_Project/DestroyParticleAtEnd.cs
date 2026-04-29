using UnityEngine;

namespace Demo_Project
{
	public class DestroyParticleAtEnd : MonoBehaviour
	{
		private void Start()
		{
		}

		private void Update()
		{
			_ = GetComponent<ParticleSystem>().time;
			_ = GetComponent<ParticleSystem>().main.duration;
		}
	}
}
