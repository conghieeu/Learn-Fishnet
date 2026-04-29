using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
	private const int maxHistory = 100;

	private GameObject panel;

	public InputField input;

	public Text output;

	private List<string> outputBuffer = new List<string>();

	private Dictionary<string, Action<string>> callbacks = new Dictionary<string, Action<string>>();

	private List<string> history = new List<string>();

	private int recallHistoryCursor = -1;

	private ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();

	private void Awake()
	{
		Logger.RLog("[AwakeLogs] DebugConsole.Awake ->");
		outputBuffer = new List<string>();
		panel = base.transform.GetChild(0).gameObject;
		Logger.RLog("[AwakeLogs] DebugConsole.Awake <-");
	}

	private void Start()
	{
		Logger.RLog("[AwakeLogs] DebugConsole.Start ->");
		string text = PlayerPrefs.GetString("debug_console_history", "");
		if (text != "")
		{
			history = new List<string>(text.Split('\n'));
		}
		Logger.RLog("[AwakeLogs] DebugConsole.Start <-");
	}

	private void Update()
	{
		Action result;
		while (actionQueue.TryDequeue(out result))
		{
			result?.Invoke();
		}
	}

	public void ThreadSafe(Action action)
	{
		actionQueue.Enqueue(action);
	}

	public void ToggleConsole()
	{
		panel.SetActive(!panel.activeSelf);
		if (panel.activeInHierarchy)
		{
			output.text = string.Join("\n", outputBuffer);
			GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = -0.1f;
		}
	}

	public bool IsConsoleActive()
	{
		return panel.activeInHierarchy;
	}

	[Conditional("DEV")]
	public void Log(string msg)
	{
		ThreadSafe(delegate
		{
			_Log(msg);
		});
	}

	private void _Log(string msg)
	{
		string[] collection = msg.Split('\n');
		outputBuffer.AddRange(collection);
		while (outputBuffer.Count > 100)
		{
			outputBuffer.RemoveAt(0);
		}
		output.text = string.Join("\n", outputBuffer);
		if (panel.activeInHierarchy)
		{
			GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = -0.1f;
		}
	}

	public void OnInputSubmit()
	{
		string text = input.text;
		Interpret(text);
		input.text = "";
		input.ActivateInputField();
	}

	public void Interpret(string line)
	{
		string text = "";
		string text2 = "";
		line = line.Trim();
		int num = line.IndexOf(" ");
		if (num == -1)
		{
			text = line;
			text2 = "";
		}
		else
		{
			text = line.Substring(0, num);
			text2 = line.Substring(num + 1);
		}
		if (callbacks.ContainsKey(text))
		{
			try
			{
				callbacks[text]?.Invoke(text2);
				AddToHistory(line);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("* exception in console callback");
				UnityEngine.Debug.Log(ex.ToString());
			}
		}
		else
		{
			ListCallback("");
		}
		input.text = "";
		input.ActivateInputField();
	}

	private void AddToHistory(string line)
	{
		if (history.Contains(line))
		{
			history.Remove(line);
		}
		history.Add(line);
		PlayerPrefs.SetString("debug_console_history", string.Join("\n", history));
		if (history.Count > 100)
		{
			history.RemoveAt(0);
		}
		ResetRecallHistoryCallback();
	}

	private void ResetRecallHistoryCallback()
	{
		recallHistoryCursor = -1;
	}

	private void RecallUp()
	{
		if (history.Count != 0)
		{
			if (recallHistoryCursor == -1)
			{
				recallHistoryCursor = history.Count - 1;
				input.text = history[recallHistoryCursor];
			}
			else if (recallHistoryCursor > 0)
			{
				recallHistoryCursor--;
				input.text = history[recallHistoryCursor];
			}
		}
	}

	private void RecallDown()
	{
		if (history.Count != 0)
		{
			if (recallHistoryCursor == -1)
			{
				recallHistoryCursor = history.Count - 1;
				input.text = history[recallHistoryCursor];
			}
			else if (recallHistoryCursor < history.Count - 1)
			{
				recallHistoryCursor++;
				input.text = history[recallHistoryCursor];
			}
		}
	}

	[Conditional("DEV")]
	public void RegisterCallback(string command, Action<string> action)
	{
		callbacks[command] = action;
	}

	[Conditional("DEV")]
	public void UnregisterCallback(string command)
	{
		if (callbacks.ContainsKey(command))
		{
			callbacks.Remove(command);
		}
	}

	private void RunCheatFileCallback(string _)
	{
		string text = Application.streamingAssetsPath + "/Cheats/" + _;
		if (File.Exists(text))
		{
			StartCoroutine(RunCheatFileCoroutine(text));
		}
	}

	private IEnumerator RunCheatFileCoroutine(string cheatFile)
	{
		string[] array = File.ReadAllLines(cheatFile);
		string[] array2 = array;
		foreach (string line in array2)
		{
			Interpret(line);
			yield return new WaitForSeconds(0.2f);
		}
	}

	private void ClearCallback(string _)
	{
		outputBuffer.Clear();
		output.text = "";
	}

	private void ListCallback(string _)
	{
		foreach (string key in callbacks.Keys)
		{
			string text = key;
		}
	}

	private void ShowHistoryCallback(string obj)
	{
		foreach (string item in history)
		{
			_ = item;
		}
	}
}
