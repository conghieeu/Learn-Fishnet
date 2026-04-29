using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_Spectator_PlayerListViewItem : UIPrefabScript
{
	public const string UEID_Name_Text = "Name_Text";

	private TMP_Text _UE_Name_Text;

	[SerializeField]
	private Image speakIcon;

	[SerializeField]
	private SpriteChangeAnimation spriteChangeAnimation;

	public TMP_Text UE_Name_Text => _UE_Name_Text ?? (_UE_Name_Text = PickText("Name_Text"));

	public Image SpeakIcon => speakIcon;

	public SpriteChangeAnimation SpriteChangeAnimation => spriteChangeAnimation;

	public void SetColor(Color color)
	{
		UE_Name_Text.color = color;
		speakIcon.color = color;
	}
}
