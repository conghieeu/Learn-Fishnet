using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_titleBG : UIPrefabScript
{
	public const string UEID_bg = "bg";

	public const string UEID_Video_Player = "Video_Player";

	private Image _UE_bg;

	private Transform _UE_Video_Player;

	public Image UE_bg => _UE_bg ?? (_UE_bg = PickImage("bg"));

	public Transform UE_Video_Player => _UE_Video_Player ?? (_UE_Video_Player = PickTransform("Video_Player"));
}
