using UnityEngine;

namespace Mimic.Character
{
	public class PuppetPersonViewModeSwitcher : PersonViewModeSwitcher
	{
		[Header("Renderers")]
		[SerializeField]
		private SkinnedMeshRenderer? headRenderer;

		[SerializeField]
		private SkinnedMeshRenderer? headlessRenderer;

		private Material originalHeadMaterial;

		private bool isInitialized;

		private void Start()
		{
			if (headRenderer == null || headlessRenderer == null)
			{
				Debug.LogError("PuppetFPP HeadRenderer or HeadlessRenderer is not assigned.");
				return;
			}
			originalHeadMaterial = new Material(headRenderer.sharedMaterial);
			isInitialized = true;
		}

		public override void SwitchMode(PersonViewMode mode)
		{
			if (headRenderer == null || headlessRenderer == null)
			{
				Debug.LogError("HeadRenderer or HeadlessRenderer is not assigned.");
				return;
			}
			if (!isInitialized)
			{
				originalHeadMaterial = new Material(headRenderer.sharedMaterial);
				isInitialized = true;
			}
			switch (mode)
			{
			case PersonViewMode.First:
				headRenderer.material = originalHeadMaterial;
				break;
			case PersonViewMode.Third:
				headRenderer.material = headlessRenderer.material;
				break;
			default:
				Logger.RError($"Invalid PersonViewMode: {mode}");
				break;
			case PersonViewMode.None:
				break;
			}
		}
	}
}
