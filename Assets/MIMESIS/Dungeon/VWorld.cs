using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Bifrost.Faction;
using Bifrost.TramupgradeData;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluProtocol.Enum;
using ReluReplay;
using ReluServerBase.Threading;
using UnityEngine;

public class VWorld
{
	public delegate MsgErrorCode AdminCommandFunc(VPlayer player, AdminCommandArgs args);

	private Dictionary<string, AdminCommandFunc> AdminCommandFuncs = new Dictionary<string, AdminCommandFunc>();

	private EventTimer _eventTimer;

	private CommandExecutor _commandExecutor;

	private RUDPServer? _rudpServer;

	private SDRServer? _sdrServer;

	private ServerDispatchManager _serverDispatchers = new ServerDispatchManager();

	private SessionManager _sessionManager;

	private VRoomManager _vRoomManager;

	private bool _initialized;

	private long _tempPlayerUID;

	private long _tempSteamID;

	private long _lastTick;

	private int _decoratorGenerator;

	private readonly string Version;

	public VRoomManager VRoomManager => _vRoomManager;

	private ReplayManager _replayManager => Hub.s.replayManager;

	public bool Running => _initialized;

	public int SaveSlotID { get; set; }

	public AdminCommandFunc? GetAdminCommand(string cmdName)
	{
		if (AdminCommandFuncs.TryGetValue(cmdName.ToLower() ?? "", out var value))
		{
			return value;
		}
		return null;
	}

	public void RegisterAdminProtocol()
	{
		RegisterCommand();
		RegisterPlayerDispatcher(delegate(VPlayer player, AdminCommandReq msg)
		{
			try
			{
				if (AdminCommandFuncs.TryGetValue(msg.command, out var value))
				{
					MsgErrorCode errorCode = value(player, new AdminCommandArgs(msg.args));
					player.SendToMe(new AdminCommandRes(msg.hashCode)
					{
						errorCode = errorCode
					});
				}
				else
				{
					player.SendToMe(new AdminCommandRes(msg.hashCode)
					{
						errorCode = MsgErrorCode.InvalidAdminCommandArgument
					});
				}
			}
			catch (Exception e)
			{
				Logger.RError(e);
			}
		});
	}

	public void RegisterCommand()
	{
		AdminCommandFuncs.Add("changestat", AdminCommandChangeImmutableStat);
		AdminCommandFuncs.Add("resetstat", AdminCommandResetStat);
		AdminCommandFuncs.Add("kill", AdminCommandKill);
		AdminCommandFuncs.Add("releaseitem", AdminCommandReleaseItem);
		AdminCommandFuncs.Add("decide", AdminCommandDungeonRoomDecide);
		AdminCommandFuncs.Add("addcurrency", AdminCommandAddCurrency);
		AdminCommandFuncs.Add("buyitem", AdminCommandBuyItem);
		AdminCommandFuncs.Add("sessiondecide", AdminCommandSessionDecide);
		AdminCommandFuncs.Add("endaftertime", AdminCommandOverwriteDungeonEndTime);
		AdminCommandFuncs.Add("turnonlantern", AdminCommandTurnOnLantern);
		AdminCommandFuncs.Add("addabnormal", AdminCommandAddAbnormal);
		AdminCommandFuncs.Add("spawnmonster", AdminCommandSpawnMonster);
		AdminCommandFuncs.Add("spawnitem", AdminCommandSpawnItem);
		AdminCommandFuncs.Add("setvisible", AdminCommandSetVisible);
		AdminCommandFuncs.Add("killallmonster", AdminCommandKillAllMonster);
		AdminCommandFuncs.Add("toggledebug", AdminCommandToggleDrawDebug);
		AdminCommandFuncs.Add("spawnfieldskill", AdminCommandSpawnFieldSkill);
		AdminCommandFuncs.Add("changeweather", AdminCommandChangeWeather);
		AdminCommandFuncs.Add("addweatherschedule", AdminCommandAddWeatherSchedule);
		AdminCommandFuncs.Add("addgametime", AdminCommandAddGametime);
		AdminCommandFuncs.Add("spawnprojectile", AdminCommandSpawnProjectile);
		AdminCommandFuncs.Add("spawnfieldskillmapmarker", AdminCommandSpawnFieldSkillMapMarker);
		AdminCommandFuncs.Add("spawnhold", AdminCommandSpawnHold);
		AdminCommandFuncs.Add("recoverconta", AdminCommandRemoveConta);
		AdminCommandFuncs.Add("addconta", AdminCommandAddConta);
		AdminCommandFuncs.Add("recoverhp", AdminCommandRecoverHP);
		AdminCommandFuncs.Add("changehp", AdminCommandChangeCurrentHP);
		AdminCommandFuncs.Add("resetdungeontime", AdminCommandResetDungeonTime);
		AdminCommandFuncs.Add("detachforce", AdminCommandDetachForce);
		AdminCommandFuncs.Add("changedaycount", AdminCommandChangeDayCount);
		AdminCommandFuncs.Add("changecycle", AdminCommandChangeCycle);
		AdminCommandFuncs.Add("killallplayer", AdminCommandKillAllPlayer);
		AdminCommandFuncs.Add("suicide", AdminCommandSuicide);
		AdminCommandFuncs.Add("alterconta", AdminCommandAlterConta);
		AdminCommandFuncs.Add("reloadai", AdminCommandReloadAI);
		AdminCommandFuncs.Add("addaura", AdminCommandAddAura);
		AdminCommandFuncs.Add("hbdisable", AdminCommandDisableHeartBeatCheck);
		AdminCommandFuncs.Add("addfaction", AdminCommandAddFaction);
		AdminCommandFuncs.Add("removefaction", AdminCommandRemoveFaction);
		AdminCommandFuncs.Add("stopai", AdminCommandStopAI);
		AdminCommandFuncs.Add("enableaggrolog", AdminCommandEnableAggroLogging);
		AdminCommandFuncs.Add("startscrapmotion", AdminCommandStartScrapMotion);
		AdminCommandFuncs.Add("endscrapmotion", AdminCommandEndScrapMotion);
		AdminCommandFuncs.Add("startrepairtram", AdminCommandPlayTramRepairCutSceneForDebug);
		AdminCommandFuncs.Add("teleport", AdminCommandTeleportToPos);
		AdminCommandFuncs.Add("savegamedata", AdminCommandSaveGameData);
		AdminCommandFuncs.Add("reinforceitem", AdminCommandReinforceItem);
		AdminCommandFuncs.Add("addtramupgrade", AdminCommandAddTramUpgrade);
		AdminCommandFuncs.Add("removetramupgrade", AdminCommandRemoveTramUpgrade);
		AdminCommandFuncs.Add("alltramupgrade", AdminCommandAllTramUpgrade);
		AdminCommandFuncs.Add("resettramupgrade", AdminCommandResetTramUpgrade);
	}

