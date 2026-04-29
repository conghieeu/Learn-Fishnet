using System.Collections.Generic;
using System.Text;
using ReluProtocol.Enum;

public class ControllerManager
{
	public AtomicFlag _disposed = new AtomicFlag(value: false);

	public readonly Dictionary<VActorControllerType, IVActorController> controllers = new Dictionary<VActorControllerType, IVActorController>();

	public void Initialize()
	{
		foreach (IVActorController value in controllers.Values)
		{
			value.Initialize();
		}
		foreach (IVActorController value2 in controllers.Values)
		{
			value2.WaitInitDone();
		}
	}

	public bool AddController(IVActorController controller)
	{
		if (controllers.ContainsKey(controller.type))
		{
			return false;
		}
		controllers.Add(controller.type, controller);
		return true;
	}

	public bool RemoveController(VActorControllerType type)
	{
		if (!controllers.ContainsKey(type))
		{
			return false;
		}
		controllers.Remove(type);
		return true;
	}

	public IVActorController? GetController(VActorControllerType type)
	{
		controllers.TryGetValue(type, out IVActorController value);
		return value;
	}

	public void Update(long delta)
	{
		foreach (IVActorController value in controllers.Values)
		{
			value.Update(delta);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void Dispose(bool disposing)
	{
		if (!disposing || !_disposed.On())
		{
			return;
		}
		foreach (IVActorController value in controllers.Values)
		{
			value.Dispose();
		}
		controllers.Clear();
	}

	public MsgErrorCode CanAction(VActorActionType actionType)
	{
		foreach (IVActorController value in controllers.Values)
		{
			MsgErrorCode msgErrorCode = value.CanAction(actionType);
			if (msgErrorCode != MsgErrorCode.Success)
			{
				return msgErrorCode;
			}
		}
		return MsgErrorCode.Success;
	}

	public string GetDebugInfo()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (IVActorController value in controllers.Values)
		{
			stringBuilder.AppendLine(value.GetDebugString());
		}
		return stringBuilder.ToString();
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
		foreach (IVActorController value in controllers.Values)
		{
			value.CollectDebugInfo(ref sig);
		}
	}
}
