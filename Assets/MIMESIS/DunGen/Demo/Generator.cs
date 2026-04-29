using System;
using System.Text;
using DunGen.Analysis;
using UnityEngine;

namespace DunGen.Demo
{
	public class Generator : MonoBehaviour
	{
		public RuntimeDungeon DungeonGenerator;

		public Action<StringBuilder> GetAdditionalText;

		private StringBuilder infoText = new StringBuilder();

		private bool showStats = true;

		private float keypressDelay = 0.1f;

		private float timeSinceLastPress;

		private bool allowHold;

		private bool isKeyDown;

		private void Start()
		{
			DungeonGenerator = GetComponentInChildren<RuntimeDungeon>();
			DungeonGenerator.Generator.OnGenerationStatusChanged += OnGenerationStatusChanged;
			GenerateRandom();
		}

		private void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			infoText.Length = 0;
			switch (status)
			{
			case GenerationStatus.Failed:
				infoText.Append("Generation Failed");
				break;
			default:
				infoText.Append($"Generating ({status})...");
				break;
			case GenerationStatus.NotStarted:
				break;
			case GenerationStatus.Complete:
			{
				infoText.AppendLine("Seed: " + generator.ChosenSeed);
				infoText.AppendLine();
				infoText.Append("## TIME TAKEN ##");
				GenerationStatus[] measurableSteps = GenerationAnalysis.MeasurableSteps;
				for (int i = 0; i < measurableSteps.Length; i++)
				{
					GenerationStatus step = measurableSteps[i];
					float generationStepTime = generator.GenerationStats.GetGenerationStepTime(step);
					AddEntry(infoText, step.ToString(), $"{generationStepTime:0.00} ms ({generationStepTime / generator.GenerationStats.TotalTime:P0})");
				}
				infoText.Append("\n\t-------------------------------------------------------");
				AddEntry(infoText, "Total", $"{generator.GenerationStats.TotalTime:0.00} ms");
				infoText.AppendLine();
				infoText.AppendLine();
				infoText.AppendLine("## ROOM COUNT ##");
				infoText.Append($"\n\tMain Path: {generator.GenerationStats.MainPathRoomCount}");
				infoText.Append($"\n\tBranch Paths: {generator.GenerationStats.BranchPathRoomCount}");
				infoText.Append("\n\t-------------------");
				infoText.Append($"\n\tTotal: {generator.GenerationStats.TotalRoomCount}");
				infoText.AppendLine();
				infoText.AppendLine();
				infoText.Append($"\n\tRetry Count: {generator.GenerationStats.TotalRetries}");
				infoText.AppendLine();
				infoText.AppendLine();
				infoText.AppendLine("Press 'F1' to toggle this information");
				infoText.AppendLine("Press 'R' to generate a new layout");
				if (GetAdditionalText != null)
				{
					GetAdditionalText(infoText);
				}
				break;
			}
			}
			static void AddEntry(StringBuilder stringBuilder, string title, string entry)
			{
				string text = new string(' ', 20 - title.Length);
				stringBuilder.Append("\n\t" + title + ":" + text + "\t" + entry);
			}
		}

		public void GenerateRandom()
		{
			DungeonGenerator.Generate();
		}

		private void Update()
		{
			timeSinceLastPress += Time.deltaTime;
			if (Input.GetKeyDown(KeyCode.R))
			{
				timeSinceLastPress = 0f;
				isKeyDown = true;
				GenerateRandom();
			}
			if (Input.GetKeyUp(KeyCode.R))
			{
				isKeyDown = false;
				allowHold = false;
			}
			if (!allowHold && isKeyDown && timeSinceLastPress >= keypressDelay)
			{
				allowHold = true;
				timeSinceLastPress = 0f;
			}
			if (allowHold && Input.GetKey(KeyCode.R) && timeSinceLastPress >= keypressDelay)
			{
				GenerateRandom();
				timeSinceLastPress = 0f;
			}
			if (Input.GetKeyDown(KeyCode.F1))
			{
				showStats = !showStats;
			}
		}

		private void OnGUI()
		{
			if (showStats)
			{
				GUILayout.Label(infoText.ToString());
			}
		}
	}
}
