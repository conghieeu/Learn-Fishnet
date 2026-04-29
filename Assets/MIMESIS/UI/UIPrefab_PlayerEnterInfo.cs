using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_PlayerEnterInfo : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_Layout = "Layout";

	private Image _UE_rootNode;

	private Transform _UE_Layout;

	[SerializeField]
	public List<TMP_Text> PlayerInfos;

	[SerializeField]
	public long displayTimeSecMilliSec = 2000L;

	[SerializeField]
	public long fadeOutDisplayTimeMilliSec = 500L;

	private List<long> displayStartTimeMilliSec = new List<long>();

	private List<string> currentDisplayed = new List<string>();

	private List<bool> isEnteringFlags = new List<bool>();

	[SerializeField]
	private string enteringPostfix = "ROOM_ENTER_STRING";

	[SerializeField]
	private string exitingPostfix = "ROOM_EXIT_STRING";

	[SerializeField]
	private Color EnterColor1 = Color.green;

	[SerializeField]
	private Color EnterColor2 = Color.green;

	[SerializeField]
	private Color ExitColor1 = Color.red;

	[SerializeField]
	private Color ExitColor2 = Color.red;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public Transform UE_Layout => _UE_Layout ?? (_UE_Layout = PickTransform("Layout"));

	private void Start()
	{
		StartCoroutine(UpdatePlayerInfos());
	}

	public void AddPlayerInfo(string userName, bool isEntering)
	{
		if (currentDisplayed.Count == PlayerInfos.Count)
		{
			currentDisplayed.RemoveAt(0);
			displayStartTimeMilliSec.RemoveAt(0);
			isEnteringFlags.RemoveAt(0);
		}
		string text = "";
		text = ((!isEntering) ? Hub.GetL10NText(exitingPostfix) : Hub.GetL10NText(enteringPostfix));
		text = text.Replace("[usernickname:]", userName);
		currentDisplayed.Add(text);
		displayStartTimeMilliSec.Add(Hub.s.timeutil.GetCurrentTickMilliSec());
		isEnteringFlags.Add(isEntering);
	}

	private IEnumerator UpdatePlayerInfos()
	{
		while (true)
		{
			yield return new WaitUntil(() => currentDisplayed.Count > 0);
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			while (displayStartTimeMilliSec.Count > 0 && currentTickMilliSec - displayStartTimeMilliSec[0] > displayTimeSecMilliSec + fadeOutDisplayTimeMilliSec)
			{
				currentDisplayed.RemoveAt(0);
				displayStartTimeMilliSec.RemoveAt(0);
				isEnteringFlags.RemoveAt(0);
			}
			for (int num = 0; num < PlayerInfos.Count; num++)
			{
				if (num < currentDisplayed.Count)
				{
					float num2 = Mathf.Clamp01((float)(currentTickMilliSec - displayStartTimeMilliSec[num] - displayTimeSecMilliSec) / (float)fadeOutDisplayTimeMilliSec);
					PlayerInfos[num].text = currentDisplayed[num];
					if (isEnteringFlags[num])
					{
						Color enterColor = EnterColor1;
						enterColor.a = 1f - num2;
						Color enterColor2 = EnterColor2;
						enterColor2.a = 1f - num2;
						PlayerInfos[num].colorGradient = new VertexGradient(enterColor, enterColor, enterColor2, enterColor2);
					}
					else
					{
						Color enterColor = ExitColor1;
						enterColor.a = 1f - num2;
						Color enterColor2 = ExitColor2;
						enterColor2.a = 1f - num2;
						PlayerInfos[num].colorGradient = new VertexGradient(enterColor, enterColor, enterColor2, enterColor2);
					}
					PlayerInfos[num].gameObject.SetActive(value: true);
				}
				else
				{
					PlayerInfos[num].gameObject.SetActive(value: false);
					PlayerInfos[num].text = string.Empty;
				}
			}
			yield return null;
		}
	}
}
