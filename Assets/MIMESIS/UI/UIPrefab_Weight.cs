using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_Weight : UIPrefabScript
{
	public const string UEID_BG = "BG";

	public const string UEID_Txt_CurrentWeight = "Txt_CurrentWeight";

	private Image _UE_BG;

	private TMP_Text _UE_Txt_CurrentWeight;

	private Color defaultColor1;

	private Color defaultColor2;

	[SerializeField]
	private Color triggeredColor1;

	[SerializeField]
	private Color triggeredColor2;

	public Image UE_BG => _UE_BG ?? (_UE_BG = PickImage("BG"));

	public TMP_Text UE_Txt_CurrentWeight => _UE_Txt_CurrentWeight ?? (_UE_Txt_CurrentWeight = PickText("Txt_CurrentWeight"));

	public void OnSetWeight(int weightGram)
	{
		int num = Mathf.CeilToInt((float)weightGram / 1000f);
		UE_Txt_CurrentWeight.text = $"{num}kg ↓";
	}

	public void OnTrapStateChanged(TrapState state)
	{
		switch (state)
		{
		case TrapState.Triggered:
			UE_Txt_CurrentWeight.colorGradient = new VertexGradient(triggeredColor1, triggeredColor1, triggeredColor2, triggeredColor2);
			break;
		case TrapState.Ready:
			UE_Txt_CurrentWeight.colorGradient = new VertexGradient(defaultColor1, defaultColor1, defaultColor2, defaultColor2);
			break;
		}
	}

	private new void Awake()
	{
		base.Awake();
		base.gameObject.SetActive(value: true);
		defaultColor1 = UE_Txt_CurrentWeight.colorGradient.topLeft;
		defaultColor2 = UE_Txt_CurrentWeight.colorGradient.bottomLeft;
	}

	private void Start()
	{
		Transform parent = base.transform.parent;
		if (parent == null)
		{
			return;
		}
		foreach (Transform item in parent)
		{
			if (!(item == base.transform) && item.TryGetComponent<TrapLevelObject>(out var component) && component.TrapType == TrapType.Weight_Controller)
			{
				component.AddOnTrapStateChanged(OnTrapStateChanged);
			}
		}
	}

	private void OnEnable()
	{
		UE_Txt_CurrentWeight.enabled = true;
	}
}
