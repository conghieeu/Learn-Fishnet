using System;
using TMPro;
using UnityEngine.UI;

public class UIPrefab_RoomListItem : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_RoomDesc = "RoomDesc";

	public const string UEID_Enter = "Enter";

	private Image _UE_rootNode;

	private TMP_Text _UE_RoomDesc;

	private Button _UE_Enter;

	private Action<string> _OnEnter;

	private string hostExternalAddress;

	private int hostExternalPort;

	private string hostInternalAddress;

	private int hostInternalPort;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_RoomDesc => _UE_RoomDesc ?? (_UE_RoomDesc = PickText("RoomDesc"));

	public Button UE_Enter => _UE_Enter ?? (_UE_Enter = PickButton("Enter"));

	public Action<string> OnEnter
	{
		get
		{
			return _OnEnter;
		}
		set
		{
			_OnEnter = value;
			SetOnButtonClick("Enter", value);
		}
	}
}
