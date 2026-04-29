using System;
using System.Collections;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public class ServerListInformer : IDisposable
{
	public Action hostInfoRefreshComplete;

	public bool hostListResponseError;

	private bool hostListRequested;

	private RUDPClientSession session;

	private SDRClientSession relaySession;

	public NetworkManagerState networkState;

	private string availableAddressOrSteamId = "";

	private int availablePort;

	public void UpdateHostInfo()
	{
		if (!hostListRequested)
		{
			hostListRequested = true;
		}
	}

	private void OnRecvPacket(IMsg msg)
	{
	}

	private void OnConnect()
	{
		networkState = NetworkManagerState.Connected;
	}

	private void OnClose(DisconnectReason reason)
	{
		if (reason != DisconnectReason.ByClient)
		{
			networkState = NetworkManagerState.Disconnected;
			availableAddressOrSteamId = "";
			availablePort = -1;
		}
	}

	public IEnumerator TryConnect(string addressOrSteamId, int port, bool steamRelay)
	{
		networkState = NetworkManagerState.Connecting;
		availableAddressOrSteamId = addressOrSteamId;
		availablePort = port;
		if (steamRelay)
		{
			if (relaySession == null)
			{
				relaySession = new SDRClientSession(OnRecvPacket, OnConnect, OnClose);
			}
			relaySession.Connect(new SteamIDConnection
			{
				SteamID = ulong.Parse(addressOrSteamId),
				Port = port
			});
		}
		else
		{
			if (session == null)
			{
				session = new RUDPClientSession(OnRecvPacket, OnConnect, OnClose);
			}
			session.Connect(new IPAddrConnection
			{
				IPAddr = addressOrSteamId,
				Port = port
			});
		}
		yield return new WaitUntil(() => networkState != NetworkManagerState.Connecting);
		if (steamRelay)
		{
			if (networkState == NetworkManagerState.Connected)
			{
				relaySession.Close(DisconnectReason.ByClient);
				yield break;
			}
			relaySession.Close(DisconnectReason.ByClient);
			networkState = NetworkManagerState.NotConnected;
		}
		else if (networkState == NetworkManagerState.Connected)
		{
			session.Close(DisconnectReason.ByClient);
		}
		else
		{
			session.Close(DisconnectReason.ByClient);
			networkState = NetworkManagerState.NotConnected;
		}
	}

	public void Update()
	{
		if (session != null)
		{
			session.Update();
		}
		if (relaySession != null)
		{
			relaySession.Update();
		}
	}

	public void Dispose()
	{
		session?.Dispose();
		session = null;
		relaySession?.Dispose();
		relaySession = null;
	}
}
