using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_Trigger : MonoBehaviour
	{
		private ShaderFaderSSU fader;

		public List<Demo_TriggerEvent> events;

		private void Start()
		{
			fader = GetComponent<ShaderFaderSSU>();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.name == "Player")
			{
				ChangeState(isActive: true);
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.name == "Player")
			{
				ChangeState(isActive: false);
			}
		}

		public void ChangeState(bool isActive)
		{
			if (fader != null)
			{
				fader.isFaded = isActive;
			}
			if (!(events != null && isActive))
			{
				return;
			}
			foreach (Demo_TriggerEvent @event in events)
			{
				StartCoroutine(PlayEvent(@event));
			}
		}

		private IEnumerator PlayEvent(Demo_TriggerEvent demoEvent)
		{
			yield return new WaitForSeconds(demoEvent.delay);
			demoEvent.Play(base.transform);
		}
	}
}