	public void RegisterAuthPktDispatcher()
	{
		RegisterClientSessionDispatcher(delegate(SessionContext ctx, JoinServerReq msg)
		{
			if (msg.clientVersion != Version)
			{
				ctx.Send(new JoinServerRes(msg.hashCode)
				{
					errorCode = MsgErrorCode.VersionMismatch
				});
			}
			else
			{
				ctx.Login(GetTempPlayerUID(), msg.guid, (msg.steamID == 0L) ? GetTempSteamID() : msg.steamID, msg.nickName, msg.voiceUID, msg.isHost, msg.hashCode);
			}
		});
		RegisterClientSessionDispatcher(delegate(SessionContext ctx, HeartBeatReq msg)
		{
			_sessionManager.SetNetworkGrade(ctx, msg.networkGrade);
			ctx.Send(new HeartBeatRes(msg.hashCode)
			{
				clientSendTime = msg.clientSendTime,
				seqID = msg.seqID
			});
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, RelayPacket msg)
		{
			player.SendToChannel(msg);
		});
		RegisterClientSessionDispatcher(delegate(SessionContext ctx, EnterWaitingRoomReq msg)
		{
			_vRoomManager.EnterWaitingRoom(ctx, msg.hashCode);
		});
		RegisterClientSessionDispatcher(delegate(SessionContext ctx, EnterMaintenanceRoomReq msg)
		{
			_vRoomManager.EnterMaintenenceRoom(ctx, msg.hashCode);
		});
		RegisterClientSessionDispatcher(delegate(SessionContext ctx, EnterDungeonReq msg)
		{
			_vRoomManager.EnterDungeon(ctx, msg.hashCode, msg.roomID);
		});
		RegisterClientSessionDispatcher(delegate(SessionContext ctx, EnterDeathMatchRoomReq msg)
		{
			_vRoomManager.EnterDeathMatchRoom(ctx, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, LevelLoadCompleteReq msg)
		{
			player.HandleLevelLoadComplete(msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, StartGameReq msg)
		{
			player.HandleStartGame(msg.hashCode, msg.selectedDungeonMasterID).HandleError<StartGameRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, StartSessionReq msg)
		{
			player.HandleStartSession(msg.hashCode).HandleError<StartSessionRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, EndSessionReq msg)
		{
			player.HandleEndSession(msg.hashCode).HandleError<EndSessionRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, SaveGameDataReq msg)
		{
			player.HandleSaveGameDataReq(msg.SlotID, msg.PlayerNames, msg.hashCode).HandleError<SaveGameDataRes>(player, msg.hashCode);
		});
	}

	public void RegisterItemDispatcher()
	{
		RegisterPlayerDispatcher(delegate(VPlayer player, GrapLootingObjectReq msg)
		{
			player.HandleGrapLootingObject(msg.lootingObjectID, msg.hashCode).HandleError<GrapLootingObjectRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, ReleaseItemReq msg)
		{
			player.HandleReleaseItem(msg.hashCode).HandleError<ReleaseItemRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, ChangeActiveInvenSlotReq msg)
		{
			player.InventoryControlUnit.HandleChangeActiveInvenSlot(msg.slotIndex, sync: true, msg.hashCode).HandleError<ChangeActiveInvenSlotRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, BuyItemReq msg)
		{
			player.HandleBuyItem(msg.itemMasterID, msg.hashCode, msg.machineIndex).HandleError<BuyItemRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, PutIntoToiletReq msg)
		{
			player.HandlePutIntoToilet(msg.hashCode).HandleError<PutIntoToiletRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, HangItemReq msg)
		{
			player.HandleHangItem(msg.index, msg.hashCode).HandleError<HangItemRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, UnhangItemReq msg)
		{
			player.HandleUnhangItem(msg.index, msg.hashCode).HandleError<UnhangItemRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, ChangeEquipStatusReq msg)
		{
			player.InventoryControlUnit.ChangeEquipStatus(msg.isTurnOn, sync: true, msg.hashCode).HandleError<ChangeEquipStatusRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, UseItemReq msg)
		{
			player.InventoryControlUnit.UseItem(msg.hashCode, sync: true).HandleError<UseItemRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, BarterItemReq msg)
		{
			player.InventoryControlUnit.BarterItem(msg.hashCode).HandleError<BarterItemRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, StartScrapMotionReq msg)
		{
			player.InventoryControlUnit.StartScrapMotion(msg.basePosition, msg.hashCode, sync: true).HandleError<StartScrapMotionRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, EndScrapMotionReq msg)
		{
			player.InventoryControlUnit.EndScrapMotion(msg.basePosition, msg.hashCode, sync: true).HandleError<EndScrapMotionRes>(player, msg.hashCode);
		});
	}

	public void RegisterPlayerPktDispatcher()
	{
		RegisterPlayerDispatcher(delegate(VPlayer player, MoveStartReq msg)
		{
			player.MovementControlUnit.DirectMoveStart(msg).HandleError<MoveStartRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, MoveStopReq msg)
		{
			player.MovementControlUnit.DirectMoveStop(msg);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, ChangeViewPointReq msg)
		{
			player.MovementControlUnit.HandleChangeViewPoint(msg);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, UseLevelObjectReq msg)
		{
			player.HandleLevelObject(msg.levelObjectID, msg.state, msg.occupy, msg.hashCode).HandleError<UseLevelObjectRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, ToggleSprintReq msg)
		{
			player.MovementControlUnit.SetSprint(msg.isSprint, msg.hashCode).HandleError<ToggleSprintRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, SyncSkillMoveReq msg)
		{
			player.SkillControlUnit.HandleSyncSkillMove(msg).HandleError<SyncSkillMoveRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, EmotionReq msg)
		{
			player.EmotionControlUnit.OnEmotion(msg.emotionMasterID, msg.basePosition, msg.hashCode).HandleError<EmotionRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, RepairTramReq msg)
		{
			player.HandleRepairTrain(msg.hashCode).HandleError<RepairTramRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, ChangeNextDungeonReq msg)
		{
			player.HandleChangeNextDungeonID(msg.selectedDungeonMasterID, msg.hashCode).HandleError<ChangeNextDungeonRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, JumpReq msg)
		{
			player.MovementControlUnit?.HandleJump();
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, PickTramUpgradeReq msg)
		{
			player.HandlePickTramUpgrade(msg.selectedUpgradeMasterID, msg.hashCode).HandleError<PickTramUpgradeRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, ReinforceItemReq msg)
		{
			player.HandleReinforceItem(msg.hashCode).HandleError<ReinforceItemRes>(player, msg.hashCode);
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, RollDungeonReq msg)
		{
			if (!(player.VRoom is VWaitingRoom vWaitingRoom))
			{
				player.SendToMe(new RollDungeonRes
				{
					errorCode = MsgErrorCode.InvalidRoomType,
					hashCode = msg.hashCode
				});
			}
			else
			{
				vWaitingRoom.RollDungeonByRequest(player, msg.hashCode).HandleError<RollDungeonRes>(player, msg.hashCode);
			}
		});
		RegisterPlayerDispatcher(delegate(VPlayer player, GetRemainScrapValueReq msg)
		{
			if (!(player.VRoom is DungeonRoom dungeonRoom))
			{
				player.SendToMe(new GetRemainScrapValueRes
				{
					errorCode = MsgErrorCode.InvalidRoomType,
					hashCode = msg.hashCode
				});
			}
			else
			{
				player.SendToMe(new GetRemainScrapValueRes(msg.hashCode));
				dungeonRoom.HandleGetRemainScrapValue();
			}
		});
	}

	public void RegisterSkillDispatcher()
	{
		RegisterPlayerDispatcher(delegate(VPlayer player, UseSkillReq msg)
		{
			player.SkillControlUnit?.HandleUseSkillReq(msg).HandleError<UseSkillRes>(player, msg.hashCode);
		});
	}

	public MsgErrorCode AdminCommandChangeImmutableStat(VPlayer player, AdminCommandArgs args)
	{
		StatType statType = args.AsEnum("stattype", StatType.Invalid);
		if (statType == StatType.Invalid)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		int num = args.AsInt32("amount", 0);
		player.StatControlUnit?.AddDebugStat(statType, num);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandResetStat(VPlayer player, AdminCommandArgs args)
	{
		player.StatControlUnit?.ResetDebugStat();
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandKill(VPlayer player, AdminCommandArgs args)
	{
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandReleaseItem(VPlayer player, AdminCommandArgs args)
	{
		player.HandleReleaseItem(0);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandDungeonRoomDecide(VPlayer player, AdminCommandArgs args)
	{
		DungeonCompleteCheckType type = (DungeonCompleteCheckType)args.AsInt32("type", 2);
		if (!(player.VRoom is DungeonRoom dungeonRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		dungeonRoom.CheckComplete(type);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAddCurrency(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("amount", 0);
		if (num <= 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!(player.VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		maintenanceRoom.AddCurrency(num);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandBuyItem(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("masterid", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		return player.HandleBuyItem(num, 0, 0);
	}

	public MsgErrorCode AdminCommandSessionDecide(VPlayer player, AdminCommandArgs args)
	{
		if (!(player.VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		maintenanceRoom.SessionDecision(player.ObjectID, force: true);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandOverwriteDungeonEndTime(VPlayer player, AdminCommandArgs args)
	{
		if (!(player.VRoom is DungeonRoom dungeonRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		long num = args.AsInt64("second", 0);
		if (num == 0L)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		dungeonRoom.OverwriteEndTime(num * 1000);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandTurnOnLantern(VPlayer player, AdminCommandArgs args)
	{
		bool isTurnOn = args.AsBool("ison", defaultValue: false);
		player.InventoryControlUnit?.ChangeEquipStatus(isTurnOn);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAddAbnormal(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("id", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.AbnormalControlUnit?.AppendAbnormal(player.ObjectID, num);
		return MsgErrorCode.Success;
	}

	private PosWithRot CreateAdminSpawnPos(PosWithRot originPos, bool randomYaw = false)
	{
		PosWithRot posWithRot = originPos.CreateForwardPosWithRot(2f);
		posWithRot.x += SimpleRandUtil.Next(-0.5f, 0.5f);
		posWithRot.z += SimpleRandUtil.Next(-0.5f, 0.5f);
		posWithRot.yaw = (randomYaw ? SimpleRandUtil.Next(0f, 360f) : (originPos.yaw - 180f));
		return posWithRot;
	}

	public MsgErrorCode AdminCommandSpawnMonster(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("id", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		PosWithRot pos = CreateAdminSpawnPos(player.Position);
		if (player.VRoom.CreateMonster(num, pos, player.IsIndoor, "", "", ReasonOfSpawn.Admin) == null)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandSpawnItem(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("masterid", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		bool isFake = args.AsBool("fake", defaultValue: false);
		ItemElement newItemElement = player.VRoom.GetNewItemElement(num, isFake);
		if (newItemElement == null)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		PosWithRot pos = CreateAdminSpawnPos(player.Position, randomYaw: true);
		if (player.VRoom.SpawnLootingObject(newItemElement, pos, player.IsIndoor, ReasonOfSpawn.Admin) == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandSetVisible(VPlayer player, AdminCommandArgs args)
	{
		bool isVisible = args.AsBool("flag", defaultValue: false);
		int num = args.AsInt32("time", 0);
		player.VRoom.AddEventTimer(delegate
		{
			player.SetVisible(!isVisible);
		}, num);
		player.SetVisible(isVisible);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandKillAllMonster(VPlayer player, AdminCommandArgs args)
	{
		player.VRoom.KillAllMonster();
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandToggleDrawDebug(VPlayer player, AdminCommandArgs args)
	{
		bool flag = args.AsBool("flag", defaultValue: false);
		long num = args.AsInt64("interval", 0);
		player.VRoom.ToggleDebug(flag);
		if (num != 0L)
		{
			player.VRoom.DebugIntervalMillisec = num;
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandSpawnFieldSkill(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("masterid", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.VRoom.SpawnFieldSkill(num, player.Position, player.IsIndoor, null, null, null, ReasonOfSpawn.Admin);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandChangeWeather(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("id", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (Hub.s.dataman.ExcelDataManager.GetWeatherInfo(num) == null)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!(player.VRoom is DungeonRoom dungeonRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		dungeonRoom.AdminChangeWeather(num);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAddWeatherSchedule(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("id", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (Hub.s.dataman.ExcelDataManager.GetWeatherInfo(num) == null)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		int num2 = args.AsInt32("start", 0);
		if (num2 < 0 || num2 >= 24)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		int num3 = args.AsInt32("duration", 1);
		if (num3 < 0 || num3 >= 24)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		bool forecast = args.AsBool("forecast", defaultValue: false);
		if (!(player.VRoom is DungeonRoom dungeonRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		dungeonRoom.AdminAddWeatherScehdule(num, num2, num3, forecast);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAddGametime(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("hour", 0);
		if (num == 0 || num <= -24 || num >= 24)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!(player.VRoom is DungeonRoom dungeonRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		dungeonRoom.AdminAddGametime(TimeSpan.FromHours(num));
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandSpawnProjectile(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("id", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.VRoom.CreateProjectileObject(num, player.Position, player.IsIndoor, null, ReasonOfSpawn.Admin);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandSpawnFieldSkillMapMarker(VPlayer player, AdminCommandArgs args)
	{
		string text = args.AsString("markerid", string.Empty);
		if (string.IsNullOrEmpty(text))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.VRoom.TriggerSpawnByEvent(text);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandSpawnHold(VPlayer player, AdminCommandArgs args)
	{
		bool spawnHold = args.AsBool("flag", defaultValue: false);
		player.VRoom.SetSpawnHold(spawnHold);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandRemoveConta(VPlayer player, AdminCommandArgs args)
	{
		player.VRoom.RecoverAllMemberConta();
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAddConta(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("value", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.StatControlUnit?.IncreaseConta(num);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandRecoverHP(VPlayer player, AdminCommandArgs args)
	{
		player.StatControlUnit?.AdjustHP(0L, full: true);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandChangeCurrentHP(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("amount", 0);
		if (num == 0)
		{
			player.StatControlUnit?.AdjustHP(0L, full: true);
		}
		else
		{
			player.StatControlUnit?.AdjustHP(num);
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandResetDungeonTime(VPlayer player, AdminCommandArgs args)
	{
		if (!(player.VRoom is DungeonRoom dungeonRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		dungeonRoom.ResetCurrentTime();
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandDetachForce(VPlayer player, AdminCommandArgs args)
	{
		player.AttachControlUnit?.DetachByRequest(player.ObjectID, 0, DetachReason.ForceCheat);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandChangeDayCount(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("day", 0);
		if (num <= 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!(player.VRoom is DungeonRoom dungeonRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		dungeonRoom.SetCurrentDayDebug(num);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandChangeCycle(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("cycle", 0);
		if (num <= 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!(player.VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		maintenanceRoom.SetCurrentCycleForDebug(num);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandKillAllPlayer(VPlayer player, AdminCommandArgs args)
	{
		player.VRoom.KillAllPlayer();
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandSuicide(VPlayer player, AdminCommandArgs args)
	{
		player.ForcedDying();
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAlterConta(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("percent", 0);
		if (num <= 0 || num > 100)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.StatControlUnit?.AdjustConta(num);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandReloadAI(VPlayer player, AdminCommandArgs args)
	{
		Hub.s.dataman.InitAIData();
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAddAura(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("masterid", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.AuraControlUnit?.AddAura(num, defaultFlag: false);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandDisableHeartBeatCheck(VPlayer player, AdminCommandArgs args)
	{
		bool flag = args.AsBool("flag", defaultValue: false);
		Hub.s.vworld?.DisableHeartBeatCheck(flag);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAddFaction(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("id", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		Faction_MasterData faction = Hub.s.dataman.ExcelDataManager.GetFaction(num);
		if (faction == null)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (faction.group == 1)
		{
			Logger.RError("AdminCommandAddFaction. Not allow to modify faction group 1");
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.AddFaction(num);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandRemoveFaction(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("id", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		Faction_MasterData faction = Hub.s.dataman.ExcelDataManager.GetFaction(num);
		if (faction == null)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (faction.group == 1)
		{
			Logger.RError("AdminCommandRemoveFaction. Not allow to modify faction group 1");
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.RemoveFaction(num);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandStopAI(VPlayer player, AdminCommandArgs args)
	{
		bool flag = args.AsBool("flag", defaultValue: false);
		if (flag)
		{
			player.VRoom.EnableAIControl(!flag);
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandEnableAggroLogging(VPlayer player, AdminCommandArgs args)
	{
		bool enable = args.AsBool("flag", defaultValue: false);
		player.VRoom.EnableAggroLogging(enable);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandStartScrapMotion(VPlayer player, AdminCommandArgs args)
	{
		player.InventoryControlUnit?.StartScrapMotion(player.Position, 0, sync: true);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandEndScrapMotion(VPlayer player, AdminCommandArgs args)
	{
		player.InventoryControlUnit?.EndScrapMotion(player.Position, 0, sync: true);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandChangeLeverToDeathMatchRoom(VPlayer player, AdminCommandArgs args)
	{
		if (!(player.VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		maintenanceRoom.SetState(MaintenenceRoomState.DecisionNextGame);
		args.AsBool("flag", defaultValue: false);
		Hub.s.pdata.GameState = Hub.PersistentData.eGameState.Maintenance;
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandPlayTramRepairCutSceneForDebug(VPlayer player, AdminCommandArgs args)
	{
		if (!(player.VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		var (msgErrorCode, remainCurrency) = maintenanceRoom.PlayTramRepairCutSceneForDebug();
		if (msgErrorCode == MsgErrorCode.Success)
		{
			player.SendToChannel(new StartRepairTramSig
			{
				remainCurrency = remainCurrency
			});
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandTeleportToPos(VPlayer player, AdminCommandArgs args)
	{
		string text = args.AsString("pos", string.Empty);
		if (string.IsNullOrEmpty(text))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		string[] array = text.Split(',');
		if (array.Length != 3)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!float.TryParse(array[0], out var result) || !float.TryParse(array[1], out var result2) || !float.TryParse(array[2], out var result3))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		player.Teleport(new PosWithRot
		{
			pos = new Vector3(result, result2, result3)
		}, TeleportReason.System);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandSaveGameData(VPlayer player, AdminCommandArgs args)
	{
		int num = -999;
		int num2 = args.AsInt32("slot", num);
		if (num2 != num && num2 != -1 && !MMSaveGameData.CheckSaveSlotID(num2))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (num2 == -1)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (num2 == num)
		{
			Logger.RWarn($"[AdminCommandSaveGameData] use current slotID. ({Hub.s.vworld.SaveSlotID})");
			if (Hub.s.vworld.SaveSlotID == -1)
			{
				Logger.RWarn($"[AdminCommandSaveGameData] but current slotID cannot use save. ({Hub.s.vworld.SaveSlotID})");
				return MsgErrorCode.InvalidAdminCommandArgument;
			}
			num2 = Hub.s.vworld.SaveSlotID;
		}
		List<string> playerNames = new List<string>();
		player.VRoom.IterateAllPlayer(delegate(VPlayer itorPlayer)
		{
			if (itorPlayer.IsHost)
			{
				playerNames.Insert(0, itorPlayer.ActorName);
			}
			else
			{
				playerNames.Add(itorPlayer.ActorName);
			}
		});
		while (playerNames.Count < Hub.s.dataman.ExcelDataManager.Consts.C_MaxPlayerCount)
		{
			playerNames.Add($"Player{playerNames.Count + 1:D02}_Slot{num2:D02}_ForTest");
		}
		if (player.HandleSaveGameDataReq(num2, playerNames, 0) == MsgErrorCode.Success)
		{
			if (num2 != Hub.s.pdata.SaveSlotID)
			{
				Hub.s.pdata.SaveSlotID = num2;
			}
			if (num2 != Hub.s.vworld.SaveSlotID)
			{
				Hub.s.vworld.SaveSlotID = num2;
			}
		}
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandReinforceItem(VPlayer player, AdminCommandArgs args)
	{
		player.HandleReinforceItem(0);
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAddTramUpgrade(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("id", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (Hub.s.dataman.ExcelDataManager.GetTramupgradeData(num) == null)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!(player.VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		maintenanceRoom.PickUpgradeCandidate(num, force: true);
		maintenanceRoom.ApplyTramUpgrage();
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandRemoveTramUpgrade(VPlayer player, AdminCommandArgs args)
	{
		int num = args.AsInt32("id", 0);
		if (num == 0)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (Hub.s.dataman.ExcelDataManager.GetTramupgradeData(num) == null)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!(player.VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!maintenanceRoom.TramUpgradeList.Contains(num))
		{
			return MsgErrorCode.ItemNotFound;
		}
		maintenanceRoom.TramUpgradeList.Remove(num);
		maintenanceRoom.SendToAllPlayers(new ChangeTramPartsSig
		{
			sessionCount = maintenanceRoom.SessionCycleCount,
			upgradeList = new List<int>(maintenanceRoom.TramUpgradeList)
		});
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandAllTramUpgrade(VPlayer player, AdminCommandArgs args)
	{
		ImmutableDictionary<int, TramupgradeData_MasterData> tramupgradeDataDict = Hub.s.dataman.ExcelDataManager.TramupgradeDataDict;
		if (tramupgradeDataDict == null)
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		if (!(player.VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		foreach (TramupgradeData_MasterData value in tramupgradeDataDict.Values)
		{
			if (!maintenanceRoom.TramUpgradeList.Contains(value.id))
			{
				maintenanceRoom.TramUpgradeList.Add(value.id);
			}
		}
		maintenanceRoom.SendToAllPlayers(new ChangeTramPartsSig
		{
			sessionCount = maintenanceRoom.SessionCycleCount,
			upgradeList = new List<int>(maintenanceRoom.TramUpgradeList)
		});
		return MsgErrorCode.Success;
	}

	public MsgErrorCode AdminCommandResetTramUpgrade(VPlayer player, AdminCommandArgs args)
	{
		if (!(player.VRoom is MaintenanceRoom maintenanceRoom))
		{
			return MsgErrorCode.InvalidAdminCommandArgument;
		}
		maintenanceRoom.TramUpgradeList.Clear();
		maintenanceRoom.SendToAllPlayers(new ChangeTramPartsSig
		{
			sessionCount = maintenanceRoom.SessionCycleCount,
			upgradeList = new List<int>(maintenanceRoom.TramUpgradeList)
		});
		return MsgErrorCode.Success;
	}

	public VirtualAcceptSession? ConnectVirtualSession(VirtualClientSession clientSession)
	{
		if (_sessionManager.ExistHost())
		{
			return null;
		}
		VirtualAcceptSession virtualAcceptSession = new VirtualAcceptSession(_serverDispatchers);
		virtualAcceptSession.SetClientSession(clientSession);
		SessionContext sessionContext = new SessionContext(virtualAcceptSession);
		virtualAcceptSession.SetContext(sessionContext);
		_replayManager?.OnSetSessionCtxEvent(sessionContext);
		if (!_sessionManager.SetHost(sessionContext))
		{
			Logger.RError("failed to set host session");
			return null;
		}
		return virtualAcceptSession;
	}

	public bool DisconnectVirtualSession(VirtualClientSession clientSession)
	{
		if (!_sessionManager.ExistHost())
		{
			Logger.RError("host session does not exist");
			return false;
		}
		if (!_sessionManager.RemoveHost())
		{
			Logger.RError("failed to remove host session");
			return false;
		}
		return true;
	}

	public void OnClientSessionAdd(ISession session)
	{
		SessionContext sessionContext = new SessionContext(session);
		_sessionManager.Add(sessionContext);
		_replayManager?.OnSetSessionCtxEvent(sessionContext);
	}

	public void OnClientSessionRemove(IContext context, DisconnectReason reason)
	{
		_sessionManager.Remove(context.GetSessionID(), reason);
	}

	public void OnClientSessionError(IContext context, Exception ex)
	{
		Logger.RError($"session error, sessionID: {context.GetSessionID()} / {ex}");
		_sessionManager.Remove(context.GetSessionID(), DisconnectReason.ConnectionError);
	}

	public void OnRUDPHolePunchingRegistHandler(IResMsg resMsg)
	{
	}

	public void BroadcastToAll(IMsg msg)
	{
		_sessionManager.BroadcastToAll(msg);
	}

	public void Disconnect(int sessionID)
	{
		_sessionManager.Remove(sessionID, DisconnectReason.ByServer);
	}

	public void DisableHeartBeatCheck(bool flag)
	{
		_sessionManager.SetDisableHeartBeatCheck(flag);
	}

	public int GetRoomTypeMemberCount(VRoomType roomType)
	{
		return _vRoomManager.GetInPlayMemberCount(roomType);
	}

	public int GetSessionCount(VRoomType roomType)
	{
		return _sessionManager.GetSessionCount(roomType);
	}

	public int GetRoomMemberCount(VRoomType roomType)
	{
		return _vRoomManager.GetRoomMemberCount(roomType);
	}

	public bool InitHostRoom(string externalToken, int saveSlotID, string hostPlayerName)
	{
		if (SaveSlotID != saveSlotID)
		{
			Logger.RError("[VWorld] InitHostRoom() saveSlotID mismatch");
			return false;
		}
		if (saveSlotID == 0 && !MonoSingleton<PlatformMgr>.Instance.IsSaveFileExist(MMSaveGameData.GetSaveFileName(saveSlotID)))
		{
			Logger.RError("[VWorld] InitHostRoom() SaveSlotID_Auto file missing");
			return false;
		}
		if (saveSlotID != -1 && MonoSingleton<PlatformMgr>.Instance.IsSaveFileExist(MMSaveGameData.GetSaveFileName(saveSlotID)) && !MonoSingleton<PlatformMgr>.Instance.CanLoadSaveGameData<MMSaveGameData>(MMSaveGameData.GetSaveFileName(saveSlotID)))
		{
			Logger.RError($"[VWorld] InitHostRoom() SaveGameData load test fail! saveSlotID={saveSlotID}");
			return false;
		}
		_vRoomManager.InitMaintenenceRoom(externalToken, SaveSlotID, hostPlayerName);
		return true;
	}

	public void InitWaitingRoom()
	{
		_vRoomManager.InitWaitingRoom();
	}

	public void InitDeathMatchRoom()
	{
		_vRoomManager.InitDeathMatchRoom();
	}

	public void CreateGameRoom()
	{
		_vRoomManager.CreateGameRoom();
	}

	public long GetMaintenenceRoomID()
	{
		if (!_initialized)
		{
			Logger.RError("VWorld is not initialized");
			return -1L;
		}
		return _vRoomManager.GetMaintenenceRoomUID();
	}

	public (int cycleCount, int dayCount, bool repaired) GetMaintenenceRoomCycleInfos()
	{
		if (!_initialized)
		{
			Logger.RError("VWorld is not initialized");
			return (cycleCount: 1, dayCount: 1, repaired: true);
		}
		return _vRoomManager.GetMaintenenceRoomCycleInfos();
	}

	public List<int> GetMaintenenceRoomTramUpgradeList()
	{
		if (!_initialized)
		{
			Logger.RError("VWorld is not initialized");
			return new List<int>();
		}
		return _vRoomManager.GetMaintenenceRoomTramUpgradeList();
	}

	public long GetHostSessionID()
	{
		if (!_initialized)
		{
			Logger.RError("VWorld is not initialized");
			return -1L;
		}
		if (Hub.s.pdata.ClientMode != NetworkClientMode.Host)
		{
			Logger.RError("Only host can get created session id");
			return -1L;
		}
		return _vRoomManager.GetLobbySessionID();
	}

	public void PendStartSession(Dictionary<ulong, long> playerUIDs, RoomDrainInfo info)
	{
		_vRoomManager.PendStartSession(playerUIDs, info);
	}

	public void PendStartGame(Dictionary<ulong, long> playerUIDs, int nextDungeonMasterID, int randomDungeonSeed, RoomDrainInfo info)
	{
		_vRoomManager.PendStartGame(playerUIDs, nextDungeonMasterID, randomDungeonSeed, info);
	}

	public bool PendEndGame(Dictionary<ulong, long> playerUIDs, RoomDrainInfo info)
	{
		return _vRoomManager.PendEndGame(playerUIDs, info);
	}

	public void PendStartDeathMatch(Dictionary<ulong, long> playerUIDs, RoomDrainInfo info)
	{
		_vRoomManager.PendStartDeathMatch(playerUIDs, info);
	}

	public void OnFinishDungeon(RoomDrainInfo info, List<IGameEventLog> eventLogs, bool isSuccess)
	{
		_vRoomManager.OnFinishGame(info, isSuccess);
		SendGameEventLogs(eventLogs);
		_replayManager.OnStopRecording();
	}

	private void SendGameEventLogs(List<IGameEventLog> eventLogs)
	{
		Hub.s.apihandler.EnqueueAPI<APIGameEventLogRes>(new APIGameEventLogReq
		{
			guid = Hub.s.pdata.GUID,
			sessionID = Hub.s.pdata.ClientSessionID,
			eventBody = JsonUtility.ToJson(eventLogs)
		}, delegate(IResMsg res)
		{
			if (res.errorCode != MsgErrorCode.Success)
			{
				Logger.RWarn($"Failed to send game event logs: {res.errorCode}");
			}
		});
	}

	public void Promote(Dictionary<ulong, long> playerUIDs, RoomDrainInfo drainInfo)
	{
		_vRoomManager.PromoteSession(playerUIDs, drainInfo);
	}

	public void TerminateSession(Dictionary<ulong, long> playerUIDs, RoomDrainInfo drainInfo)
	{
		_vRoomManager.TerminateSession(playerUIDs, drainInfo);
	}

	public MsgErrorCode RegistPlayer(ulong steamID, bool isHost)
	{
		return _vRoomManager.OnRegistPlayer(steamID, isHost);
	}

	public void OnUnregistPlayer(ulong steamID)
	{
		_vRoomManager.OnUnregistPlayer(steamID);
	}

	public void RemoveDeathMatchRoom()
	{
		_vRoomManager.RemoveDeathMatchRoom();
	}

	public VWorld(string version, int saveSlotID)
	{
		_eventTimer = new EventTimer();
		_commandExecutor = CommandExecutor.CreateCommandExecutor("VWorld", 10000001L);
		_sessionManager = new SessionManager();
		_vRoomManager = new VRoomManager();
		Version = version;
		SaveSlotID = saveSlotID;
		_serverDispatchers.SetDefaultPreDispatcher(_replayManager.OnPreDispatcherHookEvent);
		RegisterDispatcher();
		RegisterPktMetricDispatcher();
	}

	public void OnDestroy()
	{
		try
		{
			_vRoomManager.Dispose();
		}
		catch (Exception arg)
		{
			Logger.RError($"_vRoomManager.Dispose : {arg}");
		}
		try
		{
			_sessionManager.Dispose();
		}
		catch (Exception arg2)
		{
			Logger.RError($"_sessionManager.Dispose : {arg2}");
		}
		try
		{
			_rudpServer?.Dispose();
			_sdrServer?.Dispose();
		}
		catch (Exception arg3)
		{
			Logger.RError($"_rudpServer.Dispose : {arg3}");
		}
	}

	public void Initialize()
	{
		_rudpServer = new RUDPServer(_sessionManager, _serverDispatchers, Hub.s.pdata.GameServerPort, OnClientSessionAdd, OnClientSessionRemove, OnClientSessionError, OnRUDPHolePunchingRegistHandler);
		_vRoomManager.Initialize();
		_sessionManager.Initialize();
		_rudpServer.Start();
		if (SteamManager.Initialized)
		{
			_sdrServer = new SDRServer(_sessionManager, _serverDispatchers, Hub.s.pdata.GameServerPort, OnClientSessionAdd, OnClientSessionRemove, OnClientSessionError);
			_sdrServer.Start();
		}
		_initialized = true;
	}

	public void Update()
	{
		if (_initialized)
		{
			_rudpServer?.Update();
			_sdrServer?.Update();
			UpdateDL();
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (_lastTick == 0L)
			{
				_lastTick = currentTickMilliSec;
			}
			long delta = currentTickMilliSec - _lastTick;
			_lastTick = currentTickMilliSec;
			_eventTimer.Update();
			_commandExecutor.Execute();
			_sessionManager.OnUpdate();
			_vRoomManager.OnUpdate(delta);
		}
	}

	public void UpdateDL()
	{
		if (Hub.s.dLAcademyManager.ExistActivatedAgent())
		{
			Hub.s.dLAcademyManager.Step();
		}
	}

	public void Invoke(Command command)
	{
		_commandExecutor.Invoke(command);
	}

	public bool Shutdown()
	{
		return true;
	}

	public void RegisterDispatcher()
	{
		RegisterAuthPktDispatcher();
		RegisterPlayerPktDispatcher();
		RegisterItemDispatcher();
		RegisterSkillDispatcher();
		RegisterAdminProtocol();
	}

	public void RegisterPktMetricDispatcher()
	{
	}

	public void RegisterNetworkTraceLogDispatcher()
	{
	}

	public void LoadData()
	{
	}

	private void RegisterPlayerDispatcher<T>(OnPlayerDispatchEventHandler<T> handler, OnPreDispatchEventHandler? preDispatchEventHandler = null, bool passFlag = false) where T : IMsg, new()
	{
		_serverDispatchers.RegisterDispatcher(delegate(IContext ctx, T msg)
		{
			if (!(ctx is SessionContext sessionContext))
			{
				Logger.RError("context is not sessionContext");
			}
			else if (!passFlag)
			{
				if (!sessionContext.ExistPlayer())
				{
					Logger.RWarn("SessionContext's player is null. msg type: " + typeof(T).Name);
				}
				else
				{
					sessionContext.PostPlayerHandler(handler, msg);
				}
			}
		});
	}

	private void RegisterPlayerDispatcherAsync<T>(OnPlayerDispatchAsyncEventHandler<T> handler, OnPreDispatchEventHandler? preDispatchEventHandler = null, bool passFlag = false) where T : IMsg, new()
	{
		_serverDispatchers.RegisterDispatcher(delegate(IContext ctx, T msg)
		{
			if (!(ctx is SessionContext sessionContext))
			{
				Logger.RError($"context is not sessionContext, sessionID: {ctx.GetSessionID()}");
				ctx.BeginClose(DisconnectReason.Undefined);
			}
			else if (!passFlag)
			{
				if (!sessionContext.ExistPlayer())
				{
					Logger.RWarn("SessionContext's player is null");
				}
				else
				{
					sessionContext.PostPlayerAsyncHandler(handler, msg);
				}
			}
		});
	}

	private void RegisterClientSessionDispatcher<T>(OnClientSessionDispatchEventHandler<T> handler, OnPreDispatchEventHandler? preDispatchEventHandler = null) where T : IMsg, new()
	{
		_serverDispatchers.RegisterDispatcher(delegate(IContext ctx, T msg)
		{
			if (!(ctx is SessionContext sessionContext))
			{
				Logger.RError($"context is not sessionContext, sessionID: {ctx.GetSessionID()}");
				ctx.BeginClose(DisconnectReason.Undefined);
			}
			else
			{
				sessionContext.PostHandler(handler, msg);
			}
		});
	}

	private void RegisterClientSessionDispatcherAsync<T>(OnClientSessionDispatchAsyncEventHandler<T> handler, OnPreDispatchEventHandler? preDispatchEventHandler = null) where T : IMsg, new()
	{
		_serverDispatchers.RegisterDispatcher(delegate(IContext ctx, T msg)
		{
			if (!(ctx is SessionContext sessionContext))
			{
				Logger.RError($"context is not sessionContext, sessionID: {ctx.GetSessionID()}");
				ctx.BeginClose(DisconnectReason.Undefined);
			}
			else
			{
				sessionContext.PostAsyncHandler(handler, msg);
			}
		});
	}

	private long GetTempPlayerUID()
	{
		return Interlocked.Increment(ref _tempPlayerUID);
	}

	private ulong GetTempSteamID()
	{
		return (ulong)Interlocked.Increment(ref _tempSteamID);
	}

	public bool HasNearestPoly(Vector3 pos)
	{
		return Hub.s.navman.GetNearestPointOnNavMesh(pos, 100f) != Vector3.zero;
	}

	public Vector3 FindNearestPoly(Vector3 pos, float maxDistance = 100f)
	{
		return Hub.s.navman.GetNearestPointOnNavMesh(pos, maxDistance);
	}

	public bool IsHitWall(Vector3 startPos, Vector3 endPos, out Vector3 hitPos)
	{
		return Hub.s.navman.IsHitWall(startPos, endPos, out hitPos);
	}

	public Vector3 FindNearestPositionInDistance(Vector3 pos, float distance)
	{
		return Hub.s.navman.GetNearestPointOnNavMesh(pos, distance);
	}

	public float GetNavDistance(Vector3 startPos, Vector3 endPos)
	{
		PathFindResult route = Hub.s.navman.GetRoute(startPos, endPos);
		if (route.Success)
		{
			return route.Length;
		}
		return float.MaxValue;
	}

	public bool FindPath(Vector3 startPos, Vector3 endPos, out List<Vector3>? path)
	{
		PathFindResult route = Hub.s.navman.GetRoute(startPos, endPos);
		if (route.Success)
		{
			path = route.PathPoints;
			return true;
		}
		path = null;
		return false;
	}

	public bool VerticalRaycast(Vector3 startPos, float maxDistance, bool upDir, out Vector3 nearestPoint)
	{
		nearestPoint = Vector3.zero;
		if (maxDistance <= 0f)
		{
			return false;
		}
		Vector3 vector = (upDir ? Vector3.up : Vector3.down);
		float num = 0f;
		float num2 = 0f;
		Vector3 vector2 = default(Vector3);
		for (; num2 <= maxDistance; num2 += 2f)
		{
			Vector3 position = startPos + vector * num2;
			Vector3 nearestPointOnNavMesh = Hub.s.navman.GetNearestPointOnNavMesh(position, 1.1f);
			if (nearestPointOnNavMesh != Vector3.zero)
			{
				vector2 = nearestPointOnNavMesh;
				break;
			}
			num = num2;
		}
		if (vector2 == default(Vector3) && num < maxDistance)
		{
			Vector3 position2 = startPos + vector * maxDistance;
			Vector3 nearestPointOnNavMesh2 = Hub.s.navman.GetNearestPointOnNavMesh(position2, 1.1f);
			if (!(nearestPointOnNavMesh2 != Vector3.zero))
			{
				return false;
			}
			vector2 = nearestPointOnNavMesh2;
		}
		nearestPoint = vector2;
		return true;
	}

	public Vector3 MoveAlongSurface(Vector3 startPos, Vector3 endPos, bool ignore)
	{
		return Hub.s.navman.RayCast(startPos, endPos, ignore);
	}

	public bool MoveAlongSurfaceSequentially(Vector3 startPos, List<Vector3> moves, out List<Vector3> movedPositions)
	{
		movedPositions = new List<Vector3>();
		return false;
	}

	public float DistanceToWall(Vector3 pos, Vector3 dir, float maxRadius)
	{
		if (Hub.s.navman.GetNearestPointOnNavMesh(pos, 100f) == Vector3.zero)
		{
			return 9999999f;
		}
		Vector3 vector = Hub.s.navman.RayCast(pos, pos + dir * maxRadius);
		if (vector == Vector3.zero)
		{
			return 9999999f;
		}
		return Vector3.Distance(pos, vector);
	}

	public bool GetRandomReachablePointInRadius(Vector3 pos, float minRad, float maxRad, out Vector3 resultPos)
	{
		float f = SimpleRandUtil.Next(0f, MathF.PI * 2f);
		float num = SimpleRandUtil.Next(minRad, maxRad);
		Vector3 position = pos + new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * num;
		Vector3 nearestPointOnNavMesh = Hub.s.navman.GetNearestPointOnNavMesh(position, 10000f);
		if (nearestPointOnNavMesh != Vector3.zero)
		{
			resultPos = nearestPointOnNavMesh;
			return true;
		}
		resultPos = pos;
		return false;
	}

	public int GenerateNewDecoratorID()
	{
		return Interlocked.Increment(ref _decoratorGenerator);
	}

	public void ReadyToGamePktRecording(int nextDungeonMasterID, int randomDungeonSeed)
	{
		if (_replayManager.UseRecordMode)
		{
			_replayManager.OnReadyToGamePktRecording(nextDungeonMasterID, randomDungeonSeed);
		}
	}

	public void ReadyToDeathMatchPktRecording()
	{
		if (_replayManager.UseRecordMode)
		{
			_replayManager.OnReadyToDeathMatchPktRecording();
		}
	}
}
