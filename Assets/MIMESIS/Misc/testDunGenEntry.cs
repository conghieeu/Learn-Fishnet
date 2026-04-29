using DunGen;
using UnityEngine;
using UnityEngine.InputSystem;

public class testDunGenEntry : MonoBehaviour
{
	public RuntimeDungeon DungeonGenerator;

	public mapwalker mapwalker;

	public bool done;

	private void Start()
	{
		if (Hub.s == null || Hub.s.pdata == null)
		{
			return;
		}
		if (DungeonGenerator != null)
		{
			DungeonGenerator.Generator.OnGenerationComplete += delegate
			{
				done = true;
			};
		}
		Generate();
	}

	private void Update()
	{
		if (Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			Generate();
		}
	}

	private void Generate()
	{
		done = false;
		Hub.s.pdata.ResetDunGenTileIDSeed();
		if (DungeonGenerator != null)
		{
			DungeonGenerator.Generate();
		}
		Hub.s.navman.Build();
		mapwalker.Reset();
	}
}
