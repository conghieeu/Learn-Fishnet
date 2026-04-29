using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Steamworks;
using UnityEngine;

public class SteamNetworkingSocketsTest : MonoBehaviour
{
	private enum MsgType : uint
	{
		Ping = 0u,
		Ack = 1u
	}

	private Vector2 m_ScrollPos;

	private string ServerSteamIDStr = "";

	private ulong ServerSteamID;

	private HSteamListenSocket listenSocket = HSteamListenSocket.Invalid;

	private HSteamNetPollGroup pollGroup = HSteamNetPollGroup.Invalid;

	private const int virtualPort = 0;

	private readonly HashSet<HSteamNetConnection> connectedClients = new HashSet<HSteamNetConnection>();

	private bool isServerRunning;

	private HSteamNetConnection serverConn = HSteamNetConnection.Invalid;

	private bool isConnectedToServer;

	private Callback<SteamNetConnectionStatusChangedCallback_t> connectionStatusChangedCallback;

	private void Start()
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("Steamworks 초기화 실패");
		}
		else
		{
			SteamNetworkingUtils.InitRelayNetworkAccess();
		}
	}

	private void OnEnable()
	{
		connectedClients.Clear();
	}

	private void OnDisable()
	{
		if (listenSocket != HSteamListenSocket.Invalid)
		{
			SteamNetworkingSockets.CloseListenSocket(listenSocket);
			if (connectionStatusChangedCallback != null)
			{
				connectionStatusChangedCallback.Unregister();
				connectionStatusChangedCallback = null;
			}
		}
		if (pollGroup != HSteamNetPollGroup.Invalid)
		{
			SteamNetworkingSockets.DestroyPollGroup(pollGroup);
		}
		foreach (HSteamNetConnection connectedClient in connectedClients)
		{
			SteamNetworkingSockets.CloseConnection(connectedClient, 0, "서버 종료", bEnableLinger: false);
		}
		connectedClients.Clear();
	}

	private void Update()
	{
		if (isServerRunning)
		{
			ServerReceiveMessages();
		}
		else if (isConnectedToServer)
		{
			ClientReceiveMessages();
		}
	}

	private void OnGUI()
	{
		RenderOnGUI();
	}

	public void RenderOnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width / 2 + 5, 0f, Screen.width / 2 - 10, Screen.height));
		GUILayout.Label("Variables:");
		GUILayout.Label("AppID: " + SteamUtils.GetAppID().ToString());
		GUILayout.Label("");
		GUILayout.Label("IsServer: " + isServerRunning);
		GUILayout.BeginVertical("box");
		foreach (HSteamNetConnection connectedClient in connectedClients)
		{
			if (SteamNetworkingSockets.GetConnectionInfo(connectedClient, out var pInfo))
			{
				GUILayout.Label("Client: " + GetSteamNetConnectionInfoAllString(pInfo));
			}
			else
			{
				GUILayout.Label("Client: " + SteamNetworkingSockets.GetConnectionUserData(connectedClient));
			}
		}
		GUILayout.EndVertical();
		GUILayout.Label("");
		GUILayout.Label("IsClient: " + isConnectedToServer);
		GUILayout.BeginVertical("box");
		if (SteamNetworkingSockets.GetConnectionInfo(serverConn, out var pInfo2))
		{
			GUILayout.Label("Server: " + GetSteamNetConnectionInfoAllString(pInfo2));
		}
		else
		{
			GUILayout.Label("Server: " + SteamNetworkingSockets.GetConnectionUserData(serverConn));
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUILayout.BeginVertical("box");
		m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(Screen.width / 2 - 5), GUILayout.Height(Screen.height - 33));
		string text = SteamUser.GetSteamID().ToString();
		GUILayout.BeginVertical("box");
		GUILayout.Label("MySteamID : " + text);
		if (GUILayout.Button("Copy to clipboard"))
		{
			GUIUtility.systemCopyBuffer = text;
		}
		GUILayout.EndVertical();
		GUILayout.Label("");
		GUILayout.BeginVertical("box");
		GUILayout.Label("Host/Server : " + isServerRunning);
		GUI.enabled = !isServerRunning && !isConnectedToServer;
		if (GUILayout.Button("Start : CreateListenSocketP2P"))
		{
			SteamNetworkingConfigValue_t[] pOptions = null;
			listenSocket = SteamNetworkingSockets.CreateListenSocketP2P(0, 0, pOptions);
			if (listenSocket == HSteamListenSocket.Invalid)
			{
				Debug.LogError("ListenSocket 생성 실패");
			}
			else
			{
				Debug.Log("ListenSocket 생성 성공");
				pollGroup = SteamNetworkingSockets.CreatePollGroup();
				connectionStatusChangedCallback = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionStatusChanged);
				isServerRunning = true;
			}
		}
		GUI.enabled = true;
		GUI.enabled = isServerRunning;
		if (GUILayout.Button("CloseClientConnections"))
		{
			foreach (HSteamNetConnection connectedClient2 in connectedClients)
			{
				SteamNetworkingSockets.CloseConnection(connectedClient2, 0, "서버가 연결 종료", bEnableLinger: false);
			}
			connectedClients.Clear();
			Debug.Log("모든 클라이언트 연결 종료");
		}
		GUI.enabled = true;
		GUI.enabled = isServerRunning;
		if (GUILayout.Button("Stop : CloseListenSocket"))
		{
			SteamNetworkingSockets.CloseListenSocket(listenSocket);
			listenSocket = HSteamListenSocket.Invalid;
			if (connectionStatusChangedCallback != null)
			{
				connectionStatusChangedCallback.Unregister();
				connectionStatusChangedCallback = null;
			}
			isServerRunning = false;
		}
		GUI.enabled = true;
		GUI.enabled = isServerRunning;
		if (GUILayout.Button("BroadcastMessageToClients : SteamNetworkingSockets.SendMessageToConnection"))
		{
			BroadcastMessageToClients("BroadcastMessageToClients : Hello, all clients!");
		}
		GUI.enabled = true;
		GUILayout.EndVertical();
		GUILayout.Label("");
		GUILayout.BeginVertical("box");
		GUILayout.Label("Client : " + isConnectedToServer);
		GUILayout.BeginVertical("box");
		GUILayout.Label("Enter ServerSteamID : ");
		GUI.enabled = !isConnectedToServer;
		ServerSteamIDStr = GUILayout.TextField(ServerSteamIDStr);
		if (GUILayout.Button("Paste from clipboard"))
		{
			ServerSteamIDStr = GUIUtility.systemCopyBuffer;
		}
		GUI.enabled = true;
		GUILayout.EndVertical();
		GUILayout.Label("");
		GUI.enabled = !isServerRunning && !isConnectedToServer;
		if (GUILayout.Button("Start : ConnectP2P"))
		{
			if (ServerSteamIDStr == string.Empty)
			{
				MonoBehaviour.print("ServerSteamID를 입력하세요.");
				return;
			}
			if (!ulong.TryParse(ServerSteamIDStr, out ServerSteamID))
			{
				MonoBehaviour.print("ServerSteamID를 올바른 숫자로 입력하세요.");
				return;
			}
			SteamNetworkingIdentity identityRemote = default(SteamNetworkingIdentity);
			identityRemote.SetSteamID64(ServerSteamID);
			SteamNetworkingConfigValue_t[] pOptions2 = null;
			serverConn = SteamNetworkingSockets.ConnectP2P(ref identityRemote, 0, 0, pOptions2);
			if (serverConn == HSteamNetConnection.Invalid)
			{
				MonoBehaviour.print("서버 연결 실패 to steamID : " + ServerSteamID);
			}
			else
			{
				MonoBehaviour.print($"서버 연결 요청: HSteamNetConnection : {serverConn}");
				connectionStatusChangedCallback = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnClientConnectionStatusChanged);
			}
		}
		GUI.enabled = true;
		GUI.enabled = isConnectedToServer;
		if (GUILayout.Button("Stop : CloseConnection"))
		{
			SteamNetworkingSockets.CloseConnection(serverConn, 0, "사용자 종료", bEnableLinger: false);
			serverConn = HSteamNetConnection.Invalid;
			if (connectionStatusChangedCallback != null)
			{
				connectionStatusChangedCallback.Unregister();
				connectionStatusChangedCallback = null;
			}
			isConnectedToServer = false;
		}
		GUI.enabled = true;
		GUI.enabled = isConnectedToServer;
		if (GUILayout.Button("SendMessageToServer"))
		{
			SendMessageToServer("SendMessageToServer : Hello, Server! Im " + SteamUser.GetSteamID().ToString());
		}
		GUI.enabled = true;
		GUI.enabled = isConnectedToServer;
		if (GUILayout.Button("SendMessageToServer x 3"))
		{
			SendMessageToServer("SendMessageToServer : Hello, Server! 1 Im " + SteamUser.GetSteamID().ToString());
			SendMessageToServer("SendMessageToServer : Hello, Server! 2 Im " + SteamUser.GetSteamID().ToString());
			SendMessageToServer("SendMessageToServer : Hello, Server! 3 Im " + SteamUser.GetSteamID().ToString());
		}
		GUI.enabled = true;
		GUILayout.EndVertical();
		GUILayout.Label("");
		SteamRelayNetworkStatus_t pDetails;
		ESteamNetworkingAvailability relayNetworkStatus = SteamNetworkingUtils.GetRelayNetworkStatus(out pDetails);
		GUILayout.Label("SteamNetworkingUtils.GetRelayNetworkStatus : " + relayNetworkStatus.ToString() + Environment.NewLine + " + Avail : " + pDetails.m_eAvail.ToString() + Environment.NewLine + " + AvailNetworkConfig : " + pDetails.m_eAvailNetworkConfig.ToString() + Environment.NewLine + " + AvailAnyRelay : " + pDetails.m_eAvailAnyRelay.ToString() + Environment.NewLine + " + PingMeasurementInProgress : " + pDetails.m_bPingMeasurementInProgress);
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}

	private void OnConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t callback)
	{
		switch (callback.m_info.m_eState)
		{
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
			MonoBehaviour.print("클라이언트가 연결 시도 중...");
			HandleNewConnection(callback.m_hConn);
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_FindingRoute:
			MonoBehaviour.print("클라이언트가 연결 루트 찾는 중...");
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
			MonoBehaviour.print("클라이언트와 연결됨");
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
			MonoBehaviour.print("연결 종료 또는 문제 발생");
			connectedClients.Remove(callback.m_hConn);
			SteamNetworkingSockets.CloseConnection(callback.m_hConn, 0, "Disconnected", bEnableLinger: false);
			break;
		default:
			MonoBehaviour.print($"알 수 없는 상태: {callback.m_info.m_eState}");
			break;
		}
	}

	private void HandleNewConnection(HSteamNetConnection connection)
	{
		if (SteamNetworkingSockets.AcceptConnection(connection) == EResult.k_EResultOK)
		{
			MonoBehaviour.print("연결 수락 성공");
			if (SteamNetworkingSockets.SetConnectionPollGroup(connection, pollGroup))
			{
				connectedClients.Add(connection);
				MonoBehaviour.print($"PollGroup에 연결 추가 완료: {connection}");
			}
			else
			{
				MonoBehaviour.print("PollGroup에 연결 추가 실패");
			}
		}
		else
		{
			MonoBehaviour.print("연결 수락 실패");
			SteamNetworkingSockets.CloseConnection(connection, 0, "연결 수락 실패", bEnableLinger: false);
		}
	}

	private void ServerReceiveMessages()
	{
		IntPtr[] array = new IntPtr[16];
		int num = SteamNetworkingSockets.ReceiveMessagesOnPollGroup(pollGroup, array, array.Length);
		if (num > 0)
		{
			MonoBehaviour.print($"클라이언트로부터 {num}개의 메시지 수신");
			for (int i = 0; i < num; i++)
			{
				_ = array[i];
				SteamNetworkingMessage_t steamNetworkingMessage_t = SteamNetworkingMessage_t.FromIntPtr(array[i]);
				byte[] bytes = IntPtrToByteArray(steamNetworkingMessage_t.m_pData, steamNetworkingMessage_t.m_cbSize);
				string text = Encoding.UTF8.GetString(bytes);
				MonoBehaviour.print("수신된 메시지: " + text);
				BroadcastMessageToClients(text);
				SteamNetworkingMessage_t.Release(array[i]);
			}
		}
	}

	private void BroadcastMessageToClients(string message)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(message);
		IntPtr intPtr = ByteArrayToIntPtr(bytes);
		foreach (HSteamNetConnection connectedClient in connectedClients)
		{
			long pOutMessageNumber;
			EResult eResult = SteamNetworkingSockets.SendMessageToConnection(connectedClient, intPtr, (uint)bytes.Length, 8, out pOutMessageNumber);
			if (eResult != EResult.k_EResultOK)
			{
				MonoBehaviour.print($"메시지 전송 실패: {connectedClient}, 결과: {eResult}");
			}
		}
		FreeIntPtr(intPtr);
		MonoBehaviour.print("브로드캐스트 완료");
	}

	private void OnClientConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t callback)
	{
		switch (callback.m_info.m_eState)
		{
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
			MonoBehaviour.print("서버와 연결 중...");
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
			MonoBehaviour.print("서버와 연결 성공");
			isConnectedToServer = true;
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_FindingRoute:
			MonoBehaviour.print("서버와 연결 루트 찾는 중");
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
			MonoBehaviour.print("서버에 의해 연결 종료");
			isConnectedToServer = false;
			SteamNetworkingSockets.CloseConnection(serverConn, 0, "서버 종료", bEnableLinger: false);
			if (connectionStatusChangedCallback != null)
			{
				connectionStatusChangedCallback.Unregister();
				connectionStatusChangedCallback = null;
			}
			break;
		case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
			MonoBehaviour.print("연결 문제 발생: " + callback.m_info.m_szEndDebug);
			isConnectedToServer = false;
			SteamNetworkingSockets.CloseConnection(serverConn, 0, "문제 발생", bEnableLinger: false);
			if (connectionStatusChangedCallback != null)
			{
				connectionStatusChangedCallback.Unregister();
				connectionStatusChangedCallback = null;
			}
			break;
		default:
			MonoBehaviour.print($"알 수 없는 상태: {callback.m_info.m_eState}");
			break;
		}
	}

	private void ClientReceiveMessages()
	{
		if (!isConnectedToServer)
		{
			return;
		}
		IntPtr[] array = new IntPtr[10];
		int num = SteamNetworkingSockets.ReceiveMessagesOnConnection(serverConn, array, array.Length);
		if (num > 0)
		{
			MonoBehaviour.print($"서버로부터 {num}개의 메시지 수신");
			for (int i = 0; i < num; i++)
			{
				_ = array[i];
				SteamNetworkingMessage_t steamNetworkingMessage_t = Marshal.PtrToStructure<SteamNetworkingMessage_t>(array[i]);
				byte[] bytes = IntPtrToByteArray(steamNetworkingMessage_t.m_pData, steamNetworkingMessage_t.m_cbSize);
				string text = Encoding.UTF8.GetString(bytes);
				MonoBehaviour.print("수신된 메시지: " + text);
				SteamNetworkingMessage_t.Release(array[i]);
			}
		}
	}

	public void SendMessageToServer(string message)
	{
		if (!isConnectedToServer)
		{
			MonoBehaviour.print("서버와 연결되지 않았습니다. 메시지를 전송할 수 없습니다.");
			return;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(message);
		IntPtr intPtr = ByteArrayToIntPtr(bytes);
		long pOutMessageNumber;
		EResult eResult = SteamNetworkingSockets.SendMessageToConnection(serverConn, intPtr, (uint)bytes.Length, 8, out pOutMessageNumber);
		FreeIntPtr(intPtr);
		if (eResult == EResult.k_EResultOK)
		{
			MonoBehaviour.print("서버로 메시지 전송 성공");
		}
		else
		{
			MonoBehaviour.print($"서버로 메시지 전송 실패: {eResult}");
		}
	}

	public static IntPtr ByteArrayToIntPtr(byte[] byteArray)
	{
		if (byteArray == null || byteArray.Length == 0)
		{
			return IntPtr.Zero;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(byteArray.Length);
		Marshal.Copy(byteArray, 0, intPtr, byteArray.Length);
		return intPtr;
	}

	public static byte[] IntPtrToByteArray(IntPtr ptr, int length)
	{
		if (ptr == IntPtr.Zero || length <= 0)
		{
			return Array.Empty<byte>();
		}
		byte[] array = new byte[length];
		Marshal.Copy(ptr, array, 0, length);
		return array;
	}

	public static void FreeIntPtr(IntPtr ptr)
	{
		if (ptr != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(ptr);
		}
	}

	public static string GetSteamNetConnectionInfoAllString(SteamNetConnectionInfo_t connectionInfo)
	{
		connectionInfo.m_identityRemote.ToString(out var buf);
		connectionInfo.m_addrRemote.ToString(out var buf2, bWithPort: true);
		return connectionInfo.m_szConnectionDescription + Environment.NewLine + "Remote Identity: " + buf + Environment.NewLine + $"User Data: {connectionInfo.m_nUserData}" + Environment.NewLine + $"Connection State: {connectionInfo.m_eState}" + Environment.NewLine + $"End Reason: {connectionInfo.m_eEndReason}" + Environment.NewLine + "End Debug: " + connectionInfo.m_szEndDebug + Environment.NewLine + "Remote Address: " + buf2 + Environment.NewLine + $"POPID Remote: {connectionInfo.m_idPOPRemote}" + Environment.NewLine + $"POPID Relay: {connectionInfo.m_idPOPRelay}";
	}
}
