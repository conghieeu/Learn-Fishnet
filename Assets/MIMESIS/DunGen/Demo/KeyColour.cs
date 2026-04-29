using UnityEngine;

namespace DunGen.Demo
{
	public class KeyColour : MonoBehaviour, IKeyLock
	{
		[SerializeField]
		private int keyID;

		[SerializeField]
		private KeyManager keyManager;

		public void OnKeyAssigned(Key key, KeyManager manager)
		{
			keyID = key.ID;
			keyManager = manager;
			SetColour(key.Colour);
		}

		private void Start()
		{
			if (!(keyManager == null))
			{
				Key keyByID = keyManager.GetKeyByID(keyID);
				SetColour(keyByID.Colour);
			}
		}

		private void SetColour(Color colour)
		{
			if (Application.isPlaying)
			{
				Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].material.color = colour;
				}
			}
		}
	}
}
