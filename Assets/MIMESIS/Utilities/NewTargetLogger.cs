using System.Collections.Generic;
using ReluProtocol;

public class NewTargetLogger
{
	private VActor _actor;

	private HashSet<int> _alreadyHitTargets;

	private int _skillID;

	private int _basePoint;

	public NewTargetLogger(VActor actor, int skillID, int basePoint)
	{
		_actor = actor;
		_skillID = skillID;
		_basePoint = basePoint;
		_alreadyHitTargets = new HashSet<int>();
	}

	public void Update(List<TargetHitInfo> targets)
	{
		int num = 0;
		foreach (TargetHitInfo target in targets)
		{
			if (!_alreadyHitTargets.Contains(target.targetID))
			{
				_alreadyHitTargets.Add(target.targetID);
				num++;
			}
		}
	}
}
