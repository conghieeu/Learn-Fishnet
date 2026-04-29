using System.IO;
using Steamworks;
using UnityEngine;

public class SteamNetworkingTest : MonoBehaviour
{
	private enum MsgType : uint
	{
		Ping = 0u,
		Ack = 1u
	}

	private Vector2 m_ScrollPos;

	private CSteamID m_RemoteSteamId;

	protected Callback<P2PSessionRequest_t> m_P2PSessionRequest;

	protected Callback<P2PSessionConnectFail_t> m_P2PSessionConnectFail;

	protected Callback<SocketStatusCallback_t> m_SocketStatusCallback;

	public void OnEnable()
	{
		m_RemoteSteamId = new CSteamID(76561197961846750uL);
		m_P2PSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
		m_P2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
		m_SocketStatusCallback = Callback<SocketStatusCallback_t>.Create(OnSocketStatusCallback);
	}

	private void OnDisable()
	{
		if (!m_RemoteSteamId.IsValid())
		{
			SteamNetworking.CloseP2PSessionWithUser(m_RemoteSteamId);
		}
	}

	public void RenderOnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width - 200, 0f, 200f, Screen.height));
		GUILayout.Label("Variables:");
		CSteamID remoteSteamId = m_RemoteSteamId;
		GUILayout.Label("m_RemoteSteamId: " + remoteSteamId.ToString());
		GUILayout.EndArea();
		GUILayout.BeginVertical("box");
		m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(Screen.width - 215), GUILayout.Height(Screen.height - 33));
		if (!m_RemoteSteamId.IsValid())
		{
			GUILayout.Label("Please fill m_RemoteSteamId with a valid 64bit SteamId to use SteamNetworkingTest.");
			GUILayout.Label("Alternatively it will be filled automatically when a session request is recieved.");
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			return;
		}
		if (GUILayout.Button("SendP2PPacket(m_RemoteSteamId, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable)"))
		{
			byte[] array = new byte[4];
			using (MemoryStream output = new MemoryStream(array))
			{
				using BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(0u);
			}
			bool flag = SteamNetworking.SendP2PPacket(m_RemoteSteamId, array, (uint)array.Length, EP2PSend.k_EP2PSendReliable);
			string[] obj = new string[10] { "SteamNetworking.SendP2PPacket(", null, null, null, null, null, null, null, null, null };
			remoteSteamId = m_RemoteSteamId;
			obj[1] = remoteSteamId.ToString();
			obj[2] = ", ";
			obj[3] = array?.ToString();
			obj[4] = ", ";
			obj[5] = ((uint)array.Length).ToString();
			obj[6] = ", ";
			obj[7] = EP2PSend.k_EP2PSendReliable.ToString();
			obj[8] = ") : ";
			obj[9] = flag.ToString();
			MonoBehaviour.print(string.Concat(obj));
		}
		bool flag2 = SteamNetworking.IsP2PPacketAvailable(out var pcubMsgSize);
		GUILayout.Label("IsP2PPacketAvailable(out MsgSize) : " + flag2 + " -- " + pcubMsgSize);
		GUI.enabled = flag2;
		if (GUILayout.Button("ReadP2PPacket(bytes, MsgSize, out newMsgSize, out SteamIdRemote)"))
		{
			byte[] array2 = new byte[pcubMsgSize];
			flag2 = SteamNetworking.ReadP2PPacket(array2, pcubMsgSize, out var pcubMsgSize2, out var psteamIDRemote);
			using MemoryStream input = new MemoryStream(array2);
			using BinaryReader binaryReader = new BinaryReader(input);
			MsgType msgType = (MsgType)binaryReader.ReadUInt32();
			string[] obj2 = new string[10]
			{
				"SteamNetworking.ReadP2PPacket(bytes, ",
				pcubMsgSize.ToString(),
				", out newMsgSize, out SteamIdRemote) - ",
				flag2.ToString(),
				" -- ",
				pcubMsgSize2.ToString(),
				" -- ",
				null,
				null,
				null
			};
			remoteSteamId = psteamIDRemote;
			obj2[7] = remoteSteamId.ToString();
			obj2[8] = " -- ";
			obj2[9] = msgType.ToString();
			MonoBehaviour.print(string.Concat(obj2));
		}
		GUI.enabled = true;
		if (GUILayout.Button("CloseP2PSessionWithUser(m_RemoteSteamId)"))
		{
			bool flag3 = SteamNetworking.CloseP2PSessionWithUser(m_RemoteSteamId);
			remoteSteamId = m_RemoteSteamId;
			MonoBehaviour.print("SteamNetworking.CloseP2PSessionWithUser(" + remoteSteamId.ToString() + ") : " + flag3);
		}
		if (GUILayout.Button("CloseP2PChannelWithUser(m_RemoteSteamId, 0)"))
		{
			bool flag4 = SteamNetworking.CloseP2PChannelWithUser(m_RemoteSteamId, 0);
			string[] obj3 = new string[6] { "SteamNetworking.CloseP2PChannelWithUser(", null, null, null, null, null };
			remoteSteamId = m_RemoteSteamId;
			obj3[1] = remoteSteamId.ToString();
			obj3[2] = ", ";
			obj3[3] = 0.ToString();
			obj3[4] = ") : ";
			obj3[5] = flag4.ToString();
			MonoBehaviour.print(string.Concat(obj3));
		}
		GUILayout.Label("GetP2PSessionState(m_RemoteSteamId, out ConnectionState) : " + SteamNetworking.GetP2PSessionState(m_RemoteSteamId, out var pConnectionState) + " -- " + pConnectionState);
		if (GUILayout.Button("AllowP2PPacketRelay(true)"))
		{
			bool flag5 = SteamNetworking.AllowP2PPacketRelay(bAllow: true);
			MonoBehaviour.print("SteamNetworking.AllowP2PPacketRelay(" + true + ") : " + flag5);
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}

	private void OnP2PSessionRequest(P2PSessionRequest_t pCallback)
	{
		string text = 1202.ToString();
		CSteamID steamIDRemote = pCallback.m_steamIDRemote;
		Debug.Log("[" + text + " - P2PSessionRequest] - " + steamIDRemote.ToString());
		bool flag = SteamNetworking.AcceptP2PSessionWithUser(pCallback.m_steamIDRemote);
		steamIDRemote = pCallback.m_steamIDRemote;
		MonoBehaviour.print("SteamNetworking.AcceptP2PSessionWithUser(" + steamIDRemote.ToString() + ") - " + flag);
		m_RemoteSteamId = pCallback.m_steamIDRemote;
	}

	private void OnP2PSessionConnectFail(P2PSessionConnectFail_t pCallback)
	{
		string[] obj = new string[6]
		{
			"[",
			1203.ToString(),
			" - P2PSessionConnectFail] - ",
			null,
			null,
			null
		};
		CSteamID steamIDRemote = pCallback.m_steamIDRemote;
		obj[3] = steamIDRemote.ToString();
		obj[4] = " -- ";
		obj[5] = pCallback.m_eP2PSessionError.ToString();
		Debug.Log(string.Concat(obj));
	}

	private void OnSocketStatusCallback(SocketStatusCallback_t pCallback)
	{
		string[] obj = new string[10]
		{
			"[",
			1201.ToString(),
			" - SocketStatusCallback] - ",
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		SNetSocket_t hSocket = pCallback.m_hSocket;
		obj[3] = hSocket.ToString();
		obj[4] = " -- ";
		SNetListenSocket_t hListenSocket = pCallback.m_hListenSocket;
		obj[5] = hListenSocket.ToString();
		obj[6] = " -- ";
		CSteamID steamIDRemote = pCallback.m_steamIDRemote;
		obj[7] = steamIDRemote.ToString();
		obj[8] = " -- ";
		obj[9] = pCallback.m_eSNetSocketState.ToString();
		Debug.Log(string.Concat(obj));
	}
}
