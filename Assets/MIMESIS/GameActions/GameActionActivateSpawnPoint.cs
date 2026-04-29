using System.Collections.Generic;
using Bifrost.ConstEnum;

public class GameActionActivateSpawnPoint : GameAction
{
	public List<string> SpawnPointNameList { get; private set; } = new List<string>();

	public GameActionActivateSpawnPoint(List<string> spawnPointNameList)
		: base(DefAction.ACTIVATE_SPAWNPOINT)
	{
		SpawnPointNameList.Clear();
		SpawnPointNameList.AddRange(spawnPointNameList);
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionActivateSpawnPoint(SpawnPointNameList);
	}

	public override IGameAction Clone()
	{
		return new GameActionActivateSpawnPoint(SpawnPointNameList);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionActivateSpawnPoint gameActionActivateSpawnPoint)
		{
			if (SpawnPointNameList.Count != gameActionActivateSpawnPoint.SpawnPointNameList.Count)
			{
				return false;
			}
			for (int i = 0; i < SpawnPointNameList.Count; i++)
			{
				if (SpawnPointNameList[i] != gameActionActivateSpawnPoint.SpawnPointNameList[i])
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}
}
