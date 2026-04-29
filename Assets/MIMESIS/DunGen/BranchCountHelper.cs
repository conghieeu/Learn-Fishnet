using System;
using System.Collections.Generic;
using System.Linq;
using DunGen.Graph;
using UnityEngine;

namespace DunGen
{
	public static class BranchCountHelper
	{
		public static void ComputeBranchCounts(DungeonFlow dungeonFlow, RandomStream randomStream, DungeonProxy proxyDungeon, ref int[] mainPathBranches)
		{
			switch (dungeonFlow.BranchMode)
			{
			case BranchMode.Local:
				ComputeBranchCountsLocal(randomStream, proxyDungeon, ref mainPathBranches);
				break;
			case BranchMode.Global:
				ComputeBranchCountsGlobal(dungeonFlow, randomStream, proxyDungeon, ref mainPathBranches);
				break;
			case BranchMode.Section:
				ComputeBranchCountsPerSection(randomStream, proxyDungeon, ref mainPathBranches);
				break;
			default:
				throw new NotImplementedException($"{typeof(BranchMode).Name}.{dungeonFlow.BranchMode} is not implemented");
			}
		}

		private static void ComputeBranchCountsLocal(RandomStream randomStream, DungeonProxy proxyDungeon, ref int[] mainPathBranches)
		{
			for (int i = 0; i < mainPathBranches.Length; i++)
			{
				TileProxy tileProxy = proxyDungeon.MainPathTiles[i];
				if (!(tileProxy.Placement.Archetype == null))
				{
					int random = tileProxy.Placement.Archetype.BranchCount.GetRandom(randomStream);
					random = Mathf.Min(random, tileProxy.UnusedDoorways.Count());
					mainPathBranches[i] = random;
				}
			}
		}

		private static void ComputeBranchCountsPerSection(RandomStream randomStream, DungeonProxy proxyDungeon, ref int[] mainPathBranches)
		{
			Dictionary<GraphLine, int> dictionary = new Dictionary<GraphLine, int>();
			foreach (TileProxy mainPathTile in proxyDungeon.MainPathTiles)
			{
				GraphLine graphLine = mainPathTile.Placement.GraphLine;
				if (graphLine != null && !dictionary.ContainsKey(graphLine))
				{
					DungeonArchetype archetype = mainPathTile.Placement.Archetype;
					dictionary.Add(graphLine, archetype.BranchCount.GetRandom(randomStream));
				}
			}
			foreach (KeyValuePair<GraphLine, int> item in dictionary)
			{
				GraphLine key = item.Key;
				int value = item.Value;
				Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
				for (int i = 0; i < proxyDungeon.MainPathTiles.Count; i++)
				{
					TileProxy tileProxy = proxyDungeon.MainPathTiles[i];
					if (tileProxy.Placement.GraphLine == key)
					{
						dictionary2[i] = tileProxy.UnusedDoorways.Count();
					}
				}
				int num = dictionary2.Sum((KeyValuePair<int, int> x) => x.Value);
				float[] array = new float[dictionary2.Count];
				for (int num2 = 0; num2 < dictionary2.Count; num2++)
				{
					dictionary2.Keys.ElementAt(num2);
					float num3 = (float)dictionary2.Values.ElementAt(num2) / (float)num;
					array[num2] = num3;
				}
				int[] array2 = DistributeByWeights(value, array);
				for (int num4 = 0; num4 < array2.Length; num4++)
				{
					int num5 = dictionary2.Keys.ElementAt(num4);
					mainPathBranches[num5] = array2[num4];
				}
			}
		}

		private static void ComputeBranchCountsGlobal(DungeonFlow dungeonFlow, RandomStream randomStream, DungeonProxy proxyDungeon, ref int[] mainPathBranches)
		{
			int random = dungeonFlow.BranchCount.GetRandom(randomStream);
			int num = proxyDungeon.MainPathTiles.Count((TileProxy t) => t.Placement.Archetype != null && t.UnusedDoorways.Any());
			float num2 = (float)random / (float)num;
			float num3 = num2;
			int num4 = random;
			for (int num5 = 0; num5 < mainPathBranches.Length; num5++)
			{
				if (num4 <= 0)
				{
					break;
				}
				TileProxy tileProxy = proxyDungeon.MainPathTiles[num5];
				if (!(tileProxy.Placement.Archetype == null) && tileProxy.UnusedDoorways.Any())
				{
					int num6 = tileProxy.UnusedDoorways.Count();
					int num7 = Mathf.FloorToInt(num3);
					num7 = Mathf.Min(num7, num6, tileProxy.Placement.Archetype.BranchCount.Max, num4);
					num3 -= (float)num7;
					num6 -= num7;
					if (num7 < num6 && num7 < num4 && randomStream.NextDouble() < (double)num3)
					{
						num7++;
						num3 = 0f;
					}
					num3 += num2;
					num4 -= num7;
					mainPathBranches[num5] = num7;
				}
			}
		}

		private static int[] DistributeByWeights(int count, float[] weights)
		{
			if (weights == null)
			{
				throw new ArgumentNullException("weights");
			}
			if (weights.Length == 0)
			{
				throw new ArgumentOutOfRangeException("weights", "Empty weights");
			}
			double num = ((IEnumerable<float>)weights).Sum((Func<float, double>)((float x) => x));
			if (num == 0.0)
			{
				throw new ArgumentException("Weights must not sum to 0", "weights");
			}
			int[] array = new int[weights.Length];
			double num2 = 0.0;
			for (int num3 = 0; num3 < weights.Length; num3++)
			{
				double num4 = (double)count * (double)weights[num3] / num;
				int num5 = Round(num4);
				num2 += num4 - (double)num5;
				if (num2 >= 0.5)
				{
					num5++;
					num2 -= 1.0;
				}
				else if (num2 <= -0.5)
				{
					num5--;
					num2 += 1.0;
				}
				array[num3] = num5;
			}
			return array;
			static int Round(double x)
			{
				return (int)((x >= 0.0) ? (x + 0.5) : (x - 0.5));
			}
		}
	}
}
