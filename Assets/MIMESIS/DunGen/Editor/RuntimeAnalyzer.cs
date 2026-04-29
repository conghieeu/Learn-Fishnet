using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DunGen.Analysis;
using DunGen.Graph;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DunGen.Editor
{
	[AddComponentMenu("DunGen/Analysis/Runtime Analyzer")]
	public sealed class RuntimeAnalyzer : MonoBehaviour
	{
		[Serializable]
		private class StatEntry
		{
			public float min;

			public float max;

			public float average;

			public float std;

			public StatEntry()
			{
			}

			public StatEntry(NumberSetData d)
			{
				min = d.Min;
				max = d.Max;
				average = d.Average;
				std = d.StandardDeviation;
			}
		}

		[Serializable]
		private class StepEntry
		{
			public string step;

			public StatEntry time;
		}

		[Serializable]
		private class AnalysisExport
		{
			public string dungeonFlowName;

			public int targetIterations;

			public int completedIterations;

			public int successCount;

			public float successPercentage;

			public float totalAnalysisSeconds;

			public StepEntry[] steps;

			public StatEntry totalTime;

			public StatEntry mainPathRooms;

			public StatEntry branchPathRooms;

			public StatEntry totalRooms;

			public TileUsageEntry[] tileUsage;
		}

		[Serializable]
		private class TileUsageEntry
		{
			public string name;

			public int totalUses;

			public float avgPerSuccessfulDungeon;
		}

		public DungeonFlow DungeonFlow;

		public int Iterations = 100;

		public int MaxFailedAttempts = 20;

		public bool RunOnStart = true;

		public float MaximumAnalysisTime;

		[Header("File Output")]
		public bool SaveToFile = true;

		public string OutputFolderName = "DunGenAnalysis";

		public bool SaveAsText = true;

		public bool SaveAsCSV = true;

		public bool SaveAsJSON;

		[Header("UI & Visuals")]
		[Tooltip("결과 텍스트의 폰트 크기 (실시간 조절 가능)")]
		[Range(10f, 50f)]
		public int ResultFontSize = 22;

		[Space(10f)]
		[Tooltip("애니메이션 방식 선택")]
		public LoadingAnimationMode AnimationMode;

		[Tooltip("[Static/Spin 모드용] 단일 이미지")]
		public Texture2D LoadingImage;

		[Tooltip("[FrameSequence 모드용] 재생할 이미지 리스트")]
		public List<Texture2D> AnimationFrames = new List<Texture2D>();

		[Tooltip("애니메이션 속도")]
		public float AnimationSpeed = 10f;

		[Tooltip("로딩 배경 암전 정도 (0~1)")]
		[Range(0f, 1f)]
		public float BackgroundDimAlpha = 0.85f;

		[Header("Tile Usage Analysis")]
		public bool AutoTrackTiles = true;

		public List<string> TrackedTileNames = new List<string>();

		private Dictionary<string, int> tileUsageCounts = new Dictionary<string, int>();

		private bool loggedTileKeysOnce;

		private const int TextPadding = 20;

		private DungeonGenerator generator = new DungeonGenerator();

		private GenerationAnalysis analysis;

		private StringBuilder infoText = new StringBuilder();

		private bool finishedEarly;

		private bool prevShouldRandomizeSeed;

		private int targetIterations;

		private int remainingIterations;

		private Stopwatch analysisTime;

		private bool generateNextFrame;

		private GameObject _lastCountedRoot;

		public static bool IsRunning;

		private string _currentFunnyMessage = "던전 입구를 찾는 중...";

		private float _messageTimer;

		private float _messageInterval = 2.5f;

		private readonly List<string> _funnyMessages = new List<string>
		{
			"던전 깎는 노인 빙의 중...", "고블린 입주시키는 중...", "함정 위치 선정 중...", "보물 상자에 꽝 넣는 중...", "Unity 달래는 중...", "커피 한 잔 하고 오세요 ☕", "벽 뒤에 공간 있어요 (아마도)", "NavMesh 굽는 냄새...", "주사위 굴리는 중 \ud83c\udfb2", "이 던전은 안전합니다 (거짓말)",
			"엑셀로 데이터 굽는 중...", "메모리 누수 막는 중...", "심심하시죠? 저도 힘들어요...", "버그 잡는 중 (아마도 심는 중)", "타일 조각 맞추는 중 \ud83e\udde9"
		};

		private GUIStyle _loadingTextStyle;

		private GUIStyle _resultTextStyle;

		private Texture2D _whiteTexture;

		private float _rotationAngle;

		private Vector2 _scrollPosition = Vector2.zero;

		private int currentIterations => targetIterations - remainingIterations;

		private void Start()
		{
			if (RunOnStart)
			{
				Analyze();
			}
		}

		public void Analyze()
		{
			if (ValidateSetup())
			{
				PrepareAnalysis();
				IsRunning = true;
				PickRandomMessage();
				generator.OnGenerationStatusChanged -= OnGenerationStatusChanged;
				generator.OnGenerationStatusChanged += OnGenerationStatusChanged;
				generator.Generate();
			}
		}

		private bool ValidateSetup()
		{
			if (DungeonFlow == null)
			{
				UnityEngine.Debug.LogError("No DungeonFlow assigned");
				return false;
			}
			if (Iterations <= 0)
			{
				UnityEngine.Debug.LogError("Iterations must be > 0");
				return false;
			}
			return true;
		}

		private void PrepareAnalysis()
		{
			tileUsageCounts.Clear();
			if (!AutoTrackTiles && TrackedTileNames != null)
			{
				foreach (string trackedTileName in TrackedTileNames)
				{
					if (!string.IsNullOrEmpty(trackedTileName))
					{
						string key = NormalizeName(trackedTileName);
						if (!tileUsageCounts.ContainsKey(key))
						{
							tileUsageCounts.Add(key, 0);
						}
					}
				}
			}
			prevShouldRandomizeSeed = generator.ShouldRandomizeSeed;
			generator.IsAnalysis = true;
			generator.DungeonFlow = DungeonFlow;
			generator.MaxAttemptCount = MaxFailedAttempts;
			generator.ShouldRandomizeSeed = true;
			analysis = new GenerationAnalysis(Iterations);
			analysisTime = Stopwatch.StartNew();
			remainingIterations = (targetIterations = Iterations);
			_lastCountedRoot = null;
		}

		private void Update()
		{
			if (MaximumAnalysisTime > 0f && analysisTime.Elapsed.TotalSeconds >= (double)MaximumAnalysisTime)
			{
				remainingIterations = 0;
				finishedEarly = true;
			}
			if (generateNextFrame)
			{
				generateNextFrame = false;
				generator.Generate();
			}
			if (IsRunning)
			{
				_messageTimer += Time.deltaTime;
				if (_messageTimer >= _messageInterval)
				{
					_messageTimer = 0f;
					PickRandomMessage();
				}
				if (AnimationMode == LoadingAnimationMode.Spin)
				{
					_rotationAngle += AnimationSpeed * Time.deltaTime * 10f;
					_rotationAngle %= 360f;
				}
			}
		}

		private void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			if (status != GenerationStatus.Complete || generator.Root == _lastCountedRoot)
			{
				return;
			}
			_lastCountedRoot = generator.Root;
			CountTrackedTiles(generator.Root);
			analysis.IncrementSuccessCount();
			analysis.Add(generator.GenerationStats);
			remainingIterations--;
			if (remainingIterations > 0)
			{
				if (generator.Root != null)
				{
					UnityUtil.Destroy(generator.Root);
				}
				generateNextFrame = true;
			}
			else
			{
				generator.OnGenerationStatusChanged -= OnGenerationStatusChanged;
				CompleteAnalysis();
			}
		}

		private void CompleteAnalysis()
		{
			IsRunning = false;
			analysisTime.Stop();
			analysis.Analyze();
			OnAnalysisComplete();
		}

		private void OnGUI()
		{
			InitializeStyles();
			if (IsRunning)
			{
				DrawLoadingScreen();
			}
			else if (infoText != null && infoText.Length > 0)
			{
				DrawColorRect(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.8f));
				float num = 40f;
				GUILayout.BeginArea(new Rect(num, num, (float)Screen.width - num * 2f, (float)Screen.height - num * 2f));
				_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
				GUILayout.Label(infoText.ToString(), _resultTextStyle);
				GUILayout.EndScrollView();
				GUILayout.EndArea();
			}
		}

		private void InitializeStyles()
		{
			if (_loadingTextStyle == null)
			{
				_loadingTextStyle = new GUIStyle(GUI.skin.label);
				_loadingTextStyle.fontSize = 20;
				_loadingTextStyle.fontStyle = FontStyle.Bold;
				_loadingTextStyle.alignment = TextAnchor.MiddleCenter;
				_loadingTextStyle.normal.textColor = Color.white;
				_whiteTexture = new Texture2D(1, 1);
				_whiteTexture.SetPixel(0, 0, Color.white);
				_whiteTexture.Apply();
			}
			if (_resultTextStyle == null)
			{
				_resultTextStyle = new GUIStyle(GUI.skin.label);
				_resultTextStyle.normal.textColor = Color.white;
				_resultTextStyle.wordWrap = true;
				_resultTextStyle.richText = true;
			}
			_resultTextStyle.fontSize = ResultFontSize;
		}

		private void DrawLoadingScreen()
		{
			float num = Screen.width;
			float num2 = Screen.height;
			DrawColorRect(color: new Color(0f, 0f, 0f, BackgroundDimAlpha), rect: new Rect(0f, 0f, num, num2));
			float num3 = num2 * 0.3f;
			Texture2D texture2D = null;
			switch (AnimationMode)
			{
			case LoadingAnimationMode.Static:
				texture2D = LoadingImage;
				break;
			case LoadingAnimationMode.FrameSequence:
				if (AnimationFrames != null && AnimationFrames.Count > 0)
				{
					int index = (int)(Time.time * AnimationSpeed) % AnimationFrames.Count;
					texture2D = AnimationFrames[index];
				}
				else
				{
					texture2D = LoadingImage;
				}
				break;
			case LoadingAnimationMode.Spin:
				texture2D = LoadingImage;
				break;
			}
			if (texture2D != null)
			{
				float num4 = (float)texture2D.width / (float)texture2D.height;
				float num5 = num2 * 0.3f;
				float num6 = num5 * num4;
				Rect position = new Rect((num - num6) / 2f, num3, num6, num5);
				if (AnimationMode == LoadingAnimationMode.Spin)
				{
					Vector2 center = position.center;
					GUIUtility.RotateAroundPivot(_rotationAngle, center);
					GUI.DrawTexture(position, texture2D, ScaleMode.ScaleToFit);
					GUI.matrix = Matrix4x4.identity;
				}
				else
				{
					GUI.DrawTexture(position, texture2D, ScaleMode.ScaleToFit);
				}
				num3 += num5 + 20f;
			}
			else
			{
				num3 = num2 * 0.45f;
			}
			float num7 = ((targetIterations > 0) ? ((float)currentIterations / (float)targetIterations) : 0f);
			float num8 = num * 0.6f;
			float num9 = 20f;
			float x = (num - num8) / 2f;
			DrawColorRect(new Rect(x, num3, num8, num9), new Color(0.3f, 0.3f, 0.3f, 1f));
			DrawColorRect(new Rect(x, num3, num8 * num7, num9), new Color(0.2f, 0.8f, 0.2f, 1f));
			float num10 = num3 + num9 + 20f;
			string text = $"{currentIterations} / {targetIterations} ({num7 * 100f:0}%)";
			GUI.Label(new Rect(0f, num10, num, 30f), text, _loadingTextStyle);
			Color color = GUI.color;
			GUI.color = new Color(1f, 0.8f, 0.2f);
			GUI.Label(new Rect(0f, num10 + 35f, num, 40f), _currentFunnyMessage, _loadingTextStyle);
			GUI.color = color;
		}

		private void DrawColorRect(Rect rect, Color color)
		{
			Color color2 = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(rect, _whiteTexture);
			GUI.color = color2;
		}

		private void PickRandomMessage()
		{
			if (_funnyMessages.Count > 0)
			{
				_currentFunnyMessage = _funnyMessages[UnityEngine.Random.Range(0, _funnyMessages.Count)];
			}
		}

		private static string NormalizeName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}
			int num = name.LastIndexOf("(Clone)", StringComparison.Ordinal);
			if (num >= 0)
			{
				name = name.Substring(0, num);
			}
			int result;
			if (name.StartsWith("(", StringComparison.Ordinal))
			{
				int num2 = name.IndexOf(')');
				if (num2 > 0 && num2 + 1 < name.Length && int.TryParse(name.Substring(1, num2 - 1), out result))
				{
					name = name.Substring(num2 + 1);
				}
			}
			int num3 = name.LastIndexOf(" (", StringComparison.Ordinal);
			if (num3 >= 0 && name.EndsWith(")", StringComparison.Ordinal) && int.TryParse(name.Substring(num3 + 2, name.Length - num3 - 3), out result))
			{
				name = name.Substring(0, num3);
			}
			return name.Trim();
		}

		private void CountTrackedTiles(GameObject dungeonRoot)
		{
			if (dungeonRoot == null)
			{
				return;
			}
			if (AutoTrackTiles)
			{
				Tile[] componentsInChildren = dungeonRoot.GetComponentsInChildren<Tile>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					string key = NormalizeName(componentsInChildren[i].gameObject.name);
					if (!tileUsageCounts.ContainsKey(key))
					{
						tileUsageCounts.Add(key, 0);
					}
					tileUsageCounts[key]++;
				}
			}
			else
			{
				if (tileUsageCounts.Count == 0)
				{
					return;
				}
				Transform[] componentsInChildren2 = dungeonRoot.GetComponentsInChildren<Transform>(includeInactive: true);
				foreach (Transform transform in componentsInChildren2)
				{
					if (!(transform.gameObject == dungeonRoot))
					{
						string key2 = NormalizeName(transform.gameObject.name);
						if (tileUsageCounts.ContainsKey(key2))
						{
							tileUsageCounts[key2]++;
						}
					}
				}
			}
		}

		private (string flowName, string assetPath, string guid, string sceneName, string unityVersion) GetTestMeta()
		{
			string item = (DungeonFlow ? DungeonFlow.name : "UnknownFlow");
			string item2 = "N/A";
			string item3 = "N/A";
			return (flowName: item, assetPath: item2, guid: item3, sceneName: SceneManager.GetActiveScene().name, unityVersion: Application.unityVersion);
		}

		private void OnAnalysisComplete()
		{
			generator.ShouldRandomizeSeed = prevShouldRandomizeSeed;
			infoText.Length = 0;
			(string, string, string, string, string) testMeta = GetTestMeta();
			infoText.AppendLine("DungeonFlow: " + testMeta.Item1);
			if (finishedEarly)
			{
				infoText.AppendLine("[ Reached maximum analysis time before target iterations ]");
			}
			infoText.AppendFormat("Iterations: {0}, Max Failed: {1}", finishedEarly ? analysis.IterationCount : analysis.TargetIterationCount, MaxFailedAttempts);
			infoText.AppendFormat("\nTime: {0:0.00}s", analysisTime.Elapsed.TotalSeconds);
			infoText.AppendFormat("\nSuccess: {0}% ({1} failed)", Mathf.RoundToInt(analysis.SuccessPercentage), analysis.TargetIterationCount - analysis.SuccessCount);
			infoText.AppendLine("\n\n## TIME TAKEN (ms) ##");
			GenerationStatus[] measurableSteps = GenerationAnalysis.MeasurableSteps;
			for (int i = 0; i < measurableSteps.Length; i++)
			{
				GenerationStatus step = measurableSteps[i];
				AddInfoEntry(infoText, step.ToString(), analysis.GetGenerationStepData(step));
			}
			infoText.Append("\n\t--------------------------");
			AddInfoEntry(infoText, "Total", analysis.TotalTime);
			infoText.AppendLine("\n\n## ROOM DATA ##");
			AddInfoEntry(infoText, "Main Rooms", analysis.MainPathRoomCount);
			AddInfoEntry(infoText, "Branch Rooms", analysis.BranchPathRoomCount);
			infoText.Append("\n\t-------------------");
			AddInfoEntry(infoText, "Total", analysis.TotalRoomCount);
			infoText.AppendFormat("\nRetry Count: {0}", analysis.TotalRetries);
			if (tileUsageCounts.Count > 0)
			{
				infoText.AppendLine("\n\n## TILE USAGE ##");
				int num = Mathf.Max(1, analysis.SuccessCount);
				HashSet<string> hashSet = new HashSet<string>();
				if (TrackedTileNames != null)
				{
					foreach (string trackedTileName in TrackedTileNames)
					{
						hashSet.Add(NormalizeName(trackedTileName));
					}
				}
				foreach (string item in tileUsageCounts.Keys.OrderBy((string x) => x).ToList())
				{
					int num2 = tileUsageCounts[item];
					float num3 = (float)num2 / (float)num;
					string arg = item;
					if (hashSet.Contains(item))
					{
						arg = "<color=#00FFFF>" + item + "</color>";
					}
					infoText.AppendFormat("\n\t{0}: {1} (Avg {2:0.00})", arg, num2, num3);
				}
			}
			if (SaveToFile)
			{
				TrySaveFiles();
			}
		}

		private static void AddInfoEntry(StringBuilder sb, string title, NumberSetData data)
		{
			string arg = new string(' ', Mathf.Max(0, 20 - title.Length));
			sb.Append($"\n\t{title}:{arg}\t{data}");
		}

		private void TrySaveFiles()
		{
			try
			{
				string text = Path.Combine(Application.isEditor ? Application.dataPath : Application.persistentDataPath, OutputFolderName);
				Directory.CreateDirectory(text);
				string text2 = DateTime.Now.ToString("yyyyMMdd_HHmmss");
				string text3 = "Analysis_" + (DungeonFlow?.name ?? "Unknown") + "_" + text2;
				if (SaveAsText)
				{
					File.WriteAllText(Path.Combine(text, text3 + ".txt"), infoText.ToString(), Encoding.UTF8);
				}
				if (SaveAsCSV)
				{
					File.WriteAllText(Path.Combine(text, text3 + "_steps.csv"), BuildStepsCSV(), Encoding.UTF8);
					File.WriteAllText(Path.Combine(text, text3 + "_rooms.csv"), BuildRoomsCSV(), Encoding.UTF8);
					File.WriteAllText(Path.Combine(text, text3 + "_tiles.csv"), BuildTilesCSV(), Encoding.UTF8);
				}
				if (SaveAsJSON)
				{
					AnalysisExport obj = BuildJsonObject();
					File.WriteAllText(Path.Combine(text, text3 + ".json"), JsonUtility.ToJson(obj, prettyPrint: true), Encoding.UTF8);
				}
				UnityEngine.Debug.Log("[RuntimeAnalyzer] Saved to: " + text);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("Save Failed: " + ex.Message);
			}
		}

		private string BuildStepsCSV()
		{
			StringBuilder stringBuilder = new StringBuilder("Step,Min,Max,Avg,StdDev\n");
			GenerationStatus[] measurableSteps = GenerationAnalysis.MeasurableSteps;
			foreach (GenerationStatus generationStatus in measurableSteps)
			{
				NumberSetData generationStepData = analysis.GetGenerationStepData(generationStatus);
				stringBuilder.AppendLine($"{generationStatus},{generationStepData.Min:0.0},{generationStepData.Max:0.0},{generationStepData.Average:0.0},{generationStepData.StandardDeviation:0.00}");
			}
			NumberSetData totalTime = analysis.TotalTime;
			stringBuilder.AppendLine($"Total,{totalTime.Min:0.0},{totalTime.Max:0.0},{totalTime.Average:0.0},{totalTime.StandardDeviation:0.00}");
			return stringBuilder.ToString();
		}

		private string BuildRoomsCSV()
		{
			StringBuilder stringBuilder = new StringBuilder("Metric,Min,Max,Avg,StdDev\n");
			NumberSetData mainPathRoomCount = analysis.MainPathRoomCount;
			NumberSetData branchPathRoomCount = analysis.BranchPathRoomCount;
			NumberSetData totalRoomCount = analysis.TotalRoomCount;
			stringBuilder.AppendLine($"Main,{mainPathRoomCount.Min},{mainPathRoomCount.Max},{mainPathRoomCount.Average:0.0},{mainPathRoomCount.StandardDeviation:0.00}");
			stringBuilder.AppendLine($"Branch,{branchPathRoomCount.Min},{branchPathRoomCount.Max},{branchPathRoomCount.Average:0.0},{branchPathRoomCount.StandardDeviation:0.00}");
			stringBuilder.AppendLine($"Total,{totalRoomCount.Min},{totalRoomCount.Max},{totalRoomCount.Average:0.0},{totalRoomCount.StandardDeviation:0.00}");
			return stringBuilder.ToString();
		}

		private string BuildTilesCSV()
		{
			StringBuilder stringBuilder = new StringBuilder("Tile,Total,Avg\n");
			int num = Mathf.Max(1, analysis.SuccessCount);
			foreach (string key in tileUsageCounts.Keys)
			{
				stringBuilder.AppendLine($"{key},{tileUsageCounts[key]},{(float)tileUsageCounts[key] / (float)num:0.00}");
			}
			return stringBuilder.ToString();
		}

		private AnalysisExport BuildJsonObject()
		{
			return new AnalysisExport
			{
				dungeonFlowName = (DungeonFlow ? DungeonFlow.name : "Unknown"),
				targetIterations = analysis.TargetIterationCount,
				completedIterations = analysis.IterationCount,
				successCount = analysis.SuccessCount,
				successPercentage = analysis.SuccessPercentage,
				totalAnalysisSeconds = (float)analysisTime.Elapsed.TotalSeconds,
				steps = GenerationAnalysis.MeasurableSteps.Select((GenerationStatus s) => new StepEntry
				{
					step = s.ToString(),
					time = new StatEntry(analysis.GetGenerationStepData(s))
				}).ToArray(),
				totalTime = new StatEntry(analysis.TotalTime),
				mainPathRooms = new StatEntry(analysis.MainPathRoomCount),
				branchPathRooms = new StatEntry(analysis.BranchPathRoomCount),
				totalRooms = new StatEntry(analysis.TotalRoomCount),
				tileUsage = tileUsageCounts.Select((KeyValuePair<string, int> k) => new TileUsageEntry
				{
					name = k.Key,
					totalUses = k.Value,
					avgPerSuccessfulDungeon = (float)k.Value / (float)Mathf.Max(1, analysis.SuccessCount)
				}).ToArray()
			};
		}
	}
}
