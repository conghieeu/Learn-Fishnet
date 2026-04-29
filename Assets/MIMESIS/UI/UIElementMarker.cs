using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementMarker : MonoBehaviour
{
	public enum eElementType
	{
		EmptyObject = 0,
		Text = 1,
		Image = 2,
		Button = 3,
		InputField = 4
	}

	[SerializeField]
	[Header("240502 기능 추가 : ignoreMe = true 면 collect 대상에서 제외됨")]
	private bool _ignoreMe;

	[InspectorReadOnly]
	[Header("참고 : element type 은 UIPrefabScript 에서\ncollect를 할 때 update 됨")]
	public eElementType elementType;

	[InspectorReadOnly]
	public Button asButton;

	[InspectorReadOnly]
	public Image asImage;

	[InspectorReadOnly]
	public TMP_Text asText;

	[InspectorReadOnly]
	public TMP_InputField asInputField;

	public bool IgnoreMe => _ignoreMe;
}
