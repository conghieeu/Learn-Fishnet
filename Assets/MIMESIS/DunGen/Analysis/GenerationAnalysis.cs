using System;
using System.Collections.Generic;
using System.Linq;

namespace DunGen.Analysis
{
	public class GenerationAnalysis
	{
		public static readonly GenerationStatus[] MeasurableSteps = new GenerationStatus[7]
		{
			GenerationStatus.PreProcessing,
			GenerationStatus.TileInjection,
			GenerationStatus.MainPath,
			GenerationStatus.Branching,
			GenerationStatus.BranchPruning,
			GenerationStatus.InstantiatingTiles,
			GenerationStatus.PostProcessing
		};

		private readonly List<GenerationStats> statsSet = new List<GenerationStats>();

		public int TargetIterationCount { get; private set; }

		public int IterationCount { get; private set; }

		public NumberSetData MainPathRoomCount { get; private set; }

		public NumberSetData BranchPathRoomCount { get; private set; }

		public NumberSetData TotalRoomCount { get; private set; }

		public NumberSetData MaxBranchDepth { get; private set; }

		public NumberSetData TotalRetries { get; private set; }

		public Dictionary<GenerationStatus, NumberSetData> GenerationStepTimes { get; private set; }

		public NumberSetData TotalTime { get; private set; }

		public float AnalysisTime { get; private set; }

		public int SuccessCount { get; private set; }

		public float SuccessPercentage => (float)SuccessCount / (float)TargetIterationCount * 100f;

		public GenerationAnalysis(int targetIterationCount)
		{
			TargetIterationCount = targetIterationCount;
			GenerationStepTimes = new Dictionary<GenerationStatus, NumberSetData>();
		}

		public NumberSetData GetGenerationStepData(GenerationStatus step)
		{
			if (GenerationStepTimes.TryGetValue(step, out var value))
			{
				return value;
			}
			return new NumberSetData(new float[0]);
		}

		public void Clear()
		{
			IterationCount = 0;
			AnalysisTime = 0f;
			SuccessCount = 0;
			statsSet.Clear();
			GenerationStepTimes.Clear();
		}

		public void Add(GenerationStats stats)
		{
			statsSet.Add(stats.Clone());
			AnalysisTime += stats.TotalTime;
			IterationCount++;
		}

		public void IncrementSuccessCount()
		{
			SuccessCount++;
		}

		public void Analyze()
		{
			MainPathRoomCount = new NumberSetData(((IEnumerable<GenerationStats>)statsSet).Select((Func<GenerationStats, float>)((GenerationStats x) => x.MainPathRoomCount)));
			BranchPathRoomCount = new NumberSetData(((IEnumerable<GenerationStats>)statsSet).Select((Func<GenerationStats, float>)((GenerationStats x) => x.BranchPathRoomCount)));
			TotalRoomCount = new NumberSetData(((IEnumerable<GenerationStats>)statsSet).Select((Func<GenerationStats, float>)((GenerationStats x) => x.TotalRoomCount)));
			MaxBranchDepth = new NumberSetData(((IEnumerable<GenerationStats>)statsSet).Select((Func<GenerationStats, float>)((GenerationStats x) => x.MaxBranchDepth)));
			TotalRetries = new NumberSetData(((IEnumerable<GenerationStats>)statsSet).Select((Func<GenerationStats, float>)((GenerationStats x) => x.TotalRetries)));
			GenerationStatus[] measurableSteps = MeasurableSteps;
			foreach (GenerationStatus step in measurableSteps)
			{
				GenerationStepTimes[step] = new NumberSetData(statsSet.Select((GenerationStats x) => x.GetGenerationStepTime(step)));
			}
			TotalTime = new NumberSetData(statsSet.Select((GenerationStats x) => x.TotalTime));
		}
	}
}
