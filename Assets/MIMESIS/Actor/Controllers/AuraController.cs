using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Cooked;
using ReluProtocol.Enum;

public class AuraController : IVActorController, IDisposable
{
	private VCreature _creature;

	private Dictionary<int, AuraContext> _auraContexts = new Dictionary<int, AuraContext>();

	private Dictionary<int, HashSet<int>> _appliedAuraInfos = new Dictionary<int, HashSet<int>>();

	private int _defaultAuraMasterID;

	public VActorControllerType type { get; } = VActorControllerType.Aura;

	public AuraController(VActor actor)
	{
		if (!(actor is VCreature creature))
		{
			throw new Exception("actor is not VCreature");
		}
		_creature = creature;
	}

	public void Initialize()
	{
		if (!(_creature is VMonster vMonster))
		{
			return;
		}
		MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(vMonster.MasterID);
		if (monsterInfo == null)
		{
			Logger.RError($"[AuraController] MonsterInfo not found for master ID {vMonster.MasterID}");
			return;
		}
		int auraMasterID = monsterInfo.AuraMasterID;
		if (auraMasterID != 0)
		{
			AddAura(auraMasterID, defaultFlag: true);
		}
	}

	public void Update(long delta)
	{
		if (_auraContexts.Count == 0)
		{
			return;
		}
		Hub.s.timeutil.GetCurrentTickMilliSec();
		List<int> list = new List<int>();
		foreach (AuraContext value2 in _auraContexts.Values)
		{
			if (!value2.DefaultFlag && Hub.s.timeutil.GetCurrentTickMilliSec() - value2.StartTime >= value2.AuraInfo.AuraDuration)
			{
				list.Add(value2.AuraInfo.MasterID);
			}
			else
			{
				value2.Update();
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		_creature.SendInSight(new AuraSig
		{
			actorID = _creature.ObjectID,
			removedAuraMasterIDs = list
		}, includeSelf: true);
		foreach (int item in list)
		{
			if (_auraContexts.TryGetValue(item, out AuraContext value))
			{
				value.Dispose();
				_auraContexts.Remove(item);
			}
		}
	}

	public void AddAura(int auraMasterID, bool defaultFlag)
	{
		if (!_auraContexts.TryGetValue(auraMasterID, out AuraContext _))
		{
			AuraInfo auraInfo = Hub.s.dataman.ExcelDataManager.GetAuraInfo(auraMasterID);
			if (auraInfo == null)
			{
				Logger.RError($"[AuraController] AuraInfo not found for master ID {auraMasterID}");
				return;
			}
			_auraContexts.Add(auraMasterID, new AuraContext(_creature, auraInfo, defaultFlag));
			_creature.SendInSight(new AuraSig
			{
				actorID = _creature.ObjectID,
				addedAuraMasterIDs = new List<int> { auraMasterID }
			}, includeSelf: true);
		}
	}

	public void OnDead()
	{
		foreach (int item in _auraContexts.Keys.ToList())
		{
			RemoveAura(item);
		}
		_auraContexts.Clear();
		_appliedAuraInfos.Clear();
	}

	public void RemoveAura(int auraMasterID, bool sync = false)
	{
		if (_auraContexts.TryGetValue(auraMasterID, out AuraContext value))
		{
			value.Dispose();
			_auraContexts.Remove(auraMasterID);
			if (sync)
			{
				_creature.SendInSight(new AuraSig
				{
					actorID = _creature.ObjectID,
					removedAuraMasterIDs = new List<int> { auraMasterID }
				}, includeSelf: true);
			}
		}
	}

	public void ApplyAura(int casterActorID, int auraMasterID)
	{
		AuraInfo auraInfo = Hub.s.dataman.ExcelDataManager.GetAuraInfo(auraMasterID);
		if (auraInfo == null)
		{
			Logger.RError($"[AuraController] AuraInfo not found for master ID {auraMasterID}");
			return;
		}
		if (_appliedAuraInfos.TryGetValue(auraMasterID, out HashSet<int> value))
		{
			if (value.Contains(casterActorID))
			{
				return;
			}
			value.Add(casterActorID);
		}
		else
		{
			_appliedAuraInfos[auraMasterID] = new HashSet<int> { casterActorID };
		}
		_creature.AbnormalControlUnit?.AppendAbnormal(casterActorID, auraInfo.AbnormalMasterID, auraInfo.RemoveAbnormalOnAuraEnd ? 600000 : 0);
	}

	public void EscapeFromAura(int auraMasterID, int actorID)
	{
		AuraInfo auraInfo = Hub.s?.dataman?.ExcelDataManager.GetAuraInfo(auraMasterID);
		if (auraInfo == null)
		{
			Logger.RError($"[AuraController] AuraInfo not found for master ID {auraMasterID}");
		}
		else
		{
			if (!_appliedAuraInfos.TryGetValue(auraMasterID, out HashSet<int> value))
			{
				return;
			}
			value.Remove(actorID);
			if (value.Count == 0)
			{
				_appliedAuraInfos.Remove(auraMasterID);
				if (auraInfo.RemoveAbnormalOnAuraEnd)
				{
					_creature.AbnormalControlUnit?.DispelAbnormal(_creature.ObjectID, auraInfo.AbnormalMasterID, force: true);
				}
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}
		foreach (AuraContext value in _auraContexts.Values)
		{
			value.Dispose();
		}
		_auraContexts.Clear();
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		return MsgErrorCode.Success;
	}

	public void WaitInitDone()
	{
	}

	public string GetDebugString()
	{
		return string.Empty;
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
		if (_auraContexts.Count == 0)
		{
			return;
		}
		foreach (AuraContext value in _auraContexts.Values)
		{
			value.CollectDebugInfo(ref sig);
		}
	}

	public List<int> GetActivatedAuras()
	{
		return _auraContexts.Keys.ToList();
	}
}
