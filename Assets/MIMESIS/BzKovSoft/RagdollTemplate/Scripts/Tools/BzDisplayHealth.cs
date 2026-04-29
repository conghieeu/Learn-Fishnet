using BzKovSoft.RagdollTemplate.Scripts.Charachter;
using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Tools
{
	public sealed class BzDisplayHealth : MonoBehaviour
	{
		[SerializeField]
		private BzHealth _bzHealth;

		private GUIStyle _labelStile;

		private void OnGUI()
		{
			if (!(_bzHealth == null))
			{
				if (_labelStile == null)
				{
					_labelStile = GUI.skin.GetStyle("Label");
					_labelStile.alignment = TextAnchor.UpperCenter;
				}
				GUI.Label(new Rect((Screen.width - 100) / 2, 10f, 100f, 100f), "Health: " + (_bzHealth.Health * 100f).ToString("N0"), _labelStile);
			}
		}
	}
}
