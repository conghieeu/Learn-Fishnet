using UnityEngine;

[CreateAssetMenu(fileName = "MMSpriteTable", menuName = "_Mimic/MMTooltipThemeTable", order = 0)]
public class MMTooltipTheme : ScriptableObject
{
	[Header("Animation")]
	public float fadeInDuration = 0.15f;

	public float fadeOutDuration = 0.1f;
}
