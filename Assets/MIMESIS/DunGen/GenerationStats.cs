using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DunGen
{
	public sealed class GenerationStats
	{
		private Stopwatch stopwatch = new Stopwatch();

		private GenerationStatus generationStatus;

		public int MainPathRoomCount { get; private set; }

		public int BranchPathRoomCount { get; private set; }

		public int TotalRoomCount { get; private set; }

		public int MaxBranchDepth { get; private set; }

		public int TotalRetries { get; private set; }

		public int PrunedBranchTileCount { get; internal set; }

		public Dictionary<GenerationStatus, float> GenerationStepTimes { get; private set; }

		public float TotalTime => GenerationStepTimes.Values.Sum();

		public GenerationStats()
		{
			GenerationStepTimes = new Dictionary<GenerationStatus, float>();
		}

		public float GetGenerationStepTime(GenerationStatus step)
		{
			if (GenerationStepTimes.TryGetValue(step, out var value))
			{
				return value;
			}
			return 0f;
		}

		internal void Clear()
		{
			MainPathRoomCount = 0;
			BranchPathRoomCount = 0;
			TotalRoomCount = 0;
			MaxBranchDepth = 0;
			TotalRetries = 0;
			PrunedBranchTileCount = 0;
			GenerationStepTimes.Clear();
		}

		internal void IncrementRetryCount()
		{
			TotalRetries++;
		}

		internal void SetRoomStatistics(int mainPathRoomCount, int branchPathRoomCount, int maxBranchDepth)
		{
			MainPathRoomCount = mainPathRoomCount;
			BranchPathRoomCount = branchPathRoomCount;
			MaxBranchDepth = maxBranchDepth;
			TotalRoomCount = MainPathRoomCount + BranchPathRoomCount;
		}

		internal void BeginTime(GenerationStatus status)
		{
			if (stopwatch.IsRunning)
			{
				EndTime();
			}
			generationStatus = status;
			stopwatch.Reset();
			stopwatch.Start();
		}

		internal void EndTime()
		{
			stopwatch.Stop();
			float num = (float)stopwatch.Elapsed.TotalMilliseconds;
			GenerationStepTimes.TryGetValue(generationStatus, out var value);
			value += num;
			GenerationStepTimes[generationStatus] = value;
		}

		public GenerationStats Clone()
		{
			return new GenerationStats
			{
				MainPathRoomCount = MainPathRoomCount,
				BranchPathRoomCount = BranchPathRoomCount,
				TotalRoomCount = TotalRoomCount,
				MaxBranchDepth = MaxBranchDepth,
				TotalRetries = TotalRetries,
				PrunedBranchTileCount = PrunedBranchTileCount,
				GenerationStepTimes = new Dictionary<GenerationStatus, float>(GenerationStepTimes)
			};
		}
	}
}
