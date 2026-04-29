using System;
using Dissonance;
using Dissonance.Integrations.FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using FishNet.Transporting;
using Mimic.Actors;
using Steamworks;
using UnityEngine;

namespace Mimic.Voice
{
	public class FishNetDissonancePlayer : NetworkBehaviour, IDissonancePlayer
	{
		public readonly SyncVar<string> syncedCommsPlayerName = new SyncVar<string>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));

		public readonly SyncVar<long> syncedPlayerUID = new SyncVar<long>(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));

		private ProtoActor? protoActorCache;

		public string steamID;

		private bool NetworkInitialize___EarlyMimic_002EVoice_002EFishNetDissonancePlayerAssembly_002DCSharp_002Edll_Excuted;

		private bool NetworkInitialize__LateMimic_002EVoice_002EFishNetDissonancePlayerAssembly_002DCSharp_002Edll_Excuted;

		public string PlayerId => syncedCommsPlayerName.Value;

		public Vector3 Position
		{
			get
			{
				if (this != null && base.transform != null)
				{
					return base.transform.position;
				}
				return Vector3.zero;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				if (this != null && base.transform != null)
				{
					return base.transform.rotation;
				}
				return Quaternion.identity;
			}
		}

		public NetworkPlayerType Type
		{
			get
			{
				if (!base.IsOwner)
				{
					return NetworkPlayerType.Remote;
				}
				return NetworkPlayerType.Local;
			}
		}

		public bool IsTracking { get; private set; }

		public long PlayerUID => syncedPlayerUID.Value;

		private DissonanceComms comms => DissonanceFishNetComms.Instance.Comms;

		public ProtoActor? ProtoActorCache => protoActorCache;

		public virtual void Awake()
		{
			NetworkInitialize___Early();
			Awake_UserLogic_Mimic_002EVoice_002EFishNetDissonancePlayer_Assembly_002DCSharp_002Edll();
			NetworkInitialize__Late();
		}

		public override void OnOwnershipClient(NetworkConnection prevOwner)
		{
			base.OnOwnershipClient(prevOwner);
			if (prevOwner == null || !base.IsOwner)
			{
				return;
			}
			if (comms == null)
			{
				Logger.RError("Could not find any DissonanceComms instance! This DissonancePlayer instance will not work!");
				return;
			}
			comms.LocalPlayerNameChanged += SetPlayerName;
			if (comms.LocalPlayerName == null)
			{
				string localPlayerName = Guid.NewGuid().ToString();
				comms.LocalPlayerName = localPlayerName;
			}
			else
			{
				SetPlayerName(comms.LocalPlayerName);
			}
		}

		private void SetPlayerName(string playerName)
		{
			if (base.IsOwner)
			{
				if (Hub.s == null || Hub.s.pdata == null || !Hub.s.pdata.SessionJoined)
				{
					Logger.RError("Failed to get Hub.s.pdata.joinServerRes! Cannot set player name!");
				}
				else
				{
					ServerRpcSetPlayerName(playerName, Hub.s.pdata.PlayerUID);
				}
			}
		}

		[ServerRpc(RunLocally = true, RequireOwnership = true)]
		private void ServerRpcSetPlayerName(string playerName, long playerUID)
		{
			RpcWriter___Server_ServerRpcSetPlayerName_3727347235(playerName, playerUID);
			RpcLogic___ServerRpcSetPlayerName_3727347235(playerName, playerUID);
		}

		private void ManageTrackingState(bool track)
		{
			if (IsTracking == track || DissonanceFishNetComms.Instance == null || (track && !DissonanceFishNetComms.Instance.IsInitialized))
			{
				return;
			}
			if (comms == null)
			{
				Logger.RError("Could not find any DissonanceComms instance! This DissonancePlayer instance will not work!");
				return;
			}
			if (track)
			{
				comms.TrackPlayerPosition(this);
			}
			else
			{
				comms.StopTracking(this);
			}
			IsTracking = track;
		}

		private void OnChangePlayerName(string prev, string next, bool asServer)
		{
			ManageTrackingState(track: true);
		}

		private void Update()
		{
			if (IsTracking && !(Hub.s == null) && !(Hub.s.pdata.main == null))
			{
				protoActorCache = ((protoActorCache != null) ? protoActorCache : Hub.s.pdata.main.GetActorByPlayerUID(syncedPlayerUID.Value));
				if (protoActorCache != null && base.transform != null && protoActorCache.transform != null)
				{
					base.transform.position = protoActorCache.transform.position;
					base.transform.rotation = protoActorCache.transform.rotation;
				}
			}
		}

		public virtual void NetworkInitialize___Early()
		{
			if (!NetworkInitialize___EarlyMimic_002EVoice_002EFishNetDissonancePlayerAssembly_002DCSharp_002Edll_Excuted)
			{
				NetworkInitialize___EarlyMimic_002EVoice_002EFishNetDissonancePlayerAssembly_002DCSharp_002Edll_Excuted = true;
				syncedPlayerUID.InitializeEarly(this, 1u, isSyncObject: false);
				syncedCommsPlayerName.InitializeEarly(this, 0u, isSyncObject: false);
				RegisterServerRpc(0u, RpcReader___Server_ServerRpcSetPlayerName_3727347235);
			}
		}

		public virtual void NetworkInitialize__Late()
		{
			if (!NetworkInitialize__LateMimic_002EVoice_002EFishNetDissonancePlayerAssembly_002DCSharp_002Edll_Excuted)
			{
				NetworkInitialize__LateMimic_002EVoice_002EFishNetDissonancePlayerAssembly_002DCSharp_002Edll_Excuted = true;
				syncedPlayerUID.InitializeLate();
				syncedCommsPlayerName.InitializeLate();
			}
		}

		public override void NetworkInitializeIfDisabled()
		{
			NetworkInitialize___Early();
			NetworkInitialize__Late();
		}

		private void RpcWriter___Server_ServerRpcSetPlayerName_3727347235(string playerName, long playerUID)
		{
			if (!base.IsClientInitialized)
			{
				NetworkManager networkManager = base.NetworkManager;
				networkManager.LogWarning("Cannot complete action because client is not active. This may also occur if the object is not yet initialized, has deinitialized, or if it does not contain a NetworkObject component.");
				return;
			}
			if (!base.IsOwner)
			{
				NetworkManager networkManager2 = base.NetworkManager;
				networkManager2.LogWarning("Cannot complete action because you are not the owner of this object. .");
				return;
			}
			Channel channel = Channel.Reliable;
			PooledWriter pooledWriter = WriterPool.Retrieve();
			pooledWriter.WriteString(playerName);
			pooledWriter.WriteInt64(playerUID);
			SendServerRpc(0u, pooledWriter, channel, DataOrderType.Default);
			pooledWriter.Store();
		}

		private void RpcLogic___ServerRpcSetPlayerName_3727347235(string playerName, long playerUID)
		{
			if (IsTracking)
			{
				ManageTrackingState(track: false);
			}
			syncedCommsPlayerName.Value = playerName;
			syncedPlayerUID.Value = playerUID;
			ManageTrackingState(track: true);
		}

		private void RpcReader___Server_ServerRpcSetPlayerName_3727347235(PooledReader PooledReader0, Channel channel, NetworkConnection conn)
		{
			string playerName = PooledReader0.ReadString();
			long playerUID = PooledReader0.ReadInt64();
			if (base.IsServerInitialized && OwnerMatches(conn) && !conn.IsLocalClient)
			{
				RpcLogic___ServerRpcSetPlayerName_3727347235(playerName, playerUID);
			}
		}

		private void Awake_UserLogic_Mimic_002EVoice_002EFishNetDissonancePlayer_Assembly_002DCSharp_002Edll()
		{
			syncedCommsPlayerName.OnChange += OnChangePlayerName;
			if (SteamManager.Initialized)
			{
				steamID = SteamUser.GetSteamID().ToString();
			}
		}
	}
}
