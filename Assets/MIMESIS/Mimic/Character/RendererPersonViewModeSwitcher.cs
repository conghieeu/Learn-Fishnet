using UnityEngine;

namespace Mimic.Character
{
	public class RendererPersonViewModeSwitcher : PersonViewModeSwitcher
	{
		private Renderer[]? renderers;

		private Material[]? initialMaterials;

		public override void SwitchMode(PersonViewMode mode)
		{
			if (renderers == null)
			{
				renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
			}
			if (initialMaterials == null)
			{
				initialMaterials = new Material[renderers.Length];
				for (int i = 0; i < renderers.Length; i++)
				{
					initialMaterials[i] = new Material(renderers[i].sharedMaterial);
				}
			}
			switch (mode)
			{
			case PersonViewMode.First:
			{
				Material firstPersonViewModeMaterial = GetFirstPersonViewModeMaterial();
				for (int k = 0; k < renderers.Length; k++)
				{
					if (renderers[k] != null)
					{
						renderers[k].material = firstPersonViewModeMaterial;
					}
				}
				break;
			}
			case PersonViewMode.Third:
			{
				for (int j = 0; j < renderers.Length; j++)
				{
					if (renderers[j] != null)
					{
						renderers[j].material = initialMaterials[j];
					}
				}
				break;
			}
			default:
				Logger.RError($"Invalid PersonViewMode: {mode}");
				break;
			case PersonViewMode.None:
				break;
			}
		}

		private static Material? GetFirstPersonViewModeMaterial()
		{
			if (Hub.s != null && Hub.s.tableman != null && Hub.s.tableman.material != null)
			{
				return Hub.s.tableman.material.shadowOnly;
			}
			return null;
		}
	}
}
