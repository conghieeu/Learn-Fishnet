using UnityEngine;

namespace DunGen.Demo
{
	public class KeyPickup : MonoBehaviour, IKeyLock
	{
		[HideInInspector]
		[SerializeField]
		private int keyID;

		[HideInInspector]
		[SerializeField]
		private KeyManager keyManager;

		public Key Key => keyManager.GetKeyByID(keyID);

		public void OnKeyAssigned(Key key, KeyManager keyManager)
		{
			keyID = key.ID;
			this.keyManager = keyManager;
		}

		private void OnTriggerEnter(Collider c)
		{
			PlayerInventory component = c.GetComponent<PlayerInventory>();
			if (!(component == null))
			{
				ScreenText.Log("Picked up {0} key", Key.Name);
				component.AddKey(keyID);
				UnityUtil.Destroy(base.gameObject);
			}
		}
	}
}
