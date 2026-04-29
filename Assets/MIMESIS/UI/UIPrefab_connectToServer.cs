using System;
using TMPro;
using UnityEngine.UI;

public class UIPrefab_connectToServer : UIPrefabScript
{
	public const string UEID_rootNode = "rootNode";

	public const string UEID_title = "title";

	public const string UEID_VoipServerIP = "VoipServerIP";

	public const string UEID_HostButton = "HostButton";

	public const string UEID_HostAddress = "HostAddress";

	public const string UEID_ConnectButton = "ConnectButton";

	private Image _UE_rootNode;

	private TMP_Text _UE_title;

	private TMP_InputField _UE_VoipServerIP;

	private Button _UE_HostButton;

	private Action<string> _OnHostButton;

	private TMP_InputField _UE_HostAddress;

	private Button _UE_ConnectButton;

	private Action<string> _OnConnectButton;

	public Image UE_rootNode => _UE_rootNode ?? (_UE_rootNode = PickImage("rootNode"));

	public TMP_Text UE_title => _UE_title ?? (_UE_title = PickText("title"));

	public TMP_InputField UE_VoipServerIP => _UE_VoipServerIP ?? (_UE_VoipServerIP = PickInputField("VoipServerIP"));

	public Button UE_HostButton => _UE_HostButton ?? (_UE_HostButton = PickButton("HostButton"));

	public Action<string> OnHostButton
	{
		get
		{
			return _OnHostButton;
		}
		set
		{
			_OnHostButton = value;
			SetOnButtonClick("HostButton", value);
		}
	}

	public TMP_InputField UE_HostAddress => _UE_HostAddress ?? (_UE_HostAddress = PickInputField("HostAddress"));

	public Button UE_ConnectButton => _UE_ConnectButton ?? (_UE_ConnectButton = PickButton("ConnectButton"));

	public Action<string> OnConnectButton
	{
		get
		{
			return _OnConnectButton;
		}
		set
		{
			_OnConnectButton = value;
			SetOnButtonClick("ConnectButton", value);
		}
	}
}
